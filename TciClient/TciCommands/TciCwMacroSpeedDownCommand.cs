using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Events;
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
            _transceiverController.OnCwMacroSpeedDown += TransceiverControllerOnOnCwMacroSpeedDown;
        }

        private async void TransceiverControllerOnOnCwMacroSpeedDown(object sender, UintValueChangedEventArgs e)
        {
            var decrement = e.Value;
            await _transceiverController.TciClient.SendMessageAsync($"{Name}:{decrement};");
        }

        public static string Name => "cw_macros_speed_down";

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
                _transceiverController.OnCwMacroSpeedUp -= TransceiverControllerOnOnCwMacroSpeedDown;
            }

            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
    }
}
