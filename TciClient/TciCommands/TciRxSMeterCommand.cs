using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciRxSMeterCommand : ITciCommand, IDisposable
    {
        public TciRxSMeterCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnChannelSMeterChanged += TransceiverController_OnChannelSMeterChanged;
        }

        private void TransceiverController_OnChannelSMeterChanged(object sender, Events.ChannelSMeterChangeEventArgs e)
        {
            var transceiverPeriodicNumber = e.TransceiverPeriodicNumber;
            var channel = e.Channel;
            var rxSMeter = e.SMeter;

            if (transceiverPeriodicNumber >= _transceiverController.TrxCount)
            {
                return;
            }

            if (channel < _transceiverController.ChannelsCount)
            {
                _transceiverController.TciClient.SendMessageAsync($"{Name}:{transceiverPeriodicNumber},{channel},{rxSMeter};");
            }
        }


        public static TciRxSMeterCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciRxSMeterCommand(transceiverController);
        }

        // ToDo: Need to check the name
        public static string Name => "rx_smeter";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var rxSMeterMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(rxSMeterMessage))
            {
                return false;
            }

            var rxSMeterMessageElements = rxSMeterMessage.Split(':', ',', ';');
            if (rxSMeterMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(rxSMeterMessageElements[TransceiverIndex]);
            var channelNumber = Convert.ToUInt32(rxSMeterMessageElements[ChannelIndex]);
            var rxSMeter = Convert.ToInt32(rxSMeterMessageElements[RxChannelEnableIndex]);
            _transceiverController.RxSMeter(transceiverPeriodicNumber, channelNumber, rxSMeter);
            return true;
        }

        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnChannelSMeterChanged -= TransceiverController_OnChannelSMeterChanged;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int TransceiverIndex = 1;
        private const int ChannelIndex = 2;
        private const int RxChannelEnableIndex = 3;
        private const int CommandParameterCount = 5;
    }
}
