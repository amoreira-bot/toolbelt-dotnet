using System;
using NUnit.Framework;
using Vtex.Toolbelt.CommandFramework;

namespace Vtex.Toolbelt.Tests
{
    [TestFixture]
    internal class TypeNameCommandMatcherTests
    {
        private class TryGetMatchedType
        {
            private class CreateFooCommand
            {
            }

            private class FooCommand
            {
            }

            private class BarCommand
            {
            }

            private class HelpCommand
            {
            }

            [Test]
            public void Fails_for_a_command_that_cannot_be_found()
            {
                // Arrange
                var matcher = new TypeNameCommandMatcher(new Type[] {});
                Type commandType;
                int usedArgCount;

                // Act
                var success = matcher.TryGetMatchedType(new[] {"foo"}, out commandType, out usedArgCount);

                // Assert
                Assert.That(success, Is.False);
                Assert.That(commandType, Is.EqualTo(null));
                Assert.That(usedArgCount, Is.EqualTo(1));
            }

            [TestCase(typeof (FooCommand), 1, "foo")]
            [TestCase(typeof (FooCommand), 1, "foo", "--someoption")]
            [TestCase(typeof (CreateFooCommand), 2, "foo", "create")]
            [TestCase(typeof (CreateFooCommand), 2, "FoO", "CreATe")]
            [TestCase(typeof (CreateFooCommand), 2, "foo", "create", "somename")]
            [TestCase(typeof (CreateFooCommand), 2, "foo", "create", "--someoption")]
            [TestCase(typeof (BarCommand), 1, "bar")]
            [TestCase(typeof (BarCommand), 1, "bar", "-h")]
            public void Finds_correct_command(Type expectedCommandType, int expectedUsedArgCount, params string[] args)
            {
                // Arrange
                var matcher = new TypeNameCommandMatcher(new[]
                {
                    typeof (CreateFooCommand),
                    typeof (FooCommand),
                    typeof (BarCommand)
                });
                Type commandType;
                int usedArgCount;

                // Act
                var success = matcher.TryGetMatchedType(args, out commandType, out usedArgCount);

                // Assert
                Assert.That(success, Is.True);
                Assert.That(commandType, Is.EqualTo(expectedCommandType));
                Assert.That(usedArgCount, Is.EqualTo(expectedUsedArgCount));
            }

            [Test]
            public void Falls_back_to_help_command()
            {
                // Arrange
                var matcher = new TypeNameCommandMatcher(new[] { typeof(FooCommand), typeof(HelpCommand) });
                Type commandType;
                int usedArgCount;

                // Act
                var success = matcher.TryGetMatchedType(new string[0], out commandType, out usedArgCount);

                // Assert
                Assert.That(success, Is.True);
                Assert.That(commandType, Is.EqualTo(typeof(HelpCommand)));
                Assert.That(usedArgCount, Is.EqualTo(0));
            }
        }
    }
}
