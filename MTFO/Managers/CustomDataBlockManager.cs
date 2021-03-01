using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;
using MTFO.Utilities;

namespace MTFO.Managers
{
    public static class CustomDatablockManager
    {
        public static bool IsSetup { get; private set; } = false;

#nullable enable
        public static ReadOnlyDictionary<string, string>? Datablocks { get; private set; }

        public static ReadOnlyDictionary<string, int>? HashedDatablocks { get; private set; }

        public static ReadOnlyDictionary<int, string>? DatablockLookup { get; private set; }

        public static int? DatablocksHash { get; private set; }
#nullable disable


        public static void Setup()
        {
            if (IsSetup) return;

            Log.Message("Setting up CustomDatablockManager...");

            ReadDataBlocks();

            HashDatablocks(Datablocks);

            // Might just be able to use Datablocks.Values.GetHashCode();
            DatablocksHash = string.Join("", Datablocks.Values).GetStableHashCode();

            Log.Message("Computed Datablock Hash: " + DatablocksHash);

            IsSetup = true;
        }

        public static bool TryLookupDatablock(int hashCode, out string text)
        {
            Log.Message("Looking up Datablock with Hash: " + hashCode);

            text = null;

            if (!IsSetup || !DatablockLookup.TryGetValue(hashCode, out string name)) return false;

            return Datablocks.TryGetValue(name, out text);
        }

        public static bool TryGetDatablock(string name, out string text)
        {
            Log.Message("Looking up Datablock:" + name);

            text = null;

            if (!IsSetup) return false;

            return Datablocks.TryGetValue(name, out text);
        }

        public static void HashDatablocks(IDictionary<string, string> datablocks)
        {
            var hashedDatablocks = new Dictionary<string, int>();
            var datablockLookup = new Dictionary<int, string>();

            foreach (var pair in datablocks)
            {
                var hash = pair.Value.GetStableHashCode();

                Log.Message($"Hashed Datablock: {pair.Key} with Hash: {hash}");

                hashedDatablocks.Add(pair.Key, hash);
                datablockLookup.Add(hash, pair.Key);
            }
            HashedDatablocks = new ReadOnlyDictionary<string, int>(hashedDatablocks);
        }

        public static void ReadDataBlocks()
        {
            var customDatablocks = new Dictionary<string, string>();

            if (!Directory.Exists(ConfigManager.GameDataPath))
            {
                Datablocks = new ReadOnlyDictionary<string, string>(customDatablocks);
                return;
            }

            var filePaths = Directory.GetFiles(ConfigManager.GameDataPath, "*.json");

            Log.Message($"Found {filePaths.Length} datablock(s)");

            foreach (string filePath in filePaths)
            {
                if (!FileUtil.TryReadAllText(filePath, out string text)) continue;

                Log.Message($"Found datablock: {Path.GetFileName(filePath)} {Path.GetFileNameWithoutExtension(filePath)}");

                customDatablocks.Add(Path.GetFileNameWithoutExtension(filePath), text);
            }
            Datablocks = new ReadOnlyDictionary<string, string>(customDatablocks);
        }

        // TODO: Make this not cringe
        // public static string CleanDatablockFileName(string fileName)
        // {
        //     // return fileName.Substring(9, fileName.Length - 18);
        //     return fileName[9..^9]; // 2.1 WOOOOOOOOOOOO
        // }

    }
}
