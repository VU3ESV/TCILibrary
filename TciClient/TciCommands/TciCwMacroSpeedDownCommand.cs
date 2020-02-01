using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciCwMacroSpeedDownCommand : ITciCommand, IDisposable
    {
        public static TciCwMacroSpeedDownCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciCwMacroSpeedDownCommand(transceiverController);
        }

        private TciCwMacroSpeedDownCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }

        public static string Name => "cw_macros_speed_down";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var cwMacroSpeedDownMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(cwMacroSpeedDownMessage))
            {
                return false;
            }

            var cwMacroSpeedDownMessageElements = cwMacroSpeedDownMessage.Split(':', ',', ';');
            if (cwMacroSpeedDownMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var cwMacroSpeed = Convert.ToUInt32(cwMacroSpeedDownMessageElements[CwMacroSpeedIndex]);
            _transceiverController.CwMacrosSpeedDown = cwMacroSpeed;
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
        private const int CwMacroSpeedIndex = 1;
        private const int CommandParameterCount = 3;
    }
}
