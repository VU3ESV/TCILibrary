using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciIqStopCommand : ITciCommand, IDisposable
    {
        public TciIqStopCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnIqStopChanged += TransceiverController_OnIqStop;
        }

        private void TransceiverController_OnIqStop(object sender, Events.UintValueChangedEventArgs e)
        {
            var transceiverPeriodicNumber = e.Value;

            if (transceiverPeriodicNumber < _transceiverController.TrxCount)
            {
                _transceiverController.TciClient.SendMessageAsync($"{Name}:{transceiverPeriodicNumber};");
            }
        }

        public static TciIqStopCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciIqStopCommand(transceiverController);
        }

        public static string Name => "iq_stop";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var iqStopMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(iqStopMessage))
            {
                return false;
            }

            var iqStopMessageElements = iqStopMessage.Split(':', ',', ';');
            if (iqStopMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(iqStopMessageElements[TransceiverIndex]);
            _transceiverController.IqStop(transceiverPeriodicNumber);
            return true;
        }

        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnIqStartChanged -= TransceiverController_OnIqStop;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int TransceiverIndex = 1;
        private const int CommandParameterCount = 3;
    }
}
