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
        }

        public static string Name => "stop";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            if (!messages.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            _transceiverController.Stop = true;
            _transceiverController.Start = false;
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
    }
}
