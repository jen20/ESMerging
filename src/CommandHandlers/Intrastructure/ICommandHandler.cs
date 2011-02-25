using Commands;

namespace CommandHandlers
{
    public interface ICommandHandler<T> where T : Command
    {
        void Handle(T command);
    }
}