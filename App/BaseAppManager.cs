﻿using System.Collections.Generic;
using App;
using Lib.UnityQuickTools.Collections;
using Libs.GameFramework.DI;
using Libs.GameFramework.Interfaces;
using Mirror;
using UnityEngine;

namespace Libs.GameFramework
{
    public abstract class BaseAppManager : MonoBehaviour
    {
        private GenericMap<IAppModule> modules = new GenericMap<IAppModule>();
        private DependencyContainer dependencies = new DependencyContainer();

        public static BaseAppManager current { get; private set; }
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            current = this;
            dependencies.Register(this);
            RegisterDependencies();

            dependencies.InjectDependencies();
        }

        private void Start()
        {

            LoadApp();
        }

        public void InjectDependenciesTo(DependencyContainer container)
        {
            container.AddDependencies(dependencies);
        }

        private void LoadApp()
        {
            modules.Values.Foreach(item=> item.Init());
        }

        protected void Register<T>(T item)
        {
            if (item is IAppModule module)
            {
                modules.Set(module);
            }

            dependencies.Register(item);
        }

        protected abstract void RegisterDependencies();
    }
}