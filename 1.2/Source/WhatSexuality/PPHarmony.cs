using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

namespace PansexualPandemonium
{
    [StaticConstructorOnStartup]
    public class PPHarmony
    {
        static PPHarmony()
        {
            Harmony WhatSexHarmony = new HarmonyLib.Harmony("com.whatsexuality.rimworld.mod");

            WhatSexHarmony.PatchAll(Assembly.GetExecutingAssembly());
        }
	}

	[HarmonyPatch(typeof(TraitSet), "GainTrait")]
	public static class Patch_GainTrait_Prefix
	{
		[HarmonyPrefix]
		public static bool Prefix(TraitSet __instance, Trait trait)
		{
			Pawn pawn = (Pawn)AccessTools.DeclaredField(typeof(TraitSet), "pawn").GetValue(__instance);

			if (trait.def == TraitDefOf.Bisexual || trait.def == TraitDefOf.Gay)
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Pawn_RelationsTracker), "SecondaryLovinChanceFactor")]
    public class Patch_PawnRelationsTracker_SecondaryLovinChanceFactor
	{
        [HarmonyPrefix]
        public static bool Prefix(Pawn_RelationsTracker __instance, Pawn otherPawn,ref float __result)
		{
			if (!__instance.pawn.RaceProps.Humanlike || !otherPawn.RaceProps.Humanlike || __instance.pawn == otherPawn || __instance.pawn.story.traits.allTraits.Any(t => t.def.defName == "Xenophobia" && t.degree == -1))
			{
				__result = 0f;
				return false;
			}
			if (__instance.pawn.story != null && __instance.pawn.story.traits != null)
			{
				if (__instance.pawn.story.traits.HasTrait(TraitDefOf.Asexual))
				{
					__result = 0f;
					return false;
				}
			}
			float ageBiologicalYearsFloat = __instance.pawn.ageTracker.AgeBiologicalYearsFloat;
			float ageBiologicalYearsFloat2 = otherPawn.ageTracker.AgeBiologicalYearsFloat;
			if (ageBiologicalYearsFloat < 16f || ageBiologicalYearsFloat2 < 16f)
			{
				__result = 0f;
				return false;
			}
			float num = 1f;
			if (__instance.pawn.gender == Gender.Male)
			{
				float min = ageBiologicalYearsFloat - 30f;
				float lower = ageBiologicalYearsFloat - 10f;
				float upper = ageBiologicalYearsFloat + 3f;
				float max = ageBiologicalYearsFloat + 10f;
				num = GenMath.FlatHill(0.2f, min, lower, upper, max, 0.2f, ageBiologicalYearsFloat2);
			}
			else if (__instance.pawn.gender == Gender.Female)
			{
				float min2 = ageBiologicalYearsFloat - 10f;
				float lower2 = ageBiologicalYearsFloat - 3f;
				float upper2 = ageBiologicalYearsFloat + 10f;
				float max2 = ageBiologicalYearsFloat + 30f;
				num = GenMath.FlatHill(0.2f, min2, lower2, upper2, max2, 0.2f, ageBiologicalYearsFloat2);
			}
			float num2 = Mathf.InverseLerp(16f, 18f, ageBiologicalYearsFloat);
			float num3 = Mathf.InverseLerp(16f, 18f, ageBiologicalYearsFloat2);
			float num4 = 0f;
			if (otherPawn.RaceProps.Humanlike)
			{
				num4 = otherPawn.GetStatValue(StatDefOf.PawnBeauty, true);
			}
			float num5 = 1f;
			if (num4 < 0f)
			{
				num5 = 0.3f;
			}
			else if (num4 > 0f)
			{
				num5 = 2.3f;
			}
			if(otherPawn.story.traits.allTraits.Any(t => t.def.defName == "Xenophobia" && t.degree == 1))
            {
				num5 = 2.3f;
            }
			__result = num * num2 * num3 * num5;

			return false;
		}
    }
}
