using System;
using System.IO;
using System.Linq;
using System.Reflection;
using SimpleInjector;
using Vtex.Toolbelt.Services;

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

            container.RegisterSingle<Configuration>(ReadConfiguration);
            container.RegisterSingle<IFileSystem>(new PhysicalFileSystem());
        }

        private static Configuration ReadConfiguration()
        {
            var home = Environment.OSVersion.Platform == PlatformID.MacOSX ||
                       Environment.OSVersion.Platform == PlatformID.Unix
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

            var configFilePath = Path.Combine(home, ".vtexrc");
            var configurationReader = new ConfigurationReader(configFilePath);
            return configurationReader.ReadConfiguration();
        }
    }
}
