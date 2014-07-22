using System;
using SimpleInjector;

namespace Vtex.Toolbelt
{
    public class DefaultServices
    {
        public static void RegisterTo(Container container)
        {
            container.RegisterSingle<IServiceProvider>(container);
        }
    }
}
