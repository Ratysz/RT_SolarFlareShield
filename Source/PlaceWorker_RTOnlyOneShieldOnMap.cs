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
		public static bool shieldWasPlaced = false;

		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot)
		{
			if (!shieldWasPlaced)
			{
				foreach (Building building in Find.ListerBuildings.allBuildingsColonist)
				{
					if (building.TryGetComp<CompRTSolarFlareShield>() != null)
					{
						shieldWasPlaced = true;
						return "PlaceWorker_RTOnlyOneShieldOnMap".Translate();
					}
				}
			}
			else
			{
				return "PlaceWorker_RTOnlyOneShieldOnMap".Translate();
			}
			return true;
		}
	}
}
