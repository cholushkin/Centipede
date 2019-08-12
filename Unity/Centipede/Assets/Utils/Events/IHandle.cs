namespace Utils
{
    public interface IHandle
    {
    }

    public interface IHandle<TMessage> : IHandle
    {
        void Handle(TMessage message);
    }
}
