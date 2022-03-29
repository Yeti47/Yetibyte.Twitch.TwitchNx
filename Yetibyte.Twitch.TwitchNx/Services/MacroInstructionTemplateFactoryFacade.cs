using System;
using System.Collections.Generic;
using System.Linq;
using Yetibyte.Twitch.TwitchNx.Core.CommandModel.Macros;
using Yetibyte.Twitch.TwitchNx.Mvvm.ViewModels;

namespace Yetibyte.Twitch.TwitchNx.Services
{
    internal class MacroInstructionTemplateFactoryFacade : IMacroInstructionTemplateFactoryFacade
    {
        private readonly List<IMacroInstructionTemplateFactory> _macroInstructionTemplateFactories = new List<IMacroInstructionTemplateFactory>();

        private bool HasFactories => _macroInstructionTemplateFactories.Any();

        public MacroInstructionTemplateViewModel CreateFor(IMacroInstruction macroInstruction)
        {
            if (!HasFactories)
                FindFactories();

            IMacroInstructionTemplateFactory? applicableFactory = _macroInstructionTemplateFactories.FirstOrDefault(f => f.AppliesTo(macroInstruction));

            if (applicableFactory is null)
                throw new ArgumentOutOfRangeException(nameof(macroInstruction), "Could not find an applicable factory for the given macro instruction.");

            return applicableFactory.CreateFor(macroInstruction);
        }

        private void FindFactories()
        {
            _macroInstructionTemplateFactories.Clear();

            IEnumerable<Type> factoryTypes = 
                GetType()
                .Assembly
                .GetTypes()
                .Where(t => t != typeof(IMacroInstructionTemplateFactory) && typeof(IMacroInstructionTemplateFactory).IsAssignableFrom(t));

            foreach (var factoryType in factoryTypes)
            {
                IMacroInstructionTemplateFactory? factory = Activator.CreateInstance(factoryType) as IMacroInstructionTemplateFactory;

                if (factory is not null)
                    _macroInstructionTemplateFactories.Add(factory);
            }
        }
    }
}
