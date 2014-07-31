using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Vtex.Toolbelt
{
    public class ColoredConsole : IConsole
    {
        private static readonly Regex Tokenizer = BuildTokenizerRegex();

        public TextWriter Out
        {
            get { return Console.Out; }
        }

        private static Regex BuildTokenizerRegex()
        {
            var parts = new[]
            {
                @"\[\[|\]\]",
                @"[^[\]]+",
                @"\[#(?<color>[a-z]+)\s?",
                @"]",
            };
            return new Regex("(" + string.Join(")|(", parts) + ")");
        }

        public void Write(string format, params object[] args)
        {
            this.Write((args == null ? format : string.Format(format, args)));
        }

        public void Write(string content)
        {
            var matches = Tokenizer.Matches(content);
            for (var i = 0; i < matches.Count; i++)
            {
                if (matches[i].Value == "]")
                {
                    Console.ResetColor();
                }
                else if (matches[i].Value == "[[" || matches[i].Value == "]]")
                {
                    Console.Write(matches[i].Value[0]);
                }
                else
                {
                    var colorGroup = matches[i].Groups["color"];
                    if (colorGroup.Success)
                    {
                        Console.ForegroundColor = ParseColor(colorGroup.Value);
                    }
                    else
                    {
                        Console.Write(matches[i].Value);
                    }
                }
            }
        }

        public void WriteLine(string format, params object[] args)
        {
            this.Write((args == null ? format : string.Format(format, args)) + Console.Out.NewLine);
        }

        public void WriteLine(string content)
        {
            this.Write(content + Console.Out.NewLine);
        }
        
        public void WriteLine()
        {
            Console.WriteLine();
        }

        private static ConsoleColor ParseColor(string colorString)
        {
            ConsoleColor color;
            if (Enum.TryParse(colorString, true, out color))
                return color;
            throw new ArgumentException("Unidentified color: " + colorString, "colorString");
        }

        public string ReadLine(string message)
        {
            Console.Write(message);
            return Console.ReadLine();
        }

        public string ReadPassword(string message)
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

        public char Prompt(string message, Action<PromptConfiguration> configure)
        {
            var prompt = new PromptConfiguration();
            configure(prompt);

            var optionsMessage = BuildOptionsLine(prompt);

            while (true)
            {
                this.WriteLine(message);
                this.Write(optionsMessage);
                var key = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(key))
                {
                    var option = prompt.FirstOrDefault(o => o.IsDefault);
                    if (option != null)
                        return option.Key;
                }
                else if(key.Length == 1)
                {
                    var keyChar = char.ToUpperInvariant(key[0]);
                    var option = prompt.FirstOrDefault(o => o.Key == keyChar);
                    if (option != null)
                        return option.Key;
                }

                Console.WriteLine("Invalid option.");
                Console.WriteLine();
            }
        }

        private static string BuildOptionsLine(PromptConfiguration prompt)
        {
            var optionsMessage = string.Join("  ", prompt.Select(option => option.IsDefault
                ? string.Format("[#yellow [[{0}]] {1}]", option.Key, option.Description)
                : string.Format("[[{0}]] {1}", option.Key, option.Description)));

            var defaultOption = prompt.FirstOrDefault(option => option.IsDefault);
            if (defaultOption != null)
                optionsMessage += string.Format(" (default is \"{0}\")", defaultOption.Key);
            optionsMessage += ": ";
            return optionsMessage;
        }
    }
}
