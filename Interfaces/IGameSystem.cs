namespace Libs.GameFramework.Interfaces
{
    public interface IGameSystem
    {
        void Init();
        void Subscribe();
        void Unsubscribe();
        void Start();
        void Stop();
    }
}