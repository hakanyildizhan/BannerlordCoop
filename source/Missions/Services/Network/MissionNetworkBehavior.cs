﻿using Common;
using Common.Logging;
using Common.Messaging;
using Common.Network;
using Common.PacketHandlers;
using LiteNetLib;
using Missions.Messages;
using Missions.Services.Agents.Messages;
using Missions.Services.Agents.Packets;
using Missions.Services.Network.Messages;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;

namespace Missions.Services.Network
{
    public class CoopMissionNetworkBehavior : MissionBehavior
    {
        private static readonly ILogger Logger = LogManager.GetLogger<CoopMissionNetworkBehavior>();

        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        private readonly LiteNetP2PClient _client;
        private readonly Guid _playerId;

        private readonly TimeSpan WaitForConnectionsTime = TimeSpan.FromSeconds(1);

        private readonly INetworkMessageBroker _networkMessageBroker;
        private readonly INetworkAgentRegistry _agentRegistry;
        private readonly MovementHandler _movementHandler;
        private readonly EventPacketHandler _eventPacketHandler;

        public CoopMissionNetworkBehavior(
            LiteNetP2PClient client, 
            INetworkMessageBroker messageBroker,
            INetworkAgentRegistry agentRegistry,
            MovementHandler movementHandler,
            EventPacketHandler eventPacketHandler
            )
        {
            _client = client;
            _networkMessageBroker = messageBroker;
            _agentRegistry = agentRegistry;

            // TODO DI
            _movementHandler = movementHandler;
            _eventPacketHandler = eventPacketHandler;
            //_movementHandler = new MovementHandler(_client, _networkMessageBroker, _agentRegistry);
            //_eventPacketHandler = new EventPacketHandler(_networkMessageBroker, client.PacketManager);

            _networkMessageBroker.Subscribe<PeerConnected>(Handle_PeerConnected);
        }

        public override void AfterStart()
        {
            string sceneName = Mission.SceneName;
            _client.NatPunch(sceneName);

            //// TODO find way to make this not a task
            //Task.Factory.StartNew(async () =>
            //{
            //    while (Mission == null || Mission.IsLoadingFinished == false)
            //    {
            //        await Task.Delay(100);
            //    }

            //    string sceneName = Mission.SceneName;
            //    _client.NatPunch(sceneName);

            //    await Task.Delay(WaitForConnectionsTime);
            //});
        }

        private void Handle_PeerConnected(MessagePayload<PeerConnected> payload)
        {
            SendJoinInfo(payload.What.Peer);
        }

        private void SendJoinInfo(NetPeer peer)
        {
            CharacterObject characterObject = CharacterObject.PlayerCharacter;
            _agentRegistry.RegisterControlledAgent(_playerId, Agent.Main);

            List<Vec3> unitPositions = new List<Vec3>();
            List<string> unitIdStrings = new List<string>();
            List<Guid> guids = new List<Guid>();
            foreach (Agent agent in Agent.Main.Team.TeamAgents.Where(agent => agent != Agent.Main))
            {
                var newGuid = Guid.NewGuid();
                _agentRegistry.RegisterControlledAgent(newGuid, agent);
                guids.Add(newGuid);
                unitPositions.Add(agent.Position);
                unitIdStrings.Add(agent.Character.StringId);
            }

            Logger.Debug("Sending join request");

            NetworkMissionJoinInfo request = new NetworkMissionJoinInfo(characterObject, _playerId, Agent.Main.Position, guids.ToArray(), unitPositions.ToArray(), unitIdStrings.ToArray());
            _networkMessageBroker.PublishNetworkEvent(peer, request);
            Logger.Information("Sent {AgentType} Join Request for {AgentName}({PlayerID}) to {Peer}",
                characterObject.IsPlayerCharacter ? "Player" : "Agent",
                characterObject.Name, request.PlayerId, peer.EndPoint);
        }

        public override void OnRemoveBehavior()
        {
            base.OnRemoveBehavior();

            _agentRegistry.Clear();
            _client.Stop();
        }

        public override void OnAgentDeleted(Agent affectedAgent)
        {
            _networkMessageBroker.Publish(this, new AgentDeleted(affectedAgent));
            
            base.OnAgentDeleted(affectedAgent);
        }

        protected override void OnEndMission()
        {
            _client.Dispose();
            MBGameManager.EndGame();
            base.OnEndMission();
        }
    }
}
