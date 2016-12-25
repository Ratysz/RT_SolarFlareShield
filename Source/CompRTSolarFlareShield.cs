using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using RimWorld;

namespace RT_SolarFlareShield
{
	public class CompRTSolarFlareShield : ThingComp
	{
		private CompProperties_RTSolarFlareShield properties
		{
			get
			{
				return (CompProperties_RTSolarFlareShield)props;
			}
		}
		public float shieldingPowerDrain
		{
			get
			{
				return properties.shieldingPowerDrain;
			}
		}
		public float heatingPerTick
		{
			get
			{
				return properties.heatingPerTick;
			}
		}
		public float rotatorSpeedActive
		{
			get
			{
				return properties.rotatorSpeedActive;
			}
		}
		public float rotatorSpeedIdle
		{
			get
			{
				return properties.rotatorSpeedIdle;
			}
		}

		private float rotatorAngle = (float)Rand.Range(0, 360);

		public override void PostSpawnSetup()
		{
			base.PostSpawnSetup();

			IncidentDef incidentDef = DefDatabase<IncidentDef>.GetNamed("SolarFlare");
			MapConditionDef mapConditionDef = DefDatabase<MapConditionDef>.GetNamed("MapCondition_RTSolarFlare");
			if (incidentDef.mapCondition != mapConditionDef)
			{
				incidentDef.mapCondition = mapConditionDef;
				DefDatabase<IncidentDef>.ResolveAllReferences();
				Log.Message("RT_SolarFlareShield: replaced MapCondition for SolarFlare.");
			}
		}

		public override void PostDeSpawn(Map map)
		{
			base.PostDeSpawn(map);

			IncidentDef incidentDef = DefDatabase<IncidentDef>.GetNamed("SolarFlare");
			MapConditionDef mapConditionDef = DefDatabase<MapConditionDef>.GetNamed("SolarFlare");
			if (incidentDef.mapCondition != mapConditionDef)
			{
				incidentDef.mapCondition = mapConditionDef;
				DefDatabase<IncidentDef>.ResolveAllReferences();
				Log.Message("RT_SolarFlareShield: restored MapCondition for SolarFlare.");
			}

			MapCondition mapCondition = parent.Map.mapConditionManager.GetActiveCondition(
				DefDatabase<MapConditionDef>.GetNamed("MapCondition_RTSolarFlare"));
			if (mapCondition != null)
			{
				int ticksToExpire = mapCondition.TicksLeft;
				mapCondition.duration = mapCondition.TicksPassed - 1;
				parent.Map.mapConditionManager.RegisterCondition(MapConditionMaker.MakeCondition(
					MapConditionDefOf.SolarFlare, ticksToExpire));
			}
		}

		public override string CompInspectStringExtra()
		{
			return "CompRTSolarFlareShield_FlareProtection".Translate();
		}

		public override void CompTick()
		{
			SolarFlareShieldTick(1);
		}

		public override void PostDraw()
		{       // Thanks Skullywag!
			Vector3 vector = new Vector3(2.0f, 2.0f, 2.0f);
			vector.y = Altitudes.AltitudeFor(AltitudeLayer.VisEffects);
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.SetTRS(
				parent.DrawPos + Altitudes.AltIncVect,
				Quaternion.AngleAxis(rotatorAngle, Vector3.up),
				vector);
			Graphics.DrawMesh(MeshPool.plane10, matrix, Resources.rotatorTexture, 0);
		}

		private void SolarFlareShieldTick(int tickAmount)
		{
			if ((Find.TickManager.TicksGame) % tickAmount == 0)
			{
				MapCondition mapCondition = parent.Map.mapConditionManager.GetActiveCondition(
					DefDatabase<MapConditionDef>.GetNamed("MapCondition_RTSolarFlare"));
				CompPowerTrader compPowerTrader = parent.TryGetComp<CompPowerTrader>();
				if (compPowerTrader != null && compPowerTrader.PowerOn)
				{
					if (mapCondition != null)
					{
						compPowerTrader.PowerOutput = -shieldingPowerDrain;
						rotatorAngle += rotatorSpeedActive * tickAmount;
						Room room = parent.GetRoom();
						if (room != null && !room.UsesOutdoorTemperature)
						{
							room.Temperature += heatingPerTick;
						}
						List<Building_CommsConsole> commsConsoles = parent.Map.listerBuildings.AllBuildingsColonistOfClass<Building_CommsConsole>().ToList();
						foreach (Building_CommsConsole commsConsole in commsConsoles)
						{
							CompPowerTrader consoleCompPowerTrader = commsConsole.TryGetComp<CompPowerTrader>();
							if (consoleCompPowerTrader != null)
							{
								consoleCompPowerTrader.PowerOn = false;
							}
						}
					}
					else
					{
						compPowerTrader.PowerOutput = -parent.def
							.GetCompProperties<CompProperties_Power>().basePowerConsumption;
						rotatorAngle += rotatorSpeedIdle * tickAmount;
					}
				}
				else if (mapCondition != null)
				{
					int ticksToExpire = mapCondition.TicksLeft;
					mapCondition.duration = mapCondition.TicksPassed - 1;
					parent.Map.mapConditionManager.RegisterCondition(MapConditionMaker.MakeCondition(
						MapConditionDefOf.SolarFlare, ticksToExpire));
				}
			}
		}
	}
}
