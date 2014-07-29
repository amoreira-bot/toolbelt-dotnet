using System;
using NUnit.Framework;
using Vtex.Toolbelt.CommandFramework;

namespace Vtex.Toolbelt.Tests
{
    [TestFixture]
    class AliasCommandMatcherTests
    {
        class TryGetMatchedType
        {
            [CommandHelp("some", "foo")]
            class SomeCommand
            {
            }

            [CommandHelp("another", "bar")]
            class AnotherCommand
            {
            }

            [Test]
            public void Fails_when_alias_cannot_be_found()
            {
                // Arrange
                var matcher = new AliasCommandMatcher(new Type[] {});
                Type commandType;
                int usedArgCount;

                // Act
                var success = matcher.TryGetMatchedType(new[] {"foo"}, out commandType, out usedArgCount);

                // Assert
                Assert.That(success, Is.False);
                Assert.That(commandType, Is.Null);
                Assert.That(usedArgCount, Is.EqualTo(1));
            }

            [Test]
            public void Fails_when_there_are_no_arguments()
            {
                // Arrange
                var matcher = new AliasCommandMatcher(new Type[] { });
                Type commandType;
                int usedArgCount;

                // Act
                var success = matcher.TryGetMatchedType(new string[0], out commandType, out usedArgCount);

                // Assert
                Assert.That(success, Is.False);
                Assert.That(commandType, Is.Null);
                Assert.That(usedArgCount, Is.EqualTo(1));
            }

            [TestCase(typeof(SomeCommand), "foo")]
            [TestCase(typeof(AnotherCommand), "bar")]
            [TestCase(typeof(SomeCommand), "FoO")]
            [TestCase(typeof(AnotherCommand), "bar", "something", "--an-option")]
            public void Finds_correct_command(Type expectedCommandType, params string[] args)
            {
                // Arrange
                var matcher = new AliasCommandMatcher(new[] {typeof (SomeCommand), typeof (AnotherCommand)});
                Type commandType;
                int usedArgCount;

                // Act
                var success = matcher.TryGetMatchedType(args, out commandType, out usedArgCount);

                // Assert
                Assert.That(success, Is.True);
                Assert.That(commandType, Is.EqualTo(expectedCommandType));
                Assert.That(usedArgCount, Is.EqualTo(1));
            }
        }
    }
}
