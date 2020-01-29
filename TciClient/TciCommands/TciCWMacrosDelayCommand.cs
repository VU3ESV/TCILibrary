using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Events;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciCwMacrosDelayCommand : ITciCommand, IDisposable
    {
        public TciCwMacrosDelayCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnCwMacrosDelayChanged += TransceiverControllerOnOnCwDelayChanged;
        }

        private void TransceiverControllerOnOnCwDelayChanged(object sender, UintValueChangedEventArgs e)
        {
            var cwDelay = e.Value;
            _transceiverController.TciClient.SendMessageAsync($"{Name}:{cwDelay};");
        }

        public static TciCwMacrosDelayCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciCwMacrosDelayCommand(transceiverController);
        }

        public static string Name => "cw_macros_delay";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var cwDelayMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(cwDelayMessage))
            {
                return false;
            }

            var cwDelayElements = cwDelayMessage.Split(':', ',', ';');
            if (cwDelayElements.Length != CommandParameterCount)
            {
                return false;
            }

            var cwDelay = Convert.ToUInt32(cwDelayElements[CwDelayIndex]);
            _transceiverController.SetCwMacrosDelay(cwDelay);
            return true;
        }


        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnCwSpeedChanged -= TransceiverControllerOnOnCwDelayChanged;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int CwDelayIndex = 1;
        private const int CommandParameterCount = 3;
    }
}
