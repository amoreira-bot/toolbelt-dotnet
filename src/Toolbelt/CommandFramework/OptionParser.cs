using System.Collections.Generic;
using System.Linq;

namespace Vtex.Toolbelt.CommandFramework
{
    public class OptionParser
    {
        private readonly Option[] _parameters;
        private readonly Dictionary<string, Option> _optionsByName;
        private readonly Dictionary<char, Option> _optionsByShorthand;

        public OptionParser(Option[] parameters, Dictionary<string, Option> optionsByName,
            Dictionary<char, Option> indexOptionsByShorthand)
        {
            _parameters = parameters;
            _optionsByName = optionsByName;
            _optionsByShorthand = indexOptionsByShorthand;
        }

        public void Parse(Queue<string> arguments)
        {
            var nextPosition = 0;
            while (arguments.Any())
            {
                var current = arguments.Dequeue();
                if (!(TryParseName(current, arguments) || TryParseShorthand(current, arguments)))
                    nextPosition = ParsePositional(current, nextPosition);
            }
        }

        private bool TryParseName(string current, Queue<string> arguments)
        {
            if (!current.StartsWith("--") || current.Length < 3)
                return false;

            var name = current.Substring(2);
            var option = GetNamedOption(name);
            if (!TryApplyFlag(option))
            {
                var value = arguments.Dequeue();
                option.Apply(value);
            }
            return true;
        }

        private bool TryParseShorthand(string current, Queue<string> arguments)
        {
            if (!current.StartsWith("-") || current.Length < 2)
                return false;

            if (HasChainedFlags(current))
            {
                foreach (var shorthand in current.Skip(1))
                {
                    var option = GetShorthandOption(shorthand);
                    if (!TryApplyFlag(option))
                        throw new OptionSyntaxException("Option cannot be used as a flag: -" + shorthand);
                }
            }
            else
            {
                var option = GetShorthandOption(current[1]);
                if (!TryApplyFlag(option))
                {
                    var value = arguments.Dequeue();
                    option.Apply(value);
                }
            }
            return true;
        }

        private int ParsePositional(string current, int position)
        {
            if(_parameters.Length <= position)
                throw new OptionSyntaxException("Invalid parameter: " + current);

            _parameters[position].Apply(current);
            return position + 1;
        }

        private Option GetShorthandOption(char shorthand)
        {
            Option option;
            if (!_optionsByShorthand.TryGetValue(shorthand, out option))
                throw new OptionSyntaxException("Invalid option: -" + shorthand);
            return option;
        }

        private Option GetNamedOption(string name)
        {
            Option option;
            if (!_optionsByName.TryGetValue(name, out option))
                throw new OptionSyntaxException("Invalid option: --" + name);
            return option;
        }

        private static bool HasChainedFlags(string argument)
        {
            return argument.Length > 2;
        }

        private static bool TryApplyFlag(Option option)
        {
            if (!(option is FlagOption))
                return false;

            option.Apply(string.Empty);
            return true;
        }
    }
}