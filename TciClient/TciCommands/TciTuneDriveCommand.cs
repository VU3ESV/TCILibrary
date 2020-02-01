using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciTuneDriveCommand : ITciCommand, IDisposable
    {
        public TciTuneDriveCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }

        public static TciTuneDriveCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciTuneDriveCommand(transceiverController);
        }

        public static string Name => "tune_drive";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var tuneDriveMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(tuneDriveMessage))
            {
                return false;
            }

            var tuneDriveMessageElements = tuneDriveMessage.Split(':', ',', ';');
            if (tuneDriveMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var tuneDrive = Convert.ToUInt32(tuneDriveMessageElements[TuneDriveIndex]);
            _transceiverController.TuneDrive = tuneDrive;
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
        private const int TuneDriveIndex = 1;
        private const int CommandParameterCount = 3;
    }
}
