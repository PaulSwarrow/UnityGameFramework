using System;
using System.Collections.Generic;
using Lib.UnityQuickTools;

namespace Libs.GameFramework.DI
{
    public class DependencyContainer
    {
        private Dictionary<Type, object> map = new Dictionary<Type, object>();

        public void Register<T>(T item)
        {
            map.Add(typeof(T), item);
        }

        public void AddDependencies(DependencyContainer container)
        {
            foreach (var pair in container.map)
            {
                if (!map.ContainsKey(pair.Key)) map[pair.Key] = pair.Value;
            }
        }

        public void InjectDependencies()
        {
            foreach (var item in map.Values)
            {
                var itemType = item.GetType();
                foreach (var field in ReflectionTools.GetFieldsWithAttributes(itemType, typeof(InjectAttribute)))
                {
                    if (map.TryGetValue(field.FieldType, out var value))
                    {
                        field.SetValue(item, value);
                    }
                }
            }
        }

        public void Dispose()
        {
            foreach (var item in map.Values)
            {
                var itemType = item.GetType();
                foreach (var field in ReflectionTools.GetFieldsWithAttributes(itemType, typeof(InjectAttribute)))
                {
                    field.SetValue(item, null);
                }
            }

            map.Clear();
            map = null;
        }
    }
}