using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.GameData.HomeRenovations;
using StardewValley.Tools;

namespace Boomerang
{ 
    internal sealed class Patches
    {
        internal static bool getCategoryName_Prefix(MeleeWeapon __instance, ref string __result)
        {
            if (__instance.itemId.Value == Boomerang.ModEntry.itemID_c)
            {
                __result = "Boomerang";
                return false;
            }
            return true;
        }
        
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
        {
            bool foundSourceRect = false;
            var locItemID = Boomerang.ModEntry.itemID_c;
            var funcGetSourceRect = AccessTools.Method(typeof(Rectangle), "GetSourceRect");
            var codes = new List<CodeInstruction>(instructions);
            var exitPatchLabel = gen.DefineLabel();
            for (var i = 0; i < codes.Count; i++)
            {
                yield return codes[i];
                if (i > 0 && codes[i-1].opcode == OpCodes.Callvirt &&
                    codes[i-1].operand == (object) funcGetSourceRect &&
                    codes[i].opcode == OpCodes.Stloc_1)
                {
                    if (!foundSourceRect)
                    {
                        codes[i + 1].labels.Add(exitPatchLabel);
                        yield return new CodeInstruction(OpCodes.Ldarg_S, "weaponItemId");
                        yield return new CodeInstruction(OpCodes.Ldstr, locItemID);
                        yield return new CodeInstruction(OpCodes.Bne_Un_S, exitPatchLabel);
                        yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Rectangle), nameof(Rectangle.Empty)));
                        yield return new CodeInstruction(OpCodes.Stloc_1);
                        foundSourceRect = true;
                    }
                }
            }
        }
    }
}