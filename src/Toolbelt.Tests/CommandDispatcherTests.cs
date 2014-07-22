using System;
using Moq;
using NUnit.Framework;
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
                var dispatcher = new CommandDispatcher(Mock.Of<ICommandMatcher>(), null);

                // Act & Assert
                Assert.That(() => dispatcher.Dispatch(new[] {"foo"}),
                    Throws.TypeOf<DispatchException>());
            }

            [Test]
            public void Throws_dispatch_exception_when_command_execution_fails()
            {
                // Arrange
                var command = Mock.Of<Command>();
                command.Setup(c => c.Execute(new[] {"foo"})).Throws<InvalidOperationException>();

                var services = Mock.Of<IServiceProvider>(s => s.GetService(command.GetType()) == command);

                var commandMatcher = Mock.Of<ICommandMatcher>()
                    .ThatMatches(new[] {"foo"}, command.GetType(), 0);

                var dispatcher = new CommandDispatcher(commandMatcher, services);

                // Act & Assert
                Assert.That(() => dispatcher.Dispatch(new[] { "foo" }),
                    Throws.TypeOf<DispatchException>());
            }

            [Test]
            public void Excecutes_command_with_correct_arguments()
            {
                // Arrange
                var command = Mock.Of<Command>();
                var services = Mock.Of<IServiceProvider>(s => s.GetService(command.GetType()) == command);

                var commandMatcher = Mock.Of<ICommandMatcher>()
                    .ThatMatches(new[] {"do", "some", "thing"}, command.GetType(), 2);

                var dispatcher = new CommandDispatcher(commandMatcher, services);

                // Act
                dispatcher.Dispatch(new[] {"do", "some", "thing"});

                // Verify
                Mock.Get(command).Verify(c => c.Execute(new[] {"thing"}));
            }
        }
    }
}
