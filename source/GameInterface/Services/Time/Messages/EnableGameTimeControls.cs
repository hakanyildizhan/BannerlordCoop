﻿using Common.Messaging;
using System;

namespace GameInterface.Services.Time.Messages
{
    public readonly struct EnableGameTimeControls : ICommand
    {
        public Guid TransactionID => throw new NotImplementedException();
    }
}
