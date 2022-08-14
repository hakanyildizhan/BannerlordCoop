﻿using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameInterface.Serialization.DynamicModel
{
    public class DynamicModelGenerator : IDynamicModelGenerator
    {

        /// <summary>
        /// Creates a dynamic serialization model for protobuf of the provided type
        /// </summary>
        /// <typeparam name="T">Type to create model</typeparam>
        /// <param name="exclude">Excluded fields by type</param>
        public void CreateDynamicSerializer<T>(IEnumerable<Type> exclude = null)
        {
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (exclude != null)
            {
                HashSet<Type> excludesSet = exclude.ToHashSet();

                fields = fields.Where(f => excludesSet.Contains(f.FieldType) == false).ToArray();
            }

            string[] fieldNames = fields.Select(f => f.Name).ToArray();
            RuntimeTypeModel.Default.Add(typeof(T), true).Add(fieldNames);
        }

        /// <summary>
        /// Creates a dynamic serialization model for protobuf of the provided type
        /// </summary>
        /// <typeparam name="T">Type to create model</typeparam>
        /// <param name="exclude">Excluded fields by name</param>
        public void CreateDynamicSerializer<T>(IEnumerable<string> exclude = null)
        {
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (exclude != null)
            {
                HashSet<string> excludesSet = exclude.ToHashSet();

                fields = fields.Where(f => excludesSet.Contains(f.Name) == false).ToArray();
            }

            string[] fieldNames = fields.Select(f => f.Name).ToArray();
            RuntimeTypeModel.Default.Add(typeof(T), true).Add(fieldNames);
        }

        public void AssignSurrogate<TClass, TSurrogate>()
        {
            RuntimeTypeModel.Default.Add(typeof(TClass), false).SetSurrogate(typeof(TSurrogate));
        }

        public void Compile()
        {
            RuntimeTypeModel.Default.CompileInPlace();
        }
    }
}
