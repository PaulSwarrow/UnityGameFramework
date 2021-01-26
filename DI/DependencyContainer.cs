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

        public void InjectDependencies()
        {
            foreach (var item in map.Values)
            {
                var itemType = item.GetType();
                foreach (var field in ReflectionTools.GetFieldsWithAttributes(itemType, typeof(InjectAttribute)))
                {
                    var propertyType = field.FieldType;
                    field.SetValue(item, GetObject(propertyType));
                }
            }
        }

        public object GetObject(Type type)
        {
            if (map.TryGetValue(type, out var item)) return item;
            return null;
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