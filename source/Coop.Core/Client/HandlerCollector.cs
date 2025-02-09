﻿using Common.Extensions;
using Common.Messaging;
using System;
using System.Collections.Generic;

namespace Coop.Core.Client
{
    internal static class HandlerCollector
    {
        public static IEnumerable<Type> Collect<Module>()
        {
            string namespacePrefix = typeof(Module).Namespace;

            List<Type> types = new List<Type>();

            foreach(Type t in AppDomain.CurrentDomain.GetDomainTypes(namespacePrefix)) 
            {
                if(t.GetInterface(nameof(IHandler)) == null) continue;

                types.Add(t);
            }

            return types;
        }
    }
}
