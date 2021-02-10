using System;
using System.Collections.Generic;
using Lib.UnityQuickTools;
using UnityEngine.Assertions;

namespace Libs.GameFramework.DI
{
    public class DependencyContainer
    {
        private Dictionary<Type, object> map = new Dictionary<Type, object>();

        public void Register<T>(T item) where T : class
        {
            var type = typeof(T);
            Assert.IsNotNull(item, $"Register null for {type} type");
            map.Add(type, item);
        }

        public void Register(IEnumerable<object> items)
        {
            foreach (var item in items)
            {
                map.Add(item.GetType(), item);
            }
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

                foreach (var property in ReflectionTools.GetPropsWithAttributes(itemType, typeof(InjectAttribute)))
                {
                    if (map.TryGetValue(property.PropertyType, out var value))
                    {
                        property.SetValue(item, value);
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

                foreach (var property in ReflectionTools.GetPropsWithAttributes(itemType, typeof(InjectAttribute)))
                {
                    if (map.TryGetValue(property.PropertyType, out var value))
                    {
                        property.SetValue(item, null);
                    }
                }
            }

            map.Clear();
            map = null;
        }
    }
}