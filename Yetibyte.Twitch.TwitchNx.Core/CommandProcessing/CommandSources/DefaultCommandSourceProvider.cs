using System.IO;
using System.Reflection;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources
{
    public class DefaultCommandSourceProvider : ICommandSourceProvider
    {
        private const string COMMAND_SOURCES_DIRECTORY_NAME = "CommandSources";
        private const string ASSEMBLY_FILE_SEARCH_PATTERN = "*.dll";

        private readonly List<ICommandSourceFactory> _commandSourceFactories = new List<ICommandSourceFactory>();

        public IEnumerable<ICommandSourceFactory> GetCommandSourceFactories()
        {
            return _commandSourceFactories;
        }

        public void Load()
        {
            _commandSourceFactories.Clear();

            foreach(Assembly assembly in FindCommandSourceAssemblies())
            {
                foreach(Type commandSourceFactoryType in assembly.GetTypes().Where(ImplementsCommandSourceFactory)) {

                    ICommandSourceFactory? commandSourceFactory = null;

                    try
                    {
                        commandSourceFactory = Activator.CreateInstance(commandSourceFactoryType) as ICommandSourceFactory;
                    }
                    catch(Exception ex)
                    {
                        // TODO: proper error handling
                    }

                    if (commandSourceFactory != null)
                        _commandSourceFactories.Add(commandSourceFactory);

                }
            }

        }

        private static bool ImplementsCommandSourceFactory(Type type)
        {
            return typeof(ICommandSourceFactory).IsAssignableFrom(type) 
                && type != typeof(ICommandSourceFactory);
        }

        private IEnumerable<Assembly> FindCommandSourceAssemblies()
        {
            List<Assembly> assemblies = new List<Assembly>();

            // Always include the core assembly
            Assembly? currentAssembly = Assembly.GetAssembly(GetType());

            if (currentAssembly != null)
                assemblies.Add(currentAssembly);

            Directory.CreateDirectory(COMMAND_SOURCES_DIRECTORY_NAME);

            foreach (string fileName in Directory.EnumerateFiles(COMMAND_SOURCES_DIRECTORY_NAME, ASSEMBLY_FILE_SEARCH_PATTERN))
            {
                Assembly? commandSourceAssembly = null;

                try
                {
                    commandSourceAssembly = Assembly.LoadFrom(fileName);
                }
                catch
                {
                    // TODO: proper error handling
                }

                if (commandSourceAssembly != null)
                {
                    assemblies.Add(commandSourceAssembly);
                }
            }

            return assemblies;
        }
    }
}
