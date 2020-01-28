﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Events;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciIqSampleRateCommand : ITciCommand, IDisposable
    {
        public TciIqSampleRateCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnIqOutSampleRateChanged += TransceiverControllerOnIqOutSampleRateChanged;
        }

        private void TransceiverControllerOnIqOutSampleRateChanged(object sender, UintValueChangedEventArgs e)
        {
            var iqSampleRate = e.Value;
            if (iqSampleRate == 48000 || iqSampleRate == 96000 || iqSampleRate == 192000)
            {
                _transceiverController.TciClient.SendMessageAsync($"{Name}:{iqSampleRate};");
            }
        }

        public static TciIqSampleRateCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciIqSampleRateCommand(transceiverController);
        }

        public static string Name => "iq_samplerate";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var iqSampleRateMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(iqSampleRateMessage))
            {
                return false;
            }

            var iqSampleRateMessageElements = iqSampleRateMessage.Split(':', ',', ';');
            if (iqSampleRateMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var iqSampleRate = Convert.ToUInt32(iqSampleRateMessageElements[IqSampleRateIndex]);
            if (iqSampleRate != 48000 && iqSampleRate != 96000 && iqSampleRate != 192000)
            {
                return false;
            }

            _transceiverController.SetIqSampleRate(iqSampleRate);
            return true;
        }


        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnIqOutSampleRateChanged -= TransceiverControllerOnIqOutSampleRateChanged;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int IqSampleRateIndex = 1;
        private const int CommandParameterCount = 3;
    }
}
