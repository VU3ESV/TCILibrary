using System;
using System.Collections.Generic;
using System.Diagnostics;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciSpotCommand : ITciCommand, IDisposable
    {
        private TciSpotCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }

        public static TciSpotCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciSpotCommand(transceiverController);
        }

        public static string Name => "spot";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            //WriteOnlyCommand
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
