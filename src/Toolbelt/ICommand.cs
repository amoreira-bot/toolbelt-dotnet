namespace Vtex.Toolbelt
{
    public interface ICommand
    {
        void Execute(string invokedWith, string[] args);
    }
}