using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using RimWorld;

namespace RT_SolarFlareShield
{
	public class PlaceWorker_RTOnlyOneShieldOnMap : PlaceWorker
	{
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Thing thingToIgnore = null)
		{
			foreach (Building building in Map.listerBuildings.allBuildingsColonist)
			{
				if (building.TryGetComp<CompRTSolarFlareShield>() != null)
				{
					return "PlaceWorker_RTOnlyOneShieldOnMap".Translate();
				}
			}
			return true;
		}
	}
}
