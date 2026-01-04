using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    /// <summary>
    /// Represents a modulation list command for the TCI device.
    /// </summary>
    public class TciModulationListCommand : ITciCommand, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TciModulationListCommand"/> class.
        /// </summary>
        /// <param name="transceiverController">The transceiver controller used to populate modulation options.</param>
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

            List<string> modulationList = new();
            for (var i = 1; i < modulationListsMessageElements.Length - 1; i++)
            {
                modulationList.Add(modulationListsMessageElements[i]);
            }

            _transceiverController.ModulationsList = modulationList;
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
