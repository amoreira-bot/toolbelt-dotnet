using System.IO;
using NUnit.Framework;
using Vtex.Toolbelt.CommandFramework;

namespace Vtex.Toolbelt.Tests.CommandFramework
{
    [TestFixture]
    class OptionSetTests
    {
        class Parse
        {
            [Test]
            public void Sets_flag_by_name()
            {
                // Arrange
                var options = new OptionSet();
                var wasSet = false;
                options.Add("foo", 'f', "Whether should foo", () => wasSet = true);

                // Act
                options.Parse("--foo");

                // Assert
                Assert.That(wasSet, Is.True);
            }

            [Test]
            public void Sets_flag_by_shorthand()
            {
                // Arrange
                var options = new OptionSet();
                var wasSet = false;
                options.Add("foo", 'f', "Whether should foo", () => wasSet = true);

                // Act
                options.Parse("-f");

                // Assert
                Assert.That(wasSet, Is.True);
            }

            [Test]
            public void Sets_chained_flags()
            {
                // Arrange
                var options = new OptionSet();
                var wasFooSet = false;
                var wasBarSet = false;
                options.Add("foo", 'f', "Whether should foo", () => wasFooSet = true);
                options.Add("bar", 'b', "Whether should bar", () => wasBarSet = true);

                // Act
                options.Parse("-fb");

                // Assert
                Assert.That(wasFooSet, Is.True);
                Assert.That(wasBarSet, Is.True);
            }

            [Test]
            public void Doesnt_allow_chaining_options_that_are_not_flags()
            {
                // Arrange
                var options = new OptionSet();
                options.Add("foo", 'f', "Whether should foo", () => { });
                options.Add("bar", 'b', "Whether should bar", value => { });

                // Act & Assert
                Assert.That(() => options.Parse("-fb"), Throws.TypeOf<OptionSyntaxException>()
                    .With.Message.EqualTo("Option cannot be used as a flag: -b"));
            }

            [Test]
            public void Sets_value_by_name()
            {
                // Arrange
                var options = new OptionSet();
                string actual = null;
                options.Add("foo", 'f', "Whether should foo", value => actual = value);

                // Act
                options.Parse("--foo", "the foo value");

                // Assert
                Assert.That(actual, Is.EqualTo("the foo value"));
            }

            [Test]
            public void Doesnt_allow_an_option_with_invalid_name()
            {
                // Arrange
                var options = new OptionSet();

                // Act & Assert
                Assert.That(() => options.Parse("--foo"), Throws.TypeOf<OptionSyntaxException>()
                    .With.Message.EqualTo("Invalid option: --foo"));
            }

            [Test]
            public void Sets_value_by_shorthand()
            {
                // Arrange
                var options = new OptionSet();
                string actual = null;
                options.Add("foo", 'f', "Whether should foo", value => actual = value);

                // Act
                options.Parse("-f", "the foo value");

                // Assert
                Assert.That(actual, Is.EqualTo("the foo value"));
            }

            [Test]
            public void Doesnt_allow_an_option_with_invalid_shorthand()
            {
                // Arrange
                var options = new OptionSet();

                // Act & Assert
                Assert.That(() => options.Parse("-f", "the foo value"), Throws.TypeOf<OptionSyntaxException>()
                    .With.Message.EqualTo("Invalid option: -f"));
            }

            [Test]
            public void Sets_value_by_position()
            {
                // Arrange
                var options = new OptionSet();
                string actual = null;
                options.Add(0, "name", "The name", value => actual = value);

                // Act
                options.Parse("yada");

                // Assert
                Assert.That(actual, Is.EqualTo("yada"));
            }

            [Test]
            public void Doesnt_allow_unexpected_value_by_position()
            {
                // Arrange
                var options = new OptionSet();

                // Act & Assert
                Assert.That(() => options.Parse("yada"), Throws.TypeOf<OptionSyntaxException>()
                    .With.Message.EqualTo("Invalid parameter: yada"));
            }

            [TestCase("acme tests --local --config ./config.json")]
            [TestCase("acme --local --config ./config.json tests")]
            [TestCase("acme --local -c ./config.json tests")]
            [TestCase("acme tests -l --config ./config.json")]
            public void Accepts_mixing_different_option(string rawArgs)
            {
                // Arrange
                var useLocal = false;
                string accountName = null, workspace = null, configPath = null;
                var options = new OptionSet
                {
                    {0, "account", "Account name", value => accountName = value},
                    {1, "workspace", "Workspace name", value => workspace = value},
                    {"local", 'l', "Use local storage", () => useLocal = true},
                    {"config", 'c', "Configuration file path", value => configPath = value}
                };

                // Act
                var args = rawArgs.Split(' ');
                options.Parse(args);

                // Assert
                Assert.That(accountName, Is.EqualTo("acme"));
                Assert.That(workspace, Is.EqualTo("tests"));
                Assert.That(useLocal, Is.True);
                Assert.That(configPath, Is.EqualTo("./config.json"));
            }
        }

        class Validate
        {
            [Test]
            public void Accepts_parameters_in_correct_positions()
            {
                // Arrange
                var options = new OptionSet
                {
                    {1, "two", "Second", value => { }},
                    {0, "one", "First", value => { }},
                    {2, "three", "Third", value => { }}
                };

                // Act & Assert
                Assert.That(options.Validate, Throws.Nothing);
            }

            [Test]
            public void Accepts_named_options_with_different_names_and_shorthands()
            {
                // Arrange
                var options = new OptionSet
                {
                    {"foo", 'f', "Foo flag", () => { }},
                    {"bar", 'b', "Bar value", value => { }},
                    {"zaz", 'z', "Zaz value", value => { }}
                };

                // Act & Assert
                Assert.That(options.Validate, Throws.Nothing);
            }

            [Test]
            public void Fails_for_more_than_one_parameter_in_the_same_position()
            {
                // Arrange
                var options = new OptionSet
                {
                    {0, "first", "First parameter", value => { }},
                    {0, "second", "Second parameter", value => { }}
                };

                // Act & Assert
                Assert.That(options.Validate, Throws.TypeOf<OptionsValidationException>()
                    .With.Message.Contains("Multiple parameters added to the same positions"));
            }

            [Test]
            public void Fails_for_non_continuous_parameter_positions()
            {
                // Arrange
                var options = new OptionSet
                {
                    {0, "first", "First parameter", value => { }},
                    {3, "second", "Second parameter", value => { }}
                };

                // Act & Assert
                Assert.That(options.Validate, Throws.TypeOf<OptionsValidationException>()
                    .With.Message.Contains("positions should be continuous and non-negative"));
            }

            [Test]
            public void Fails_for_negative_parameter_position()
            {
                // Arrange
                var options = new OptionSet
                {
                    {-1, "first", "First parameter", value => { }}
                };

                // Act & Assert
                Assert.That(options.Validate, Throws.TypeOf<OptionsValidationException>()
                    .With.Message.Contains("positions should be continuous and non-negative"));
            }

            [Test]
            public void Fails_for_required_parameter_after_an_optional_one()
            {
                // Arrange
                var options = new OptionSet
                {
                    {0, "first", "First parameter", value => { }},
                    {1, "second", "Second parameter", true, value => { }}
                };

                // Act & Assert
                Assert.That(options.Validate, Throws.TypeOf<OptionsValidationException>()
                    .With.Message.EqualTo("Optional parameters must appear after all required parameters"));
            }

            [Test]
            public void Fails_for_duplicate_option_names()
            {
                // Arrange
                var options = new OptionSet
                {
                    {"foo", 'a', "A flag", () => { }},
                    {"foo", 'b', "B flag", () => { }}
                };

                // Act & Assert
                Assert.That(options.Validate, Throws.TypeOf<OptionsValidationException>()
                    .With.Message.Contains("names are not unique"));
            }

