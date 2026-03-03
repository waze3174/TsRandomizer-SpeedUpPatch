using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;

namespace TsRandomizer
{
	// Increases the shop purchase limit to match the configured stack cap.
	// This is the only Harmony patch in the mod — it targets GetAvailableQuantityByItem
	// in ShopMenuScreen which has a hardcoded 9 that cannot be overridden without IL patching.
	static class ShopCapHarmonyPatch
	{
		static readonly Harmony HarmonyInstance = new Harmony("tsrandomizer.shopcap");

		public static void Apply()
		{
			HarmonyInstance.PatchAll(typeof(ShopCapHarmonyPatch).Assembly);
		}

		public static void Unapply()
		{
			HarmonyInstance.UnpatchAll("tsrandomizer.shopcap");
		}
	}

	[HarmonyPatch]
	class ShopCapPatch
	{
		static MethodBase TargetMethod()
		{
			var type = AccessTools.TypeByName("Timespinner.GameStateManagement.Screens.Shop.ShopMenuScreen");
			return AccessTools.Method(type, "GetAvailableQuantityByItem");
		}

		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = new List<CodeInstruction>(instructions);

			for (int i = 0; i < codes.Count; i++)
			{
				if (codes[i].LoadsConstant(9))
					codes[i].operand = QoLSettings.Current.StackCap;
			}

			return codes;
		}
	}
}
