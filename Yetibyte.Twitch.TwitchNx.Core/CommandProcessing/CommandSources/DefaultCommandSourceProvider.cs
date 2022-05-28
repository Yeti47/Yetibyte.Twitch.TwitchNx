using System.IO;
using System.Reflection;

namespace Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources
{
    public class DefaultCommandSourceProvider : ICommandSourceProvider
    {
        private readonly List<ICommandSourceFactory> _commandSourceFactories = new List<ICommandSourceFactory>();

        private readonly CommandSourceAssemblyFinder _commandSourceAssemblyFinder = new CommandSourceAssemblyFinder();

        public IEnumerable<ICommandSourceFactory> GetCommandSourceFactories()
        {
            return _commandSourceFactories;
        }

        public void Load()
        {
            _commandSourceFactories.Clear();

            foreach(Assembly assembly in _commandSourceAssemblyFinder.FindCommandSourceAssemblies())
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


    }
}
