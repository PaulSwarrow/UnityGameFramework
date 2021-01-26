using System;
using System.Collections.Generic;
using Lib.UnityQuickTools;

namespace Libs.GameFramework.DI
{
    public class DependencyContainer
    {
        private Dictionary<Type, object> map = new Dictionary<Type, object>();
        private HashSet<DependencyContainer> externalDependencies = new HashSet<DependencyContainer>();

        public void Register<T>(T item)
        {
            map.Add(typeof(T), item);
        }

        public void AddExternalDependencies(DependencyContainer container)
        {
            externalDependencies.Add(container);
        }

        public void InjectDependencies()
        {
            foreach (var item in map.Values)
            {
                var itemType = item.GetType();
                foreach (var field in ReflectionTools.GetFieldsWithAttributes(itemType, typeof(InjectAttribute)))
                {
                    if (GetObject(field.FieldType, out var value))
                    {
                        field.SetValue(item, value);
                    }
                }
            }
        }

        private bool GetObject(Type type, out object result)
        {
            if (map.TryGetValue(type, out result)) return true;
            foreach (var container in externalDependencies)
            {
                if (container.GetObject(type, out result)) return true;
            }

            return false;
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