using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciStopCommand : ITciCommand, IDisposable
    {
        public static TciStopCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciStopCommand(transceiverController);
        }

        private TciStopCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnStopped += TransceiverController_OnStopped;
        }

        private void TransceiverController_OnStopped(object sender, EventArgs e)
        {
            if (!_transceiverController.IsStarted())
            {
                return;
            }

            _transceiverController.TciClient.SendMessageAsync("stop;");
        }

        public static string Name => "stop";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            if (!messages.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            _transceiverController.Stop();
            return true;
        }

        public void Dispose()
        {
            if (_transceiverController != null)
            {
                _transceiverController.OnStopped += TransceiverController_OnStopped;
            }

            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
    }
}
