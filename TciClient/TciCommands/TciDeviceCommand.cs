using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciDeviceCommand : ITciCommand, IDisposable
    {
        private TciDeviceCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }

        public static TciDeviceCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciDeviceCommand(transceiverController);
        }

        public static string Name => "device";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var deviceMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(deviceMessage))
            {
                return false;
            }

            var deviceMessageElements = deviceMessage.Split(':', ',', ';');
            if (deviceMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var device = deviceMessageElements[DeviceIndex];
            _transceiverController.Device = device;
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
        private const int DeviceIndex = 1;
        private const int CommandParameterCount = 3;
    }
}
