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
            var transceiver = _transceiverController.GeTransceiver(transceiverPeriodicNumber);
            if (transceiver != null)
            {
                transceiver.RitOffset = ritOffset;
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
        private const int RitOffsetIndex = 2;
        private const int CommandParameterCount = 4;
    }
}
