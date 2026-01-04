using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    /// <summary>
    /// Represents a Cw Macro speed up command for the TCI device.
    /// </summary>
    public class TciCwMacroSpeedUpCommand : ITciCommand, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TciCwMacroSpeedUpCommand"/> class.
        /// </summary>
        /// <param name="transceiverController">The transceiver controller used to update CW macro speed up state.</param>
        private TciCwMacroSpeedUpCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }
        public static TciCwMacroSpeedUpCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciCwMacroSpeedUpCommand(transceiverController);
        }



        public static string Name => "cw_macros_speed_up";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var cwMacroSpeedUpMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(cwMacroSpeedUpMessage))
            {
                return false;
            }

            var cwMacroSpeedUpMessageElements = cwMacroSpeedUpMessage.Split(':', ',', ';');
            if (cwMacroSpeedUpMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var cwMacroSpeed = Convert.ToUInt32(cwMacroSpeedUpMessageElements[CwMacroSpeedIndex]);
            _transceiverController.CwMacrosSpeedUp = cwMacroSpeed;
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
