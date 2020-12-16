﻿using CellMenu;
using DataDumper.Managers;
using GTFO_DataDumper.HotReload;
using Harmony;
using MelonLoader;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Text;
using UnhollowerRuntimeLib;

namespace DataDumper
{
    public class DataDumperMain : MelonMod
    {
        public static Dictionary<int, string> gameDataLookup;
        public CustomManager CustomManager;
        public const string
            MODNAME = "Data-Dumper",
            AUTHOR = "Dak",
            GUID = "com." + AUTHOR + "." + MODNAME,
            VERSION = "2.1.0";


        public override void OnApplicationStart()
        {
            //Inject hot reloader
            ClassInjector.RegisterTypeInIl2Cpp<HotReloader>();
            if (ConfigManager.HasCustomContent)
            {
                CustomManager = new CustomManager();
            }

            MelonLogger.Log($"Game Version: {ConfigManager.GAME_VERSION}");

            //Setup hotreload if enabled
            var harmony = HarmonyInstance.Create(GUID);
            if (ConfigManager.IsHotReloadEnabled)
            {
                var hotReloadInjectPoint = typeof(CM_PageIntro).GetMethod("EXT_PressInject");
                var hotReloadPatch = typeof(HotReloadInjector).GetMethod("PostFix");
                harmony.Patch(hotReloadInjectPoint, null, new HarmonyMethod(hotReloadPatch));
            }

            /*
            *  Hash local game data for comparing
            */
            //Create gamedata lookup
            MelonLogger.Log("Hashing GameData...");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            gameDataLookup = new Dictionary<int, string>();
            ResourceSet gameData = GameData.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, true);
            IDictionaryEnumerator gameDataCollection = gameData.GetEnumerator();

            while (gameDataCollection.MoveNext())
            {
                byte[] byteKey = gameDataCollection.Value as byte[];
                string key = Encoding.UTF8.GetString(byteKey);
                int hash = key.GetHashCode();
                gameDataLookup.Add(hash, gameDataCollection.Key as string);
            }
            sw.Stop();
            MelonLogger.Log("Hash done!");
            MelonLogger.Log("Time elapsed: " + sw.Elapsed);
        }
    }
}