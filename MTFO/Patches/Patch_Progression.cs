using HarmonyLib;
using MTFO.Managers;
using MTFO.Utilities;
using ChainedPuzzles;
using UnityEngine;
using Newtonsoft.Json;
using BepInEx;
using System.IO;
using DropServer;

namespace MTFO.Patches
{
    [HarmonyPatch(typeof(RundownManager), nameof(RundownManager.OnRundownProgressionRequestDone))]
    internal static class Patch_RundownProgression
    {

        internal static void Postfix()
        {

            File.WriteAllText(Path.Combine(Paths.ConfigPath, "progression.json"), JsonConvert.SerializeObject(RundownManager.RundownProgression));

        }

    }
}
