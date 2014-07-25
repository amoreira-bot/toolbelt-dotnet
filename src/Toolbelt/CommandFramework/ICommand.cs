namespace Vtex.Toolbelt.CommandFramework
{
    public interface ICommand
    {
        void Execute(string invokedWith, string[] args);
    }
}