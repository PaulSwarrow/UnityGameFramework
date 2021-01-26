using System.Collections.Generic;
using App;
using Lib.UnityQuickTools.Collections;
using Libs.GameFramework.DI;
using Libs.GameFramework.Interfaces;
using Mirror;
using UnityEngine;

namespace Libs.GameFramework
{
    public abstract class AppManager : MonoBehaviour
    {
        private GenericMap<IAppModule> modules = new GenericMap<IAppModule>();
        private DependencyContainer dependencies = new DependencyContainer();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }


        private void Start()
        {
            dependencies.Register(this);
            RegisterDependencies();

            dependencies.InjectDependencies();

            LoadApp();
        }

        private void LoadApp()
        {
            modules.Values.Foreach(item=> item.Init());
        }

        public void Register<T>(T item)
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