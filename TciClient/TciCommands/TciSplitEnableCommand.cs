using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciSplitEnableCommand : ITciCommand, IDisposable
    {
        public TciSplitEnableCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnSplitEnableChanged += TransceiverController_OnSplitEnableChanged;
        }

        private void TransceiverController_OnSplitEnableChanged(object sender, Events.TrxEventArgs e)
        {
            var transceiverPeriodicNumber = e.TransceiverPeriodicNumber;
            var splitEnableState = e.State;

            if (transceiverPeriodicNumber < _transceiverController.TrxCount)
            {
                _transceiverController.TciClient.SendMessageAsync($"{Name}:{transceiverPeriodicNumber},{splitEnableState};");
            }
        }

        public static TciSplitEnableCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciSplitEnableCommand(transceiverController);
        }

        public static string Name => "split_enable";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var splitEnableMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(splitEnableMessage))
            {
                return false;
            }

            var splitEnableMessageElements = splitEnableMessage.Split(':', ',', ';');
            if (splitEnableMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(splitEnableMessageElements[TransceiverIndex]);
            var splitEnable = Convert.ToBoolean(splitEnableMessageElements[SplitEnableIndex]);
            _transceiverController.SplitEnable(transceiverPeriodicNumber, splitEnable);
            return true;
        }

        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnSplitEnableChanged -= TransceiverController_OnSplitEnableChanged;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int TransceiverIndex = 1;
        private const int SplitEnableIndex = 2;
        private const int CommandParameterCount = 4;
    }
}
