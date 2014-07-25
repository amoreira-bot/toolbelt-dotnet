using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Vtex.Toolbelt.Services;

namespace Vtex.Toolbelt.Tests.Services
{
    [TestFixture]
    public class DebouncerTests
    {
        class Debounce
        {
            [Test]
            public void Should_not_immediately_fire_action()
            {
                // Arrange
                var debouncer = new Debouncer(TimeSpan.FromMilliseconds(20));
                var fireCount = 0;
                Action action = () => fireCount++;

                // Act
                debouncer.Debounce(action);

                // Assert
                Assert.That(fireCount, Is.EqualTo(0));
            }

            [Test]
            public void Should_fire_action_after_period_of_time()
            {
                // Arrange
                var debouncer = new Debouncer(TimeSpan.FromMilliseconds(20));
                var fireCount = 0;
                Action action = () => fireCount++;

                // Act
                debouncer.Debounce(action);
                Task.Delay(50).Wait();

                // Assert
                Assert.That(fireCount, Is.EqualTo(1));
            }

            [Test]
            public void Multiple_calls_in_a_short_period_of_time_should_fire_only_once()
            {
                // Arrange
                var debouncer = new Debouncer(TimeSpan.FromMilliseconds(20));
                var fireCount = 0;
                Action action = () => fireCount++;

                // Act
                debouncer.Debounce(action);
                debouncer.Debounce(action);
                debouncer.Debounce(action);
                Task.Delay(50).Wait();

                // Assert
                Assert.That(fireCount, Is.EqualTo(1));
            }

            [Test]
            public void Calls_spaced_by_a_longer_time_should_fire_sepparately()
            {
                // Arrange
                var debouncer = new Debouncer(TimeSpan.FromMilliseconds(20));
                var fireCount = 0;
                Action action = () => fireCount++;

                // Act
                debouncer.Debounce(action);
                Task.Delay(50).Wait();
                debouncer.Debounce(action);
                Task.Delay(50).Wait();

                // Assert
                Assert.That(fireCount, Is.EqualTo(2));
            }
        }
    }
}