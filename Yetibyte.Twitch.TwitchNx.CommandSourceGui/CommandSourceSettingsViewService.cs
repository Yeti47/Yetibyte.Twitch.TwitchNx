using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using Yetibyte.Twitch.TwitchNx.Core.CommandProcessing.CommandSources;

namespace Yetibyte.Twitch.TwitchNx.CommandSourceGui
{
    public class CommandSourceSettingsViewService
    {
        public record CommandSourceSettingsViewConfig(Type ViewType, Type ViewModelType);

        private const string XML_NAMESPACE_PREFIX_CS_VIEWMODEL = "csvmplg";
        private const string XML_NAMESPACE_PREFIX_CS_VIEW = "csvplg";
        private const string XML_NAMESPACE_PREFIX_XAML = "xx";

        private readonly CommandSourceAssemblyFinder _commandSourceAssemblyFinder = new CommandSourceAssemblyFinder();

        private readonly List<CommandSourceSettingsViewConfig> _commandSourceSettingsViewConfigs = new List<CommandSourceSettingsViewConfig>();

        private bool HasLoadedConfiguration => _commandSourceSettingsViewConfigs.Any();

        public CommandSourceSettingsViewService()
        {

        }

        public void RegisterCommandSourceSettingsDataTemplates()
        {
            if (!HasLoadedConfiguration)
                LoadCommandSourceSettingsViewConfiguration();

            const string xamlTemplate =
                 @" <DataTemplate DataType=""{{xx:Type csvmplg:{0}}}"">
                        <csvplg:{1} />
                    </DataTemplate >
                 ";

            foreach (var config in _commandSourceSettingsViewConfigs)
            {
                string dataTemplateXaml = string.Format(xamlTemplate, config.ViewModelType.Name, config.ViewType.Name);

                var parserCtx = new ParserContext
                {
                    XamlTypeMapper = new XamlTypeMapper(Array.Empty<string>())
                };

                parserCtx.XamlTypeMapper.AddMappingProcessingInstruction(XML_NAMESPACE_PREFIX_CS_VIEWMODEL, config.ViewModelType.Namespace, config.ViewModelType.Assembly.FullName);
                parserCtx.XamlTypeMapper.AddMappingProcessingInstruction(XML_NAMESPACE_PREFIX_CS_VIEW, config.ViewType.Namespace, config.ViewType.Assembly.FullName);

                parserCtx.XmlnsDictionary.Add(string.Empty, "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                parserCtx.XmlnsDictionary.Add(XML_NAMESPACE_PREFIX_XAML, "http://schemas.microsoft.com/winfx/2006/xaml");
                parserCtx.XmlnsDictionary.Add(XML_NAMESPACE_PREFIX_CS_VIEWMODEL, XML_NAMESPACE_PREFIX_CS_VIEWMODEL);
                parserCtx.XmlnsDictionary.Add(XML_NAMESPACE_PREFIX_CS_VIEW, XML_NAMESPACE_PREFIX_CS_VIEW);

                DataTemplate? dataTemplate = XamlReader.Parse(dataTemplateXaml, parserCtx) as DataTemplate;

                if (dataTemplate != null)
                {
                    Application.Current.Resources.Add(dataTemplate.DataTemplateKey, dataTemplate);
                }
            }
        }

        private void LoadCommandSourceSettingsViewConfiguration()
        {
            _commandSourceSettingsViewConfigs.Clear();
            IEnumerable<Assembly> commandSourceAssemblies = FindAssemblies();

            foreach (var viewType in commandSourceAssemblies.SelectMany(a => a.GetTypes()))
            {
                if (viewType.GetCustomAttributes(typeof(CommandSourceSettingsViewAttribute), true)?.FirstOrDefault() is not CommandSourceSettingsViewAttribute cmdSourceSettingsViewAttribute)
                    continue;

                var config = new CommandSourceSettingsViewConfig(viewType, cmdSourceSettingsViewAttribute.ViewModelType);

                _commandSourceSettingsViewConfigs.Add(config);
            }
        }

        private IEnumerable<Assembly> FindAssemblies()
        {
            return _commandSourceAssemblyFinder.FindCommandSourceAssemblies().Concat(new[] { Application.Current.GetType().Assembly });
        }
    }
}
