using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Harmony;
using Verse;
using UnityEngine;

namespace RT_SolarFlareShield
{
	[HarmonyPatch(typeof(PowerNet))]
	[HarmonyPatch("PowerNetTick")]
	static class Patch_PowerNetTick
	{
		public static IEnumerable<CodeInstruction> Transpiler(
			MethodBase original,
			IEnumerable<CodeInstruction> instructions)
		{
			return instructions.MethodReplacer(
				AccessTools.Method(typeof(GameConditionManager), "ConditionIsActive"),
				AccessTools.Method(typeof(Patch_PowerNetTick), "ConditionIsActive"));
		}

		public static bool ConditionIsActive(this GameConditionManager instance, GameConditionDef def)
		{
			if (instance.ConditionIsActive(GameConditionDefOf.SolarFlare))
			{
				Log.Message("Blocking " + def.ToString() + "!");
				return !ProcessMap(instance.map);
			}
			cache.Clear();
			return false;
		}

		private static Dictionary<Map, PerishableBool> cache = new Dictionary<Map, PerishableBool>();
		private class PerishableBool
		{
			public bool value;
			public int age;
			public PerishableBool(bool value, int age)
			{
				this.value = value;
				this.age = age;
			}
		}

		private static bool ProcessMap(Map map)
		{
			PerishableBool pbool;
			if (cache.TryGetValue(map, out pbool))
			{
				if (pbool.age >= 10)
				{
					cache.Remove(map);
				}
				else
				{
					pbool.age++;
					return pbool.value;
				}
			}

			bool shieldFound = false;
			foreach (Building building in map.listerBuildings.allBuildingsColonist)
			{
				if (typeof(Building_CommsConsole).IsAssignableFrom(building.def.thingClass))
				{
					CompPowerTrader consoleCompPowerTrader = building.TryGetComp<CompPowerTrader>();
					if (consoleCompPowerTrader != null)
					{
						consoleCompPowerTrader.PowerOn = false;
					}
				}
				else if (building.def == DefDatabase<ThingDef>.GetNamed("Building_RTMagneticShield"))
				{
					CompPowerTrader powerTrader = building.TryGetComp<CompPowerTrader>();
					if (null != powerTrader && powerTrader.PowerOn)
					{
						shieldFound = true;
					}
				}
			}
			cache.Add(map, new PerishableBool(shieldFound, 0));
			return shieldFound;
		}
	}
}
