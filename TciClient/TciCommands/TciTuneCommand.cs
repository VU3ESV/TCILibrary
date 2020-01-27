using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciTuneCommand : ITciCommand, IDisposable
    {
        public TciTuneCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnTune += TransceiverController_OnTune;
        }

        private void TransceiverController_OnTune(object sender, Events.TrxEventArgs e)
        {
            var transceiverPeriodicNumber = e.TransceiverPeriodicNumber;
            var tuneState = e.State;

            if (transceiverPeriodicNumber < _transceiverController.TrxCount)
            {
                _transceiverController.TciClient.SendMessageAsync($"{Name}:{transceiverPeriodicNumber},{tuneState};");
            }
        }

        public static TciTuneCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciTuneCommand(transceiverController);
        }

        public static string Name => "tune";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var tuneMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(tuneMessage))
            {
                return false;
            }

            var tuneMessageElements = tuneMessage.Split(':', ',', ';');
            if (tuneMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(tuneMessageElements[TransceiverIndex]);
            var tune = Convert.ToBoolean(tuneMessageElements[TuneIndex]);
            _transceiverController.Tune(transceiverPeriodicNumber, tune);
            return true;
        }

        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnTune -= TransceiverController_OnTune;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int TransceiverIndex = 1;
        private const int TuneIndex = 2;
        private const int CommandParameterCount = 4;
    }
}
