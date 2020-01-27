using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciTxEnableCommand : ITciCommand, IDisposable
    {
        public TciTxEnableCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }

        public static TciTxEnableCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciTxEnableCommand(transceiverController);
        }

        public static string Name => "tx_enable";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var txEnableMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(txEnableMessage))
            {
                return false;
            }

            var txEnableMessageElements = txEnableMessage.Split(':', ',', ';');
            if (txEnableMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(txEnableMessageElements[TransceiverIndex]);
            var txEnable = Convert.ToBoolean(txEnableMessageElements[TxEnableIndex]);
            _transceiverController.TxEnable(transceiverPeriodicNumber, txEnable);
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
        private const int TxEnableIndex = 2;
        private const int CommandParameterCount = 4;
    }
}
