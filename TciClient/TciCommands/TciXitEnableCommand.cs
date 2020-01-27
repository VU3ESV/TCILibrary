using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciXitEnableCommand : ITciCommand, IDisposable
    {
        public TciXitEnableCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnXitEnableChanged += TransceiverController_OnXitEnableChanged;
        }

        private void TransceiverController_OnXitEnableChanged(object sender, Events.TrxEventArgs e)
        {
            var transceiverPeriodicNumber = e.TransceiverPeriodicNumber;
            var xitEnableState = e.State;

            if (transceiverPeriodicNumber < _transceiverController.TrxCount)
            {
                _transceiverController.TciClient.SendMessageAsync($"{Name}:{transceiverPeriodicNumber},{xitEnableState};");
            }
        }

        public static TciXitEnableCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciXitEnableCommand(transceiverController);
        }

        public static string Name => "xit_enable";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var xitEnableMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(xitEnableMessage))
            {
                return false;
            }

            var xitEnableMessageElements = xitEnableMessage.Split(':', ',', ';');
            if (xitEnableMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(xitEnableMessageElements[TransceiverIndex]);
            var xitEnable = Convert.ToBoolean(xitEnableMessageElements[XitEnableIndex]);
            _transceiverController.XitEnable(transceiverPeriodicNumber, xitEnable);
            return true;
        }

        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnXitEnableChanged -= TransceiverController_OnXitEnableChanged;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int TransceiverIndex = 1;
        private const int XitEnableIndex = 2;
        private const int CommandParameterCount = 4;
    }
}
