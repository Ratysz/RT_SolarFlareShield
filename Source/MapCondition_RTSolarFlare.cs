using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using RimWorld;

namespace RT_SolarFlareShield
{
	class MapCondition_RTSolarFlare : MapCondition
	{
		private bool initialized = false;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue(ref initialized, "initialized", false);
		}

		public override void Init()
		{
			foreach (Building building in Map.listerBuildings.AllBuildingsColonistOfDef(DefDatabase<ThingDef>.GetNamed("Building_RTMagneticShield")))
			{
				CompPowerTrader powerTrader = building.TryGetComp<CompPowerTrader>();
				if (null != powerTrader && powerTrader.PowerOn)
				{
					initialized = true;
					break;
				}
			}
			if (!initialized)
			{
				End();
			}
		}

		public override void End()
		{
			if (TicksLeft > 0)
			{
				if (initialized)
				{
					base.End();
				}
				else
				{
					mapConditionManager.ActiveConditions.Remove(this);
				}
				mapConditionManager.RegisterCondition(MapConditionMaker.MakeCondition(MapConditionDefOf.SolarFlare, TicksLeft));
			}
			else
			{
				Messages.Message(MapConditionDefOf.SolarFlare.endMessage, MessageSound.Standard);
				mapConditionManager.ActiveConditions.Remove(this);
			}
		}

		public override void MapConditionTick()
		{
			if (0 == Find.TickManager.TicksGame % 10)
			{
				bool workingShield = false;
				foreach (Building building in Map.listerBuildings.allBuildingsColonist)
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
							workingShield = true;
						}
					}
				}
				if (!workingShield)
				{
					End();
				}
			}
		}
	}
}
