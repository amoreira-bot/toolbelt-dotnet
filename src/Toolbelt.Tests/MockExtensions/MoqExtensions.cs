using System;
using System.Linq.Expressions;
using Moq;
using Moq.Language.Flow;

namespace Vtex.Toolbelt.Tests.MockExtensions
{
    public static class MoqExtensions
    {
        public static ISetup<T, TResult> Setup<T, TResult>(this T @this, Expression<Func<T, TResult>> expression)
            where T : class
        {
            return Mock.Get(@this).Setup(expression);
        }

        public static ISetup<T> Setup<T>(this T @this, Expression<Action<T>> expression)
            where T : class
        {
            return Mock.Get(@this).Setup(expression);
        }
    }
}
