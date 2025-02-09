﻿using Common.Messaging;
using GameInterface.Services.Heroes;
using GameInterface.Services.MobileParties.Interfaces;
using GameInterface.Services.MobileParties.Messages;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Diamond;
using TaleWorlds.Library;

namespace GameInterface.Services.MobileParties.Patches
{
    [HarmonyPatch(typeof(MobileParty))]
    internal class PartyMovementPatch
    {
        private static MobileParty AllowedChangeParty;

        [HarmonyPrefix]
        [HarmonyPatch("TargetPosition")]
        [HarmonyPatch(MethodType.Setter)]
        private static void MovementPrefix(ref MobileParty __instance, ref Vec2 value)
        {
            if (AllowedChangeParty == __instance) 
            {
                return;
            }

            var message = new PartyTargetPositionChanged(__instance, value);
            MessageBroker.Instance.Publish(__instance, message);
        }

        internal static readonly PropertyInfo MobileParty_TargetPosition = typeof(MobileParty).GetProperty(nameof(MobileParty.TargetPosition));
        public static void SetTargetPositionOverride(MobileParty party, ref Vec2 position)
        {
            AllowedChangeParty = party;
            lock (AllowedChangeParty)
            {
                MobileParty_TargetPosition.SetValue(party, position);
            }
            AllowedChangeParty = null;
        }
    }
}
