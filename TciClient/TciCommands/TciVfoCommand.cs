using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciVfoCommand : ITciCommand, IDisposable
    {
        public TciVfoCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnVfoChange += TransceiverController_OnVfoChanged;
        }

        private void TransceiverController_OnVfoChanged(object sender, Events.VfoChangeEventArgs e)
        {
            var transceiverPeriodicNumber = e.TransceiverPeriodicNumber;
            var channel = e.Channel;
            var vfo = e.Vfo;

            if (transceiverPeriodicNumber >= _transceiverController.TrxCount)
            {
                return;
            }

            if (channel < _transceiverController.ChannelsCount)
            {
                _transceiverController.TciClient.SendMessageAsync($"{Name}:{transceiverPeriodicNumber},{channel},{vfo};");
            }
        }


        public static TciVfoCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciVfoCommand(transceiverController);
        }

        // ToDo: Need to check the name
        public static string Name => "vfo";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var vfoMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));           
            if (string.IsNullOrEmpty(vfoMessage))
            {
                return false;
            }

            var vfoMessageElements = vfoMessage.Split(':', ',', ';');
            if (vfoMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(vfoMessageElements[TransceiverIndex]);
            var channelNumber = Convert.ToUInt32(vfoMessageElements[ChannelIndex]);
            var vfo = Convert.ToInt32(vfoMessageElements[VfoIndex]);            
            _transceiverController.Vfo(transceiverPeriodicNumber, channelNumber, vfo);
            return true;
        }

        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnVfoChange -= TransceiverController_OnVfoChanged;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int TransceiverIndex = 1;
        private const int ChannelIndex = 2;
        private const int VfoIndex = 3;
        private const int CommandParameterCount = 5;
    }
}
