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
        }

        public static TciSpotDeleteCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciSpotDeleteCommand(transceiverController);
        }

        public static string Name => "spot_delete";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            // WriteOnlyCommand
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
