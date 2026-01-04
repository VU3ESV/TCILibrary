using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciRxMuteCommand : ITciCommand, IDisposable
    {
        public TciRxMuteCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }

        public static TciRxMuteCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciRxMuteCommand(transceiverController);
        }

        public static string Name => "rx_mute";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var rxMuteMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(rxMuteMessage))
            {
                return false;
            }

            var rxMuteMessageElements = rxMuteMessage.Split(':', ',', ';');
            if (rxMuteMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(rxMuteMessageElements[TransceiverIndex]);
            var rxMute = Convert.ToBoolean(rxMuteMessageElements[RxMuteIndex]);
            var transceiver = _transceiverController.GetTransceiver(transceiverPeriodicNumber);
            if (transceiver != null)
            {
                transceiver.RxMute = rxMute;
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
        private const int RxMuteIndex = 2;
        private const int CommandParameterCount = 4;
    }
}
