using Libs.GameFramework.Interfaces;

namespace Libs.GameFramework
{
    public abstract class GameSystem : IGameSystem
    {
        public virtual void Init()
        {
        }

        public abstract void Subscribe();
        public abstract void Unsubscribe();

        public virtual void Start()
        {
        }

        public virtual void Stop()
        {
        }
    }
}