using System;
using System.Collections.Generic;
using System.Diagnostics;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciSpotDeleteCommand : ITciCommand, IDisposable
    {
        public TciSpotDeleteCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnSpotDelete += TransceiverController_OnSpotDelete;
        }

        private void TransceiverController_OnSpotDelete(object sender, Events.StringValueChangedEventArgs e)
        {
            var spot = e.Value;

            if (string.IsNullOrEmpty(spot))
            {
                return;
            }

            _transceiverController.TciClient.SendMessageAsync($"{Name}:{spot};");
        }

        public static TciSpotDeleteCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciSpotDeleteCommand(transceiverController);
        }

        public static string Name => "spot_delete";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            //ToDo
            return true;
        }

        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnSpotDelete -= TransceiverController_OnSpotDelete;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int TransceiverIndex = 1;
        private const int CommandParameterCount = 3;
    }
}
