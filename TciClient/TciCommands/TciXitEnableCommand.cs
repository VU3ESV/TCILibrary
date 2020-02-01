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
            var transceiver = _transceiverController.GeTransceiver(transceiverPeriodicNumber);
            if (transceiver != null)
            {
                transceiver.Xit = xitEnable;
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
        private const int XitEnableIndex = 2;
        private const int CommandParameterCount = 4;
    }
}
