using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Vtex.Toolbelt.CommandFramework
{
    public class OptionSet : List<Option>
    {
        /// <summary>
        /// Add a named option
        /// </summary>
        /// <param name="name">Option name</param>
        /// <param name="shorthand">Option shorthand</param>
        /// <param name="description">Option description</param>
        /// <param name="apply">Action that will be called passing the supplied value</param>
        public void Add(string name, char shorthand, string description, Action<string> apply)
        {
            this.Add(new NamedOption(name, shorthand, description, false, apply));
        }

        /// <summary>
        /// Add a parameter
        /// </summary>
        /// <param name="position">Zero-based position of the parameter</param>
        /// <param name="name">Parameter name to show in the usage</param>
        /// <param name="description">Parameter description</param>
        /// <param name="apply">Action that will be called passing the supplied value</param>
        public void Add(int position, string name, string description, Action<string> apply)
        {
            this.Add(new ParameterOption(position, name, description, false, apply));
        }

        /// <summary>
        /// Add a flag option
        /// </summary>
        /// <param name="name">Flag name</param>
        /// <param name="shorthand">Flag shorthand</param>
        /// <param name="description">Flag description</param>
        /// <param name="apply">Action that will be called if flag is used</param>
        public void Add(string name, char shorthand, string description, Action apply)
        {
            this.Add(new FlagOption(name, shorthand, description, apply));
        }

        /// <summary>
        /// Add a named option
        /// </summary>
        /// <param name="name">Option name</param>
        /// <param name="shorthand">Option shorthand</param>
        /// <param name="description">Option description</param>
        /// <param name="required">Whether the option must be specified or not</param>
        /// <param name="apply">Action that will be called passing the supplied value</param>
        public void Add(string name, char shorthand, string description, bool required, Action<string> apply)
        {
            this.Add(new NamedOption(name, shorthand, description, required, apply));
        }

        /// <summary>
        /// Add a parameter
        /// </summary>
        /// <param name="position">Zero-based position of the parameter</param>
        /// <param name="name">Parameter name to show in the usage</param>
        /// <param name="description">Parameter description</param>
        /// <param name="required">Whether the parameter must be specified or not</param>
        /// <param name="apply">Action that will be called passing the supplied value</param>
        public void Add(int position, string name, string description, bool required, Action<string> apply)
        {
            this.Add(new ParameterOption(position, name, description, required, apply));
        }

        /// <summary>
        /// Parse the command line arguments and apply each option value
        /// </summary>
        /// <param name="args">The command line arguments to parse</param>
        /// <exception cref="OptionSyntaxException">
        /// Is thrown if the arguments aren't compatible with the configured options
        /// </exception>
        public void Parse(params string[] args)
        {
            var parser = new OptionParser(IndexParameters(), IndexOptionsByName(), IndexOptionsByShorthand());
            parser.Parse(new Queue<string>(args));
        }

        /// <summary>
        /// Validats the current configuration of the OptionSet
        /// </summary>
        /// <exception cref="OptionsValidationException">
        /// Is thrown if any part of the options configuration is invalid
        /// </exception>
        public void Validate()
        {
            Validate(this.Where(o => o is NamedOption).Cast<NamedOption>().ToArray());
            Validate(this.Where(o => o is ParameterOption).Cast<ParameterOption>().ToArray());
        }

        public string GetParametersUsage()
        {
            return string.Join(" ", this.Where(o => o is ParameterOption)
                .Cast<ParameterOption>().OrderBy(p => p.Position)
                .Select(p => p.Required ? "<" + p.Name + ">" : "[<" + p.Name + ">]"));
        }

        public void WriteNamedOptionsUsage(TextWriter writer)
        {
            foreach (var option in this.Where(o => o is NamedOption).Cast<NamedOption>())
            {
                var usage = option is FlagOption
                    ? string.Format("-{0}, --{1}", option.Shorthand, option.Name)
                    : string.Format("-{0}, --{1} <value>", option.Shorthand, option.Name);

                var description = option.Required ? "required - " + option.Description : option.Description;

                if (usage.Length < 22)
                {
                    writer.WriteLine("    {0, -22}{1}", usage, description);
                }
                else
                {
                    writer.WriteLine("    " + usage);
                    writer.WriteLine("                      " + description);
                }
            }
        }

        private static void Validate(ICollection<NamedOption> options)
        {
            if (!options.Any())
                return;

            var repeatedNames = options
                .Where(one => options.Any(other => one != other && other.Name == one.Name))
                .Select(o => o.Name).Distinct().ToArray();
            if (repeatedNames.Any())
            {
                throw new OptionsValidationException("The following option names are not unique: " +
                                                     string.Join(", ", repeatedNames));
            }

            var repeatedShorthands = options
                .Where(one => options.Any(other => one != other && other.Shorthand == one.Shorthand))
                .Select(o => o.Shorthand).Distinct().ToArray();
            if (repeatedShorthands.Any())
            {
                throw new OptionsValidationException("The following option shorthands are not unique: " +
                                                     string.Join(", ", repeatedShorthands));
            }
        }

        private static void Validate(ICollection<ParameterOption> parameters)
        {
            if (!parameters.Any())
                return;

            if (parameters.Any(p => p.Position < 0 || p.Position >= parameters.Count))
            {
                throw new OptionsValidationException("The parameters' positions should be continuous and non-negative");
            }

            var duplicates = parameters
                .Where(one => parameters.Any(other => one != other && one.Position == other.Position)).ToArray();
            if (duplicates.Any())
            {
                throw new OptionsValidationException("Multiple parameters added to the same positions: " +
                                                    string.Join(", ", duplicates.Select(d => d.Position).Distinct()));
            }

            var lastParameterIsRequired = true;
            foreach (var parameter in parameters.OrderBy(p => p.Position))
            {
                if (!lastParameterIsRequired && parameter.Required)
                    throw new OptionsValidationException("Optional parameters must appear after all required parameters");
                lastParameterIsRequired = parameter.Required;
            }
        }

        private Dictionary<char, Option> IndexOptionsByShorthand()
        {
            return this.Where(o => o is NamedOption).ToDictionary(o => ((NamedOption)o).Shorthand);
        }

        private Dictionary<string, Option> IndexOptionsByName()
        {
            return this.Where(o => o is NamedOption).ToDictionary(o => ((NamedOption)o).Name);
        }

        private Option[] IndexParameters()
        {
            return this.Where(o => o is ParameterOption).ToArray();
        }
    }
}
