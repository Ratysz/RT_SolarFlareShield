using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using RimWorld;

namespace RT_SolarFlareShield
{
	public class CompProperties_RTSolarFlareShield : CompProperties
	{
		public float shieldingPowerDrain = 0.0f;
		public float heatingPerTick = 0.0f;
		public float rotatorSpeedActive = 10.0f;
		public float rotatorSpeedIdle = 0.5f;

		public CompProperties_RTSolarFlareShield()
		{
			compClass = typeof(CompRTSolarFlareShield);
		}
	}
}
