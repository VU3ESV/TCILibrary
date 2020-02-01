using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciFootSwitchCommand : ITciCommand, IDisposable
    {
        private TciFootSwitchCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }

        public static TciFootSwitchCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciFootSwitchCommand(transceiverController);
        }

        public static string Name => "tx_footswitch";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var txFootSwitchMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(txFootSwitchMessage))
            {
                return false;
            }

            var txFootSwitchMessageElements = txFootSwitchMessage.Split(':', ',', ';');
            if (txFootSwitchMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(txFootSwitchMessageElements[TransceiverIndex]);
            var footSwitchStatus = Convert.ToBoolean(txFootSwitchMessageElements[FootSwitchIndex]);

            var transceiver = _transceiverController.GeTransceiver(transceiverPeriodicNumber);
            if (transceiver != null)
            {
                transceiver.TxFootSwitch = footSwitchStatus;
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
        private const int FootSwitchIndex = 2;
        private const int TransceiverIndex = 1;
        private const int CommandParameterCount = 5;
    }
}
