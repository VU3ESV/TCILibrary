using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciChannelCountCommand : ITciCommand, IDisposable
    {
        private TciChannelCountCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }

        public static TciChannelCountCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciChannelCountCommand(transceiverController);
        }

        public static string Name => "channels_count";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var channelCountMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(channelCountMessage))
            {
                return false;
            }

            var channelCountMessageElements = channelCountMessage.Split(':', ',', ';');
            if (channelCountMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var channelCount = Convert.ToUInt32(channelCountMessageElements[TransceiverIndex]);
            (_transceiverController as TransceiverController).CreateChannel(channelCount);
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
        private const int TransceiverIndex = 1;
        private const int CommandParameterCount = 3;
    }
}
