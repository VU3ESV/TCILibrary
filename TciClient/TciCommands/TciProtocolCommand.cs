using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciProtocolCommand : ITciCommand, IDisposable
    {
        public static TciProtocolCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciProtocolCommand(transceiverController);
        }

        private TciProtocolCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }

        public static string Name => "protocol";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var protocolMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(protocolMessage))
            {
                return false;
            }

            var protocolMessageElements = protocolMessage.Split(':', ',', ';');
            if (protocolMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            _transceiverController.SoftwareName = protocolMessageElements[SoftwareNameIndex];
            _transceiverController.SoftwareVersion = protocolMessageElements[VersionIndex];
            return true;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const uint CommandParameterCount = 4;
        private const uint SoftwareNameIndex = 1;
        private const uint VersionIndex = 2;
    }
}
