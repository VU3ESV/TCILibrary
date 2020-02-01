using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciAudioStopCommand : ITciCommand, IDisposable
    {
        public TciAudioStopCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }
                
        public static TciAudioStopCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciAudioStopCommand(transceiverController);
        }

        public static string Name => "audio_stop";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var audioStopMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(audioStopMessage))
            {
                return false;
            }

            var audioStopMessageElements = audioStopMessage.Split(':', ',', ';');
            if (audioStopMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(audioStopMessageElements[TransceiverIndex]);
            var transceiver = _transceiverController.GeTransceiver(transceiverPeriodicNumber);
            if (transceiver != null)
            {
                transceiver.AudioEnable = false;
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
        private const int CommandParameterCount = 3;
    }
}
