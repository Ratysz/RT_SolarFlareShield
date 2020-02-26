using Verse;

namespace RT_SolarFlareShield
{
	public class MapComponent_ShieldCoordinator : MapComponent
	{
		public MapComponent_ShieldCoordinator(Map map) : base(map)
		{
		}

		public bool hasAnyShield = false;
		public bool hasActiveShield = false;
	}
}