using System.Reflection;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources
{
    public class CommandSourceAssemblyFinder
    {
        private const string COMMAND_SOURCES_DIRECTORY_NAME = "CommandSources";
        private const string ASSEMBLY_FILE_SEARCH_PATTERN = "*.dll";

        public IEnumerable<Assembly> FindCommandSourceAssemblies()
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
