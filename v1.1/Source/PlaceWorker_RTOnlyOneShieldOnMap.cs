using Verse;

namespace RT_SolarFlareShield
{
	public class PlaceWorker_RTOnlyOneShieldOnMap : PlaceWorker
	{
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
		{
			if (map.GetShieldCoordinator().hasAnyShield)
			{
				return "PlaceWorker_RTOnlyOneShieldOnMap".Translate();
			}
			return true;
		}
	}
}