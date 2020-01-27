using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciModulationCommand : ITciCommand, IDisposable
    {
        public TciModulationCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnModulationChanged += TransceiverController_OnModulationChanged;
        }

        private void TransceiverController_OnModulationChanged(object sender, Events.TrxStringValueChangedEventArgs e)
        {
            var transceiverPeriodicNumber = e.TransceiverPeriodicNumber;
            var modulation = e.Value;

            if (transceiverPeriodicNumber >= _transceiverController.TrxCount)
            {
                return;
            }
            if (!_transceiverController.ModulationsList.Contains(modulation))
            {
                return;
            }

            _transceiverController.TciClient.SendMessageAsync($"{Name}:{transceiverPeriodicNumber},{modulation};");
        }


        public static TciModulationCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciModulationCommand(transceiverController);
        }

        public static string Name => "modulation";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {

            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var modulationMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(modulationMessage))
            {
                return false;
            }

            var modulationMessageElements = modulationMessage.Split(':', ',', ';');
            if (modulationMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(modulationMessageElements[TransceiverIndex]);

            var modulation = modulationMessageElements[ModulationIndex];
            _transceiverController.Modulation(transceiverPeriodicNumber, modulation);
            return true;
        }

        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnModulationChanged -= TransceiverController_OnModulationChanged;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int TransceiverIndex = 1;
        private const int ModulationIndex = 2;
        private const int CommandParameterCount = 4;
    }
}
