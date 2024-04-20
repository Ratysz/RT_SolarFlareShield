using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace RT_SolarFlareShield
{
	[HarmonyPatch(typeof(PowerNet))]
	[HarmonyPatch(nameof(PowerNet.PowerNetTick))]
	internal class Patch_PowerNetTick
    {
        public static IEnumerable<CodeInstruction> Transpiler(
			MethodBase original,
			IEnumerable<CodeInstruction> instructions)
		{
			return instructions.MethodReplacer(
				AccessTools.Method(typeof(GameConditionManager), nameof(GameConditionManager.ElectricityDisabled)),
				AccessTools.Method(typeof(Patch_PowerNetTick), nameof(Patch_PowerNetTick.ElectricityDisabled)));
		}

        public bool ElectricityDisabled(Map map)
        {
            foreach (GameCondition activeCondition in map.gameConditionManager.ActiveConditions)
            {
                if (activeCondition.ElectricityDisabled && !activeCondition.HiddenByOtherCondition(map) && !map.GetShieldCoordinator().HasActiveShield())
                {
                    return true;
                }
            }
            return Find.World.GameConditionManager.Parent?.ElectricityDisabled(map) ?? false;
        }
    }
}