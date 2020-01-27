using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciStartCommand : ITciCommand, IDisposable
    {
        public static TciStartCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciStartCommand(transceiverController);
        }

        private TciStartCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnStarted += TransceiverController_OnStarted;
        }

        private async void TransceiverController_OnStarted(object sender, EventArgs e)
        {
            if (_transceiverController.IsStarted())
            {
                return;
            }

            await _transceiverController.TciClient.SendMessageAsync("start;");
        }

        public static string Name => "start";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            if (!messages.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            _transceiverController.Start();
            return true;
        }

        public void Dispose()
        {
            if (_transceiverController != null)
            {
                _transceiverController.OnStarted -= TransceiverController_OnStarted;
            }

            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
    }
}
