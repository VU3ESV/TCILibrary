using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciRxChannelEnableCommand : ITciCommand, IDisposable
    {
        public TciRxChannelEnableCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }

        public static TciRxChannelEnableCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciRxChannelEnableCommand(transceiverController);
        }

        public static string Name => "rx_channel_enable";

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
            var channelNumber = Convert.ToUInt32(rxEnableMessageElements[ChannelIndex]);
            var rxChannelEnable = Convert.ToBoolean(rxEnableMessageElements[RxChannelEnableIndex]);
            var transceiver = _transceiverController.GetTransceiver(transceiverPeriodicNumber);
            var channel = transceiver?.Channels?.FirstOrDefault(_ => _.PeriodicNumber == channelNumber);
            if (channel != null)
            {
                channel.Enable = rxChannelEnable;
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
        private const int ChannelIndex = 2;
        private const int RxChannelEnableIndex = 3;
        private const int CommandParameterCount = 5;
    }
}
