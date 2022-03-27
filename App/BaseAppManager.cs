using Lib.UnityQuickTools.Collections;
using Libs.GameFramework.DI;
using Libs.GameFramework.Interfaces;
using UnityEngine;

namespace Libs.GameFramework
{
    public abstract class BaseAppManager : MonoBehaviour
    {
        public static BaseAppManager current { get; protected set; }
        public bool isReady { get; protected set; }
        public static bool IsReady() => current && current.isReady;
        private GenericMap<IAppModule> modules = new GenericMap<IAppModule>();
        private DependencyContainer dependencies = new DependencyContainer();

        protected virtual void Awake()
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

        private void LoadApp()
        {
            modules.Values.Foreach(item => item.Init());
            isReady = true;
        }

        protected void Register<T>(T item) where T : class
        {
            if (item is IAppModule module)
            {
                modules.Set(module);
            }

            dependencies.Register(item);
        }

        protected abstract void RegisterDependencies();

        public void InjectDependenciesTo(DependencyContainer container)
        {
            container.AddDependencies(dependencies);
        }
    }

    public abstract class BaseAppManager<T> : BaseAppManager where T : BaseAppManager
    {
        public static T current { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            current = this as T;
        }
    }
}