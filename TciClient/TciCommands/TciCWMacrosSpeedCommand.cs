using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciCwMacrosSpeedCommand : ITciCommand, IDisposable
    {
        public TciCwMacrosSpeedCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }

        public static TciCwMacrosSpeedCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciCwMacrosSpeedCommand(transceiverController);
        }

        public static string Name => "cw_macros_speed";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var cwSpeedMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(cwSpeedMessage))
            {
                return false;
            }

            var cwSpeedElements = cwSpeedMessage.Split(':', ',', ';');
            if (cwSpeedElements.Length != CommandParameterCount)
            {
                return false;
            }

            var cwSpeed = Convert.ToUInt32(cwSpeedElements[CwSpeedIndex]);
            _transceiverController.CwMacroSpeed = cwSpeed;
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
        private const int CwSpeedIndex = 1;
        private const int CommandParameterCount = 3;
    }
}
