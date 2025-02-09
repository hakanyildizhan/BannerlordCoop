﻿using Common;
using Common.Logging;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.ObjectSystem;

namespace GameInterface.Services.Registry
{
    internal class RegistryBase<T> : IRegistry<T> where T : MBObjectBase
    {
        protected readonly ILogger Logger = LogManager.GetLogger<RegistryBase<T>>();

        protected readonly Dictionary<string, T> objIds = new Dictionary<string, T>();

        public int Count => objIds.Count;

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator() => objIds.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => objIds.GetEnumerator();

        public bool RegisterExistingObject(string id, T obj)
        {
            if (objIds.ContainsKey(id)) return false;

            objIds.Add(id, obj);

            return true;
        }

        public bool RegisterNewObject(T obj)
        {
            if (objIds.ContainsKey(obj.StringId)) return false;

            return true;
        }

        public bool Remove(T obj) => objIds.Remove(obj.StringId);

        public bool Remove(string id) => objIds.Remove(id);

        public bool TryGetValue(T obj, out string id)
        {
            id = null;
            if (objIds.ContainsKey(obj.StringId) == false) return false;

            id = obj.StringId;
            return true;
        }

        public bool TryGetValue(string id, out T obj) => objIds.TryGetValue(id, out obj);
    }
}
