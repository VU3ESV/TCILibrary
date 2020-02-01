using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciTrxCommand : ITciCommand, IDisposable
    {
        private TciTrxCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }

        public static TciTrxCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciTrxCommand(transceiverController);
        }

        public static string Name => "trx";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var trxMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(trxMessage))
            {
                return false;
            }

            var trxMessageElements = trxMessage.Split(':', ',', ';');
            if (trxMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(trxMessageElements[TransceiverIndex]);
            var state = Convert.ToBoolean(trxMessageElements[StateIndex]);
            var transceiver = _transceiverController.GeTransceiver(transceiverPeriodicNumber);
            if (transceiver != null)
            {
                transceiver.Trx = state;
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
        private const int StateIndex = 2;
        private const int CommandParameterCount = 4;
    }
}
