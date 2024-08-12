namespace Simplify.ORM.Interfaces
{
    public interface ISimplifyCommand
    {
        Task Execute(ISimplifyCommandBuilder command);
        Task Execute(IEnumerable<ISimplifyCommandBuilder> commands);

        Task ExecuteAsync(ISimplifyCommandBuilder command);
        Task ExecuteAsync(IEnumerable<ISimplifyCommandBuilder> commands);
    }
}
