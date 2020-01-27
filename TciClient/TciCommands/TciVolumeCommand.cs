using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Events;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciVolumeCommand : ITciCommand, IDisposable
    {
        public TciVolumeCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnVolumeChanged += TransceiverControllerOnVolumeChanged;
        }

        private void TransceiverControllerOnVolumeChanged(object sender, IntValueChangedEventArgs e)
        {
            var volume = e.Value;
            if (volume >= -60 && volume <= 0)
            {
                _transceiverController.TciClient.SendMessageAsync($"{Name}:{volume};");
            }
        }

        public static TciVolumeCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciVolumeCommand(transceiverController);
        }

        public static string Name => "volume";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var volumeMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(volumeMessage))
            {
                return false;
            }

            var volumeMessageElements = volumeMessage.Split(':', ',', ';');
            if (volumeMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var volume = Convert.ToInt32(volumeMessageElements[VolumeIndex]);
            if (volume < -60 || volume > 0)
            {
                return false;
            }

            _transceiverController.Volume(volume);
            return true;
        }


        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnVolumeChanged -= TransceiverControllerOnVolumeChanged;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int VolumeIndex = 1;
        private const int CommandParameterCount = 3;
    }
}
