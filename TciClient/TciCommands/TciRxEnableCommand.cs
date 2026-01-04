using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciRxEnableCommand : ITciCommand, IDisposable
    {
        public TciRxEnableCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }

        public static TciRxEnableCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciRxEnableCommand(transceiverController);
        }

        public static string Name => "rx_enable";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var rxEnableMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(rxEnableMessage))
            {
                return false;
            }

            var rxEnableMessageElements = rxEnableMessage.Split(':', ',', ';');
            if (rxEnableMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(rxEnableMessageElements[TransceiverIndex]);
            var rxEnable = Convert.ToBoolean(rxEnableMessageElements[RxEnableIndex]);
            var transceiver = _transceiverController.GetTransceiver(transceiverPeriodicNumber);
            if (transceiver != null)
            {
                transceiver.RxEnable = rxEnable;
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
        private const int RxEnableIndex = 2;
        private const int CommandParameterCount = 4;
    }
}
