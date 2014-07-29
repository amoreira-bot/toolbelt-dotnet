using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Vtex.Toolbelt.CommandFramework;
using Vtex.Toolbelt.Tests.MockExtensions;

namespace Vtex.Toolbelt.Tests
{
    [TestFixture]
    class CommandDispatcherTests
    {
        class Dispatch
        {
            [Test]
            public void Throws_exception_when_command_isnt_found()
            {
                // Arrange
                var dispatcher = new TestableCommandDispatcher(Mock.Of<ICommandMatcher>());

                // Act & Assert
                Assert.That(() => dispatcher.Dispatch(new[] {"foo"}),
                    Throws.TypeOf<DispatchException>());
            }

            [Test]
            public void Throws_dispatch_exception_when_command_execution_fails()
            {
                // Arrange
                var command = Mock.Of<ICommand>();
                command.Setup(c => c.Execute("", new[] {"foo"})).Throws<InvalidOperationException>();

                var commandMatcher = Mock.Of<ICommandMatcher>()
                    .ThatMatches(new[] {"foo"}, command.GetType(), 0);

                var dispatcher = new TestableCommandDispatcher(commandMatcher)
                    .WithCommand(command);

                // Act & Assert
                Assert.That(() => dispatcher.Dispatch(new[] { "foo" }),
                    Throws.TypeOf<DispatchException>());
            }

            [Test]
            public void Executes_command_with_correct_arguments()
            {
                // Arrange
                var command = Mock.Of<ICommand>();
                var commandMatcher = Mock.Of<ICommandMatcher>()
                    .ThatMatches(new[] {"do", "some", "thing"}, command.GetType(), 2);

                var dispatcher = new TestableCommandDispatcher(commandMatcher)
                    .WithCommand(command);

                // Act
                dispatcher.Dispatch(new[] {"do", "some", "thing"});

                // Verify
                Mock.Get(command).Verify(c => c.Execute("do some", new[] {"thing"}));
            }

            public class TestableCommandDispatcher : CommandDispatcher
            {
                private readonly Dictionary<Type, ICommand> _commandsMock = new Dictionary<Type, ICommand>();

                public TestableCommandDispatcher(ICommandMatcher commandMatcher)
                    : base(commandMatcher, null)
                {
                }

                protected override ICommand CreateCommand(Type commandType)
                {
                    return _commandsMock[commandType];
                }

                public TestableCommandDispatcher WithCommand(ICommand command)
                {
                    _commandsMock[command.GetType()] = command;
                    return this;
                }
            }
        }
    }
}
