using System;
using System.Collections.Generic;
using System.Diagnostics;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciSpotClearCommand : ITciCommand, IDisposable
    {
        public TciSpotClearCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnSpotClear += TransceiverController_OnSpotClear;
        }

        private void TransceiverController_OnSpotClear(object sender, Events.TrxEventArgs e)
        {
            _transceiverController.TciClient.SendMessageAsync($"{Name};");
        }

        public static TciSpotClearCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciSpotClearCommand(transceiverController);
        }

        public static string Name => "spot_clear";

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

            _transceiverController.OnSpotClear -= TransceiverController_OnSpotClear;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
    }
}
