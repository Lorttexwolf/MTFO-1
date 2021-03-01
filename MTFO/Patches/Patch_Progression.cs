using HarmonyLib;
using MTFO.Managers;
using MTFO.Utilities;
using ChainedPuzzles;
using UnityEngine;
using Newtonsoft.Json;

namespace MTFO.Patches
{
    [HarmonyPatch(typeof(DropServerManager), nameof(DropServerManager.GetRundownProgressionAsync))]
    internal static class Patch_RundownProgression
    {

        internal static void Prefix()
        {



        }

    }
}
