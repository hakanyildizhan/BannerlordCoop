﻿using HarmonyLib;
using TaleWorlds.CampaignSystem.Party;

namespace GameInterface.Services.MobileParties.Patches
{
    /// <summary>
    ///     Patch that alter <see cref="CampaignSystem.MobileParty.ComputeIsWaiting"/> method so that the player's party is never waiting.
    ///     <para> For more information see <seealso href="https://github.com/Bannerlord-Coop-Team/BannerlordCoop/issues/133">issue #133</seealso></para>
    /// </summary>
    [HarmonyPatch(typeof(MobileParty), nameof(MobileParty.ComputeIsWaiting))]
    class ComputeIsWaitingPatch
    {

        [HarmonyPrefix]
        static bool Prefix(MobileParty __instance, ref bool __result)
        {
            if (__instance.IsMainParty)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
