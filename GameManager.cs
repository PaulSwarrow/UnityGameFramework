using System;
using System.Collections.Generic;
using Lib.UnityQuickTools.Collections;
using NUnit.Framework;
using UnityEngine;

namespace DefaultNamespace
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
        public static GameManager instance { get; private set; }


        private HashSet<GameSystem> systems = new HashSet<GameSystem>();
        private Dictionary<Type, object> map = new Dictionary<Type, object>();

        private void Awake()
        {
            Assert.IsNull(instance);
            instance = this;
            RegisterDependencies();
            systems.Foreach(item => item.InjectDependencies());
            systems.Foreach(item => item.Init());
        }

        private void Start()
        {
            ReadSceneEvent?.Invoke();
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
            systems.Foreach(system=> system.Stop());
            EndEvent?.Invoke();
            instance = null;
        }

        private void OnDrawGizmos()
        {
            if(!Application.isPlaying) return;
            GizmosEvent?.Invoke();
        }

        protected abstract void RegisterDependencies();

        protected void Register<T>(T item)
        {
            if (item is GameSystem system) systems.Add(system);
            map.Add(typeof(T), item);
        }

        internal object GetObject(Type type) => map[type];
    }
}