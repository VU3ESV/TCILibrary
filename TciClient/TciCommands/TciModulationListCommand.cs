using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciModulationListCommand : ITciCommand, IDisposable
    {
        private TciModulationListCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }

        public static TciModulationListCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciModulationListCommand(transceiverController);
        }

        public static string Name => "modulations_list";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var modulationListsMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(modulationListsMessage))
            {
                return false;
            }

            var modulationListsMessageElements = modulationListsMessage.Split(':', ',', ';');
            if (modulationListsMessageElements.Length <= MinimumCommandParameterCount)
            {
                return false;
            }

            var modulationList = new List<string>();
            for (var i = 1; i < modulationListsMessageElements.Length - 1; i++)
            {
                modulationList.Add(modulationListsMessageElements[i]);
            }

            (_transceiverController as TransceiverController)?.AddModulationList(modulationList);
            return true;
        }

        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int MinimumCommandParameterCount = 2;
    }
}
