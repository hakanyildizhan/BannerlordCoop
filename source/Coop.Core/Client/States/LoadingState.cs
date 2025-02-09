﻿using Common.Messaging;
using Coop.Core.Client.Messages;
using GameInterface.Services.GameState.Messages;
using System;

namespace Coop.Core.Client.States
{
    /// <summary>
    /// State Logic Controller for the Loading Client State
    /// </summary>
    public class LoadingState : ClientStateBase
    {
        public LoadingState(IClientLogic logic) : base(logic)
        {
            Logic.NetworkMessageBroker.Subscribe<CampaignLoaded>(Handle);
            Logic.NetworkMessageBroker.Subscribe<MainMenuEntered>(Handle);
        }

        public override void Dispose()
        {
            Logic.NetworkMessageBroker.Unsubscribe<CampaignLoaded>(Handle);
            Logic.NetworkMessageBroker.Unsubscribe<MainMenuEntered>(Handle);
        }

        public override void EnterMainMenu()
        {
            Logic.NetworkMessageBroker.Publish(this, new EnterMainMenu());
        }

        private void Handle(MessagePayload<MainMenuEntered> obj)
        {
            Logic.State = new MainMenuState(Logic);
        }

        private void Handle(MessagePayload<CampaignLoaded> obj)
        {
            Logic.EnterCampaignState();
        }

        public override void Connect()
        {
        }

        public override void Disconnect()
        {
            Logic.NetworkMessageBroker.Publish(this, new EnterMainMenu());
        }

        public override void ExitGame()
        {
        }

        public override void LoadSavedData()
        {
        }

        public override void StartCharacterCreation()
        {
        }

        public override void EnterCampaignState()
        {
            Logic.State = new CampaignState(Logic);
        }

        public override void EnterMissionState()
        {
        }

        public override void ValidateModules()
        {
        }
    }
}
