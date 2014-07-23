using System;
using System.Linq;
using System.Reflection;
using SimpleInjector;

namespace Vtex.Toolbelt
{
    public class DefaultServices
    {
        public static void RegisterTo(Container container)
        {
            container.RegisterSingle<IServiceProvider>(container);
            container.RegisterSingle<IConsole>(new ColoredConsole());

            var commandTypes = Assembly.GetExecutingAssembly().GetExportedTypes()
                .Where(type => typeof (Command).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                .ToList();
            var typeNameMatcher = new TypeNameCommandMatcher(commandTypes);
            var aliasMatcher = new AliasCommandMatcher(commandTypes);
            container.RegisterSingle<ICommandMatcher>(new CompositeCommandMatcher(typeNameMatcher, aliasMatcher));
        }
    }
}
