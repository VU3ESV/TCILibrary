using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciRitEnableCommand : ITciCommand, IDisposable
    {
        public TciRitEnableCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }       

        public static TciRitEnableCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciRitEnableCommand(transceiverController);
        }

        public static string Name => "rit_enable";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var ritEnableMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(ritEnableMessage))
            {
                return false;
            }

            var ritEnableMessageElements = ritEnableMessage.Split(':', ',', ';');
            if (ritEnableMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(ritEnableMessageElements[TransceiverIndex]);
            var ritEnable = Convert.ToBoolean(ritEnableMessageElements[RitEnableIndex]);
            var transceiver = _transceiverController.GeTransceiver(transceiverPeriodicNumber);
            if (transceiver != null)
            {
                transceiver.Rit = ritEnable;
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
        private const int RitEnableIndex = 2;
        private const int CommandParameterCount = 4;
    }
}
