using System;
using HarmonyLib;
using StardewValley.Tools;

namespace Boomerang
{ 
    internal sealed class Patches
    {
        internal static bool getCategoryName_Prefix(MeleeWeapon __instance, ref string __result)
        {
            if (__instance.itemId.Value == Boomerang.Mod.itemID_c)
            {
                __result = "Boomerang";
                return false;
            }
            return true;
        }
    }
}