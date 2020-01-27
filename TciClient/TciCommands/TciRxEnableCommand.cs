using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciRxEnableCommand : ITciCommand, IDisposable
    {
        public TciRxEnableCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnRxEnableChanged += TransceiverController_OnRxEnableChanged;
        }

        private void TransceiverController_OnRxEnableChanged(object sender, Events.TrxEventArgs e)
        {
            var transceiverPeriodicNumber = e.TransceiverPeriodicNumber;
            var rxEnableState = e.State;

            if (transceiverPeriodicNumber < _transceiverController.TrxCount)
            {
                _transceiverController.TciClient.SendMessageAsync($"{Name}:{transceiverPeriodicNumber},{rxEnableState};");
            }
        }

        public static TciRxEnableCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciRxEnableCommand(transceiverController);
        }

        public static string Name => "rx_enable";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var rxEnableMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(rxEnableMessage))
            {
                return false;
            }

            var rxEnableMessageElements = rxEnableMessage.Split(':', ',', ';');
            if (rxEnableMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(rxEnableMessageElements[TransceiverIndex]);
            var rxEnable = Convert.ToBoolean(rxEnableMessageElements[RxEnableIndex]);
            _transceiverController.RxEnable(transceiverPeriodicNumber, rxEnable);
            return true;
        }

        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnRxEnableChanged -= TransceiverController_OnRxEnableChanged;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int TransceiverIndex = 1;
        private const int RxEnableIndex = 2;
        private const int CommandParameterCount = 4;
    }
}