            [Test]
            public void Fails_for_duplicate_option_shorthands()
            {
                // Arrange
                var options = new OptionSet
                {
                    {"one", 'n', "Flag one", () => { }},
                    {"two", 'n', "Flag two", () => { }}
                };

                // Act & Assert
                Assert.That(options.Validate, Throws.TypeOf<OptionsValidationException>()
                    .With.Message.Contains("shorthands are not unique"));
            }
        }

        class GetParametersUsage
        {
            [Test]
            public void Returns_empty_when_there_are_no_parameters()
            {
                // Arrange
                var options = new OptionSet();

                // Act
                var usage = options.GetParametersUsage();

                // Assert
                Assert.That(usage, Is.Empty);
            }

            [Test]
            public void Returns_an_optional_parameter_identified_by_its_name()
            {
                // Arrange
                var options = new OptionSet
                {
                    {0, "foo", "the foo", value => { }}
                };

                // Act
                var usage = options.GetParametersUsage();

                // Assert
                Assert.That(usage, Is.EqualTo("[<foo>]"));
            }

            [Test]
            public void Returns_a_required_parameter_identified_by_its_name()
            {
                // Arrange
                var options = new OptionSet
                {
                    {0, "foo", "the foo", true, value => { }}
                };

                // Act
                var usage = options.GetParametersUsage();

                // Assert
                Assert.That(usage, Is.EqualTo("<foo>"));
            }

            [Test]
            public void Returns_parameters_ordered_by_position()
            {
                // Arrange
                var options = new OptionSet
                {
                    {3, "foo", "the foo", true, value => { }},
                    {1, "bar", "the bar", true, value => { }},
                    {2, "zaz", "the zaz", value => { }}
                };

                // Act
                var usage = options.GetParametersUsage();

                // Assert
                Assert.That(usage, Is.EqualTo("<bar> [<zaz>] <foo>"));
            }
        }

        class WriteNamedOptionsUsage
        {
            [Test]
            public void Writes_nothing_if_there_are_no_options()
            {
                // Arrange
                var options = new OptionSet();
                var writer = new StringWriter();

                // Act
                options.WriteNamedOptionsUsage(writer);

                // Assert
                Assert.That(writer.ToString(), Is.Empty);
            }

            [Test]
            public void Ignores_all_parameters()
            {
                // Arrange
                var options = new OptionSet
                {
                    {0, "foo", "foo value", value => { }}
                };
                var writer = new StringWriter();

                // Act
                options.WriteNamedOptionsUsage(writer);

                // Assert
                Assert.That(writer.ToString(), Is.Empty);
            }

            [Test]
            public void Writes_help_for_a_flag()
            {
                // Arrange
                var options = new OptionSet
                {
                    {"foo", 'f', "foos all things", () => { }}
                };
                var writer = new StringWriter();

                // Act
                options.WriteNamedOptionsUsage(writer);

                // Assert
                Assert.That(writer.ToString().TrimEnd(),
                    Is.EqualTo("    -f, --foo             foos all things"));
            }

            [Test]
            public void Writes_help_for_a_value()
            {
                // Arrange
                var options = new OptionSet
                {
                    {"foo", 'f', "foos all things", value => { }}
                };
                var writer = new StringWriter();

                // Act
                options.WriteNamedOptionsUsage(writer);

                // Assert
                Assert.That(writer.ToString().TrimEnd(),
                    Is.EqualTo("    -f, --foo <value>     foos all things"));
            }

            [Test]
            public void Writes_help_for_a_required_option()
            {
                // Arrange
                var options = new OptionSet
                {
                    {"foo", 'f', "foos all things", true, value => { }}
                };
                var writer = new StringWriter();

                // Act
                options.WriteNamedOptionsUsage(writer);

                // Assert
                Assert.That(writer.ToString().TrimEnd(),
                    Is.EqualTo("    -f, --foo <value>     required - foos all things"));
            }
        }
    }
}
