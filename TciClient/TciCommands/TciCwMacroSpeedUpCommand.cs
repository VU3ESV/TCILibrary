using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Events;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciCwMacroSpeedUpCommand : ITciCommand, IDisposable
    {
        public static TciCwMacroSpeedUpCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciCwMacroSpeedUpCommand(transceiverController);
        }

        private TciCwMacroSpeedUpCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnCwMacroSpeedUp += TransceiverControllerOnOnCwMacroSpeedUp;
        }

        private async void TransceiverControllerOnOnCwMacroSpeedUp(object sender, UintValueChangedEventArgs e)
        {
            var increment = e.Value;
            await _transceiverController.TciClient.SendMessageAsync($"{Name}:{increment};");
        }

        public static string Name => "cw_macros_speed_up";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            if (!messages.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            // ToDo:
            // _transceiverController.CwMacroSpeedUp();
            return true;
        }

        public void Dispose()
        {
            if (_transceiverController != null)
            {
                _transceiverController.OnCwMacroSpeedUp -= TransceiverControllerOnOnCwMacroSpeedUp;
            }

            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
    }
}
