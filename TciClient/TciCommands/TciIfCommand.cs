using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;


namespace ExpertElectronics.Tci.TciCommands
{
    public class TciIfCommand : ITciCommand, IDisposable
    {
        public TciIfCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnIfFreqChanged += TransceiverController_OnIfFreqChanged;
        }

        public static TciIfCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciIfCommand(transceiverController);
        }

        public static string Name => "if";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var ifMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(ifMessage))
            {
                return false;
            }

            var ifMessageElements = ifMessage.Split(':', ',', ';');
            if (ifMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(ifMessageElements[TransceiverIndex]);
            var channelNumber = Convert.ToUInt32(ifMessageElements[ChannelIndex]);
            var ifFrequency = Convert.ToInt64(ifMessageElements[IfFrequencyIndex]);
            _transceiverController.IfFilter(transceiverPeriodicNumber, channelNumber, ifFrequency);
            return true;
        }

        private void TransceiverController_OnIfFreqChanged(object sender, Events.IfFrequencyChangedEventArgs e)
        {
            var transceiverPeriodicNumber = e.TransceiverPeriodicNumber;
            var channel = e.Channel;
            var ifFrequency = e.Value;

            if (transceiverPeriodicNumber >= _transceiverController.TrxCount)
            {
                return;
            }

            if (channel < _transceiverController.ChannelsCount)
            {
                _transceiverController.TciClient.SendMessageAsync($"{Name}:{transceiverPeriodicNumber},{channel},{ifFrequency};");
            }
        }

        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnIfFreqChanged -= TransceiverController_OnIfFreqChanged;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int TransceiverIndex = 1;
        private const int ChannelIndex = 2;
        private const int IfFrequencyIndex = 3;
        private const int CommandParameterCount = 5;
    }
}


