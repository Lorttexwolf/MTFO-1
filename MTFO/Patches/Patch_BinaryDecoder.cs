﻿using MTFO.Managers;
using MTFO.Utilities;
using HarmonyLib;
using System.IO;

namespace MTFO.Patches
{
    [HarmonyPatch(typeof(BinaryEncoder), "Decode")]
    class Patch_BinaryDecoder
    {
        public static void Postfix(ref string __result)
        {
            //Ensure the file is game data related
            if (__result.Contains("Headers"))
            {
                int hash = __result.GetStableHashCode();
                ConfigManager.gameDataLookup.TryGetValue(hash, out string name);

                if (name != null)
                {
                    Log.Verbose("Found " + name);
                    try
                    {
                        if (!CustomDatablockManager.TryGetDatablock(name, out string text))
                        {
                            Log.Verbose($"Could not find custom Datablock: {name}");
                            File.WriteAllText(Path.Combine(ConfigManager.GameDataPath, name + ".json"), __result);
                        }

                        Log.Verbose($"Found custom Datablock: {name}");
                        __result = text;

                        return;

                        // string filePath = Path.Combine(ConfigManager.GameDataPath, name + ".json");

                        // if (File.Exists(filePath))
                        // {
                        //     Log.Verbose("Reading [" + name + "] from disk...");
                        //     __result = File.ReadAllText(filePath);
                        //     return;
                        // }
                        // else
                        // {
                        //     Log.Verbose("No file found at [" + filePath + "], writing file to disk...");
                        //     File.WriteAllText(filePath, __result);
                        // }
                    }
                    catch
                    {
                        Log.Error("Failed to write " + name + " to disk!!");
                    }
                }
                else
                {
                    string errorPath = Path.Combine(ConfigManager.GameDataPath, "UNKNOWN");
                    if (!Directory.Exists(errorPath))
                    {
                        Directory.CreateDirectory(errorPath);
                    }
                    string errorFilePath = Path.Combine(errorPath, hash + ".json");
                    Log.Error("Failed to find match for hash [" + hash + "]! Cannot load custom data for this block!");
                    if (File.Exists(errorFilePath))
                    {
                        Log.Warn("-- FILE FOUND IN DUMP FOLDER WITH MATCHING HASH FILE NAME, LOADING INSTEAD --");
                        __result = File.ReadAllText(errorFilePath);
                        return;
                    }

                    if (ConfigManager.DumpUnknownFiles)
                    {
                        Log.Debug("----- FILE CONTENT DUMP START -----");
                        Log.Debug(__result);
                        Log.Debug("----- FILE CONTENT DUMP END -----");
                    }
                    Log.Debug("DUMPING FILE CONTENTS TO [" + errorFilePath + "]");
                    File.WriteAllText(errorFilePath, __result);
                }
            }
        }
    }
}
