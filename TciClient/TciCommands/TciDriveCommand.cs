using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Events;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciDriveCommand : ITciCommand, IDisposable
    {
        public TciDriveCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnDrive += TransceiverControllerOnDriveChanged;
        }

        private void TransceiverControllerOnDriveChanged(object sender, UintValueChangedEventArgs e)
        {
            var drive = e.Value;
            _transceiverController.TciClient.SendMessageAsync($"{Name}:{drive};");
        }

        public static TciDriveCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciDriveCommand(transceiverController);
        }

        public static string Name => "drive";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var driveMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(driveMessage))
            {
                return false;
            }

            var driveMessageElements = driveMessage.Split(':', ',', ';');
            if (driveMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var drive = Convert.ToUInt32(driveMessageElements[DriveIndex]);
            _transceiverController.SetDrive(drive);
            return true;
        }


        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnDrive -= TransceiverControllerOnDriveChanged;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int DriveIndex = 1;
        private const int CommandParameterCount = 3;
    }
}
