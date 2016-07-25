using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using RimWorld;

namespace RT_SolarFlareShield
{
	[StaticConstructorOnStartup]
	public static class Resources
	{
		public static Material rotatorTexture = MaterialPool.MatFrom("RT_Buildings/Building_RTMagneticShield_Top", ShaderDatabase.Cutout);
	}
}
