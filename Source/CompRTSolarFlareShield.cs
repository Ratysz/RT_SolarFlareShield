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
		
		private CompPowerTrader compPowerTrader;
		private float rotatorAngle = (float)Rand.Range(0, 360);
		private int timerTicks = 300;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			compPowerTrader = parent.TryGetComp<CompPowerTrader>();
			if (compPowerTrader == null)
			{
				Log.Error("[RT Solar Flare Shield]: Could not get CompPowerTrader of " + parent);
			}
			timerTicks = 300;
		}

		public override string CompInspectStringExtra()
		{
			return "CompRTSolarFlareShield_FlareProtection".Translate();
		}

		public override void CompTick()
		{
			if (timerTicks > 0)
			{
				timerTicks--;
			}
			else if (timerTicks == 0)
			{
				timerTicks = -10;
				IncidentParms incidentParms =
					StorytellerUtility.DefaultParmsNow(
						Find.Storyteller.def,
						IncidentCategory.ThreatSmall,
						Find.World);
				IncidentDefOf.SolarFlare.Worker.TryExecute(incidentParms);
			}
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
				if (compPowerTrader != null && compPowerTrader.PowerOn)
				{
					GameCondition gameCondition =
						Find.World.GameConditionManager.GetActiveCondition(GameConditionDefOf.SolarFlare);
					if (gameCondition != null)
					{
						compPowerTrader.PowerOutput = -shieldingPowerDrain;
						rotatorAngle += rotatorSpeedActive * tickAmount;
						RoomGroup roomGroup = parent.GetRoomGroup();
						if (roomGroup != null && !roomGroup.UsesOutdoorTemperature)
						{
							roomGroup.Temperature += heatingPerTick * tickAmount;
						}
					}
					else
					{
						compPowerTrader.PowerOutput = -compPowerTrader.Props.basePowerConsumption;
						rotatorAngle += rotatorSpeedIdle * tickAmount;
					}
				}
			}
		}
	}
}
