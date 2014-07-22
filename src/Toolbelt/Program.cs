using System;
using SimpleInjector;

namespace Vtex.Toolbelt
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new Container();
            DefaultServices.RegisterTo(container);

            var commandDispatcher = container.GetInstance<CommandDispatcher>();
            try
            {
                commandDispatcher.Dispatch(args);
            }
            catch (DispatchException exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Error: {0}", exception.Message);
                Console.ResetColor();
            }
        }
    }
}
