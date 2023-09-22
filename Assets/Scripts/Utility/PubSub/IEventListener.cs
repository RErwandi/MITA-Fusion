namespace Mita
{
    public interface IEventListener<T> : IEventListenerBase
    {
        void OnEvent(T e);
    }
}