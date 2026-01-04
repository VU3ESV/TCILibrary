using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    /// <summary>
    /// Represents a DDS command for the TCI device.
    /// </summary>
    public class TciDdsCommand : ITciCommand, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TciDdsCommand"/> class.
        /// </summary>
        /// <param name="transceiverController">The transceiver controller used to update DDS state.</param>
        public TciDdsCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }

        public static TciDdsCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciDdsCommand(transceiverController);
        }

        public static string Name => "dds";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var ddsMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(ddsMessage))
            {
                return false;
            }

            var ddsMessageElements = ddsMessage.Split(':', ',', ';');
            if (ddsMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(ddsMessageElements[TransceiverIndex]);
            var ddsFrequency = Convert.ToInt64(ddsMessageElements[DdsFrequencyIndex]);
            var transceiver = _transceiverController.GetTransceiver(transceiverPeriodicNumber);
            if (transceiver != null)
            {
                transceiver.DdsFrequency = ddsFrequency;
            }
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
        private const int TransceiverIndex = 1;
        private const int DdsFrequencyIndex = 2;
        private const int CommandParameterCount = 4;
    }
}
