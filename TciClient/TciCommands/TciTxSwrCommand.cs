using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciTxSwrCommand : ITciCommand, IDisposable
    {
        public static TciTxSwrCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciTxSwrCommand(transceiverController);
        }

        private TciTxSwrCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }


        public static string Name => "tx_swr";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var txSwrMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(txSwrMessage))
            {
                return false;
            }

            var txSwrMessageElements = txSwrMessage.Split(':', ',', ';');
            if (txSwrMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            _transceiverController.TxSwr = (float)Convert.ToDouble(txSwrMessageElements[TxPowerIndex]);
            return true;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const uint CommandParameterCount = 2;
        private const uint TxPowerIndex = 1;
    }
}
