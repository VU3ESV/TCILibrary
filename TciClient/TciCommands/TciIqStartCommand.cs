using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciIqStartCommand : ITciCommand, IDisposable
    {
        public TciIqStartCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }
               
        public static TciIqStartCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciIqStartCommand(transceiverController);
        }

        public static string Name => "iq_start";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var iqStartMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(iqStartMessage))
            {
                return false;
            }

            var iqStartMessageElements = iqStartMessage.Split(':', ',', ';');
            if (iqStartMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(iqStartMessageElements[TransceiverIndex]);
            var transceiver = _transceiverController.GeTransceiver(transceiverPeriodicNumber);
            if (transceiver != null)
            {
                transceiver.IqEnable = true;
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
        private const int CommandParameterCount = 3;
    }
}
