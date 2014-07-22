using System;
using Moq;

namespace Vtex.Toolbelt.Tests.MockExtensions
{
    public static class CommandMatcherExtensions
    {
        public static ICommandMatcher ThatMatches(this ICommandMatcher @this, string[] args,
            Type commandType, int usedArgCount)
        {
            Mock.Get(@this)
                .Setup(matcher => matcher.TryGetMatchedType(args, out commandType, out usedArgCount))
                .Returns(true);
            return @this;
        }
    }
}
