using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciReadyCommand : ITciCommand, IDisposable
    {
        public static TciReadyCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciReadyCommand(transceiverController);
        }

        private TciReadyCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }

        public static string Name => "ready";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            if (!messages.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            _transceiverController.Ready = true;
            return true;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
    }
}
