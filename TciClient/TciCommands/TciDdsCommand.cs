using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciDdsCommand : ITciCommand, IDisposable
    {
        public TciDdsCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnDdsFreqChanged += TransceiverController_OnDdsFreqChanged;
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
            _transceiverController.SetDdsFrequency(transceiverPeriodicNumber, ddsFrequency);
            return true;
        }

        private void TransceiverController_OnDdsFreqChanged(object sender, Events.TrxDoubleValueChangedEventArgs e)
        {
            var transceiverPeriodicNumber = e.TransceiverPeriodicNumber;
            var ddsFrequency = e.Value;

            if (transceiverPeriodicNumber < _transceiverController.TrxCount)
            {
                _transceiverController.TciClient.SendMessageAsync($"{Name}:{transceiverPeriodicNumber},{ddsFrequency};");
            }
        }

        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnDdsFreqChanged -= TransceiverController_OnDdsFreqChanged;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int TransceiverIndex = 1;
        private const int DdsFrequencyIndex = 2;
        private const int CommandParameterCount = 4;
    }
}
