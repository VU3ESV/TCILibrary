using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Events;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciAudioSampleRateCommand : ITciCommand, IDisposable
    {
        public TciAudioSampleRateCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnAudioSampleRateChanged += TransceiverControllerOnAudioSampleRateChanged;
        }

        private void TransceiverControllerOnAudioSampleRateChanged(object sender, UintValueChangedEventArgs e)
        {
            var audioSampleRate = e.Value;
            if (audioSampleRate == 8000 || audioSampleRate == 12000 || audioSampleRate == 24000 || audioSampleRate == 48000)
            {
                _transceiverController.TciClient.SendMessageAsync($"{Name}:{audioSampleRate};");
            }
        }

        public static TciAudioSampleRateCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciAudioSampleRateCommand(transceiverController);
        }

        public static string Name => "audio_samplerate";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var audioSampleRateMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(audioSampleRateMessage))
            {
                return false;
            }

            var audioSampleRateMessageElements = audioSampleRateMessage.Split(':', ',', ';');
            if (audioSampleRateMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var audioSampleRate = Convert.ToUInt32(audioSampleRateMessageElements[IqSampleRateIndex]);
            if (audioSampleRate != 8000 && audioSampleRate != 12000 && audioSampleRate != 24000 && audioSampleRate != 48000)
            {
                return false;
            }

            _transceiverController.IqSampleRate(audioSampleRate);
            return true;
        }


        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnAudioSampleRateChanged -= TransceiverControllerOnAudioSampleRateChanged;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int IqSampleRateIndex = 1;
        private const int CommandParameterCount = 3;
    }
}
