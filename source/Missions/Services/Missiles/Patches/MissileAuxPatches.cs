﻿using Common.Messaging;
using Common.Network;
using HarmonyLib;
using JetBrains.Annotations;
using Missions.Services.Missiles.Message;
using Missions.Services.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Missions.Services.Missiles.Patches
{
    [HarmonyPatch(typeof(Mission), "AddMissileAux")]
    public class AddMissileAuxPatch
    {
        private static void Prefix(Agent shooterAgent, ref MissionWeapon __state)
        {
            __state = shooterAgent.WieldedWeapon;
        }
        private static void Postfix(int __result, Agent shooterAgent, ref Vec3 direction, ref Vec3 position, ref Mat3 orientation, bool addRigidBody, int forcedMissileIndex, ref MissionWeapon __state)
        {
            if (NetworkAgentRegistry.Instance.IsControlled(shooterAgent))
            {
                AgentShoot message = new AgentShoot(shooterAgent, __state, position, direction, orientation, addRigidBody, __result);
                NetworkMessageBroker.Instance.Publish(shooterAgent, message);
            }
        }
    }

    [HarmonyPatch(typeof(Mission), "AddMissileSingleUsageAux")]
    public class AddMissileSingleUsageAuxPatch
    {
        private static void Prefix(Agent shooterAgent, ref MissionWeapon __state)
        {
            __state = shooterAgent.WieldedWeapon;
        }
        private static void Postfix(int __result, Agent shooterAgent, ref Vec3 direction, ref Vec3 position, ref Mat3 orientation, bool addRigidBody, int forcedMissileIndex, ref MissionWeapon __state)
        {
            if (NetworkAgentRegistry.Instance.IsControlled(shooterAgent))
            {
                AgentShoot message = new AgentShoot(shooterAgent, __state, position, direction, orientation, addRigidBody, __result);
                NetworkMessageBroker.Instance.Publish(shooterAgent, message);
            }
        }
    }

    [HarmonyPatch(typeof(Mission), "OnAgentShootMissile")]
    public static class BlockMissileIfNative
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool OnAgentShootMissile(
            ref Agent shooterAgent,
            ref EquipmentIndex weaponIndex,
            ref Vec3 position,
            ref Vec3 velocity,
            ref Mat3 orientation,
            ref bool hasRigidBody,
            ref bool isPrimaryWeaponShot,
            ref int forcedMissileIndex)
        {
            if (!NetworkAgentRegistry.Instance.IsControlled(shooterAgent) && forcedMissileIndex == -1)
            {
                return false;
            }

            return true;

        }
    }
}
