using System;

namespace Vtex.Toolbelt.Cli.Commands
{
    public abstract class Command
    {
        public abstract void Run();

        protected string ReadLine(string message)
        {
            Console.Write(message);
            return Console.ReadLine();
        }

        protected string ReadPassword(string message)
        {
            Console.Write(message);

            var password = "";
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.Backspace:
                        password = password.Length > 0 ? password.Substring(0, password.Length - 1) : password;
                        break;

                    case ConsoleKey.Enter:
                        break;

                    default:
                        password += key.KeyChar;
                        break;
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }
    }
}