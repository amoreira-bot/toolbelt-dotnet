namespace Vtex.Toolbelt
{
    public abstract class Command : ICommand
    {
        public void Execute(string invokedWith, string[] args)
        {
            throw new System.NotImplementedException();
        }
    }
}