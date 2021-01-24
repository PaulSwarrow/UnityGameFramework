using System;
using System.Collections.Generic;
using Lib.UnityQuickTools;
using Lib.UnityQuickTools.Collections;
using Libs.GameFramework.Interfaces;
using UnityEngine;

namespace Libs.GameFramework
{
    public abstract class GameManager : MonoBehaviour
    {
        public static event Action ReadSceneEvent;
        public static event Action StartEvent;
        public static event Action UpdateEvent;
        public static event Action FixedUpdateEvent;
        public static event Action LateUpdateEvent;
        public static event Action EndEvent;
        public static event Action GizmosEvent;


        private HashSet<IGameSystem> systems = new HashSet<IGameSystem>();
        private Dictionary<Type, object> map = new Dictionary<Type, object>();

        private void Awake()
        {
            Register(this);
            RegisterDependencies();
            InjectDependencies();    
            systems.Foreach(item => item.Init());
        }

        private void Start()
        {
            ReadSceneEvent?.Invoke();
            systems.Foreach(item => item.Subscribe());
            systems.Foreach(item => item.Start());
        }

        private void Update()
        {
            UpdateEvent?.Invoke();
        }

        private void FixedUpdate()
        {
            FixedUpdateEvent?.Invoke();
        }

        private void LateUpdate()
        {
            LateUpdateEvent?.Invoke();
        }

        private void OnDestroy()
        {
            systems.Foreach(item => item.Stop());
            systems.Foreach(system => system.Unsubscribe());
            EndEvent?.Invoke();
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            GizmosEvent?.Invoke();
        }

        protected abstract void RegisterDependencies();

        protected void Register<T>(T item)
        {
            if (item is IGameSystem system) systems.Add(system);
            map.Add(typeof(T), item);
        }

        private void InjectDependencies()
        {
            foreach (var item in map.Values)
            {
                var itemType = item.GetType();
                foreach (var field in ReflectionTools.GetFieldsWithAttributes(itemType,  typeof(InjectAttribute)))
                {
                    var propertyType = field.FieldType;
                    field.SetValue(item, GetObject(propertyType));
                }
                
            }
        }

        private object GetObject(Type type)
        {
            if (map.TryGetValue(type, out var item)) return item;
            Debug.LogError("Type was not injected: " + type);
            return null;
        }
    }
}