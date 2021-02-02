using System;
using System.Collections;
using System.Collections.Generic;
using Lib.UnityQuickTools;
using Lib.UnityQuickTools.Collections;
using Libs.GameFramework.DI;
using Libs.GameFramework.Interfaces;
using UnityEngine;

namespace Libs.GameFramework
{
    public abstract class BaseGameManager : MonoBehaviour
    {
        public event Action ReadSceneEvent;
        public event Action StartEvent;
        public event Action UpdateEvent;
        public event Action FixedUpdateEvent;
        public event Action LateUpdateEvent;
        public event Action EndEvent;
        public event Action GizmosEvent;


        private HashSet<IGameSystem> systems = new HashSet<IGameSystem>();
        private DependencyContainer dependencies = new DependencyContainer();

        private void Awake()
        {
            StartCoroutine(Init());
        }

        private IEnumerator Init()
        {
            yield return new WaitUntil(BaseAppManager.IsReady);
            
            BaseAppManager.current.InjectDependenciesTo(dependencies);
            dependencies.Register(this);
            
            RegisterDependencies();
            dependencies.InjectDependencies();
            systems.Foreach(item => item.Init());
            
            
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

        protected void Register<T>(T item) where T : class
        {
            if (item is IGameSystem system) systems.Add(system);
            dependencies.Register(item);
        }
    }
}