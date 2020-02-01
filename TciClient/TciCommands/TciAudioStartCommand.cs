using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciAudioStartCommand : ITciCommand, IDisposable
    {
        public TciAudioStartCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }       

        public static TciAudioStartCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciAudioStartCommand(transceiverController);
        }

        public static string Name => "audio_start";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var audioStartMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(audioStartMessage))
            {
                return false;
            }

            var audioStartMessageElements = audioStartMessage.Split(':', ',', ';');
            if (audioStartMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(audioStartMessageElements[TransceiverIndex]);
            var transceiver = _transceiverController.GeTransceiver(transceiverPeriodicNumber);
            if (transceiver != null)
            {
                transceiver.AudioEnable = true;
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
