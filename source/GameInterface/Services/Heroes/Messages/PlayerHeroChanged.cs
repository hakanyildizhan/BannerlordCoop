﻿using TaleWorlds.CampaignSystem;

namespace GameInterface.Services.Heroes.Messages
{
    public readonly struct PlayerHeroChanged
    {
        public Hero PreviousHero { get; }
        public Hero NewHero { get; }

        public PlayerHeroChanged(Hero previousHero, Hero newHero)
        {
            PreviousHero = previousHero;
            NewHero = newHero;
        }
    }
}
