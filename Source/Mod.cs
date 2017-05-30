using Harmony;
using Verse;
using UnityEngine;
using System.Reflection;

namespace RT_SolarFlareShield
{
	class Mod : Verse.Mod
	{
		public Mod(ModContentPack content) : base(content)
		{
			var harmony = HarmonyInstance.Create("io.github.ratysz.rt_solarflareshield");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
	}

	[StaticConstructorOnStartup]
	public static class Resources
	{
		public static Material rotatorTexture
			= MaterialPool.MatFrom("RT_Buildings/Building_RTMagneticShield_Top", ShaderDatabase.Cutout);
	}
}
