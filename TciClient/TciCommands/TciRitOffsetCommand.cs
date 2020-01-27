using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciRitOffsetCommand : ITciCommand, IDisposable
    {
        public TciRitOffsetCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnRitOffsetChanged += TransceiverController_OnRitOffsetChanged;
        }

        public static TciRitOffsetCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciRitOffsetCommand(transceiverController);
        }

        public static string Name => "rit_offset";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var ritOffsetMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(ritOffsetMessage))
            {
                return false;
            }

            var ritOffsetMessageElements = ritOffsetMessage.Split(':', ',', ';');
            if (ritOffsetMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(ritOffsetMessageElements[TransceiverIndex]);
            var ritOffset = Convert.ToInt32(ritOffsetMessageElements[RitOffsetIndex]);
            _transceiverController.RitOffset(transceiverPeriodicNumber, ritOffset);
            return true;
        }

        private void TransceiverController_OnRitOffsetChanged(object sender, Events.TrxIntValueChangedEventArgs e)
        {
            var transceiverPeriodicNumber = e.TransceiverPeriodicNumber;
            var ritOffset = e.Value;

            if (transceiverPeriodicNumber < _transceiverController.TrxCount)
            {
                _transceiverController.TciClient.SendMessageAsync($"{Name}:{transceiverPeriodicNumber},{ritOffset};");
            }
        }

        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnRitOffsetChanged -= TransceiverController_OnRitOffsetChanged;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int TransceiverIndex = 1;
        private const int RitOffsetIndex = 2;
        private const int CommandParameterCount = 4;
    }
}
