using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciVfoLimitsCommand : ITciCommand, IDisposable
    {
        private TciVfoLimitsCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }

        public static TciVfoLimitsCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciVfoLimitsCommand(transceiverController);
        }

        public static string Name => "vfo_limits";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var vfoLimitsMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(vfoLimitsMessage))
            {
                return false;
            }

            var vfoLimitsMessageElements = vfoLimitsMessage.Split(':', ',', ';');
            if (vfoLimitsMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            (_transceiverController as TransceiverController)?.VfoLimits(
                Convert.ToInt64(vfoLimitsMessageElements[MinIndex]),
                Convert.ToInt64(vfoLimitsMessageElements[MaxIndex]));

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
        private const int MaxIndex = 2;
        private const int MinIndex = 1;
        private const int CommandParameterCount = 4;
    }
}
