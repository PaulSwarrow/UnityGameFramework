using System;
using System.Linq;
using System.Reflection;
using Lib.UnityQuickTools;

namespace DefaultNamespace
{
    public abstract class GameSystem
    {
        
        internal void InjectDependencies()
        {
            foreach (var field in ReflectionTools.GetFieldsWithAttributes(GetType(),  typeof(InjectAttribute)))
            {
                var propertyType = field.FieldType;
                field.SetValue(this, GameManager.instance.GetObject(propertyType));
            }
            
        }

        public virtual void Init() {}
        public abstract void Start();
        public abstract void Stop();
        
    }
}