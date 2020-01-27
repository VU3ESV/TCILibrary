using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciMuteCommand : ITciCommand, IDisposable
    {
        public TciMuteCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnMute += TransceiverController_OnMute;
        }

        private void TransceiverController_OnMute(object sender, Events.TrxEventArgs e)
        {
            var mute = e.State ? "true" : "false";
            _transceiverController.TciClient.SendMessageAsync($"{Name}:{mute};");
        }

        public static TciMuteCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciMuteCommand(transceiverController);
        }

        public static string Name => "mute";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var muteMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(muteMessage))
            {
                return false;
            }

            var muteMessageElements = muteMessage.Split(':', ',', ';');
            if (muteMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var mute = Convert.ToBoolean(muteMessageElements[MuteIndex]);
            _transceiverController.Mute(mute);
            return true;
        }

        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnMute -= TransceiverController_OnMute;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int MuteIndex = 1;
        private const int CommandParameterCount = 3;
    }
}
