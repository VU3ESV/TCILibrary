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
            var transceiver = _transceiverController.GetTransceiver(transceiverPeriodicNumber);
            if (transceiver != null)
            {
                transceiver.XitOffset = xitOffset;
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
