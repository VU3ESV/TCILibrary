using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciSpotCommand : ITciCommand, IDisposable
    {
        private TciSpotCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnSpot += TransceiverController_OnSpot;
        }

        private async void TransceiverController_OnSpot(object sender, Events.SpotEventArgs e)
        {
            var callSign = e.CallSign;
            var mode = e.Mode;
            var frequencyInHz = e.FrequencyInHz;
            var color = e.Color;
            var colorToUi = color.ToRgbString();
            var additionalText = e.AdditionalText;

            await _transceiverController.TciClient.SendMessageAsync($"{Name}:{callSign},{mode},{frequencyInHz},{colorToUi},{additionalText};");
        }

        public static TciSpotCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciSpotCommand(transceiverController);
        }

        public static string Name => "spot";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var spotMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(spotMessage))
            {
                return false;
            }

            var spotMessageElements = spotMessage.Split(':', ',', ';');
            if (spotMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var callSign = spotMessageElements[CallSignIndex];
            var mode = spotMessageElements[ModeIndex];
            var frequency = Convert.ToInt64(spotMessageElements[FrequencyIndex]);
            var color = Color.FromName(spotMessageElements[ColorIndex]);
            var additionalText = spotMessageElements[AddTextIndex];
            _transceiverController.Spot(callSign, mode, frequency, color, additionalText);
            return true;
        }

        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnSpot += TransceiverController_OnSpot;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int AddTextIndex = 5;
        private const int ColorIndex = 4;
        private const int FrequencyIndex = 3;
        private const int ModeIndex = 2;
        private const int CallSignIndex = 1;
        private const int CommandParameterCount = 7;
    }
}
