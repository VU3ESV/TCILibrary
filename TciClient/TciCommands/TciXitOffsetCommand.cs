using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciXitOffsetCommand : ITciCommand, IDisposable
    {
        public TciXitOffsetCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnRitOffsetChanged += TransceiverController_OnXitOffsetChanged;
        }

        public static TciXitOffsetCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciXitOffsetCommand(transceiverController);
        }

        public static string Name => "xit_offset";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var xitOffsetMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(xitOffsetMessage))
            {
                return false;
            }

            var xitOffsetMessageElements = xitOffsetMessage.Split(':', ',', ';');
            if (xitOffsetMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(xitOffsetMessageElements[TransceiverIndex]);
            var xitOffset = Convert.ToInt32(xitOffsetMessageElements[RitOffsetIndex]);
            _transceiverController.XitOffset(transceiverPeriodicNumber, xitOffset);
            return true;
        }

        private void TransceiverController_OnXitOffsetChanged(object sender, Events.TrxIntValueChangedEventArgs e)
        {
            var transceiverPeriodicNumber = e.TransceiverPeriodicNumber;
            var xitOffset = e.Value;

            if (transceiverPeriodicNumber < _transceiverController.TrxCount)
            {
                _transceiverController.TciClient.SendMessageAsync($"{Name}:{transceiverPeriodicNumber},{xitOffset};");
            }
        }

        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnRitOffsetChanged -= TransceiverController_OnXitOffsetChanged;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int TransceiverIndex = 1;
        private const int RitOffsetIndex = 2;
        private const int CommandParameterCount = 4;
    }
}
