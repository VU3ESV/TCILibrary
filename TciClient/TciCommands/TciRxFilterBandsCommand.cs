using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciRxFilterBandsCommand : ITciCommand, IDisposable
    {
        private TciRxFilterBandsCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnRxFilterChanged += TransceiverController_OnRxFilterChanged;
        }

        private void TransceiverController_OnRxFilterChanged(object sender, Events.RxFilterChangedEventArgs e)
        {
            var transceiverPeriodicNumber = Convert.ToUInt32(e.TransceiverPeriodicNumber);
            var low = e.Low;
            var high = e.High;
            _transceiverController.RxFilter(transceiverPeriodicNumber, low, high);
        }

        public static TciRxFilterBandsCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciRxFilterBandsCommand(transceiverController);
        }

        public static string Name => "rx_filter_band";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var rxFilterMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(rxFilterMessage))
            {
                return false;
            }

            var rxFilterMessageElements = rxFilterMessage.Split(':', ',', ';');
            if (rxFilterMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var receiverPeriodicNumber = Convert.ToUInt32(rxFilterMessageElements[ReceiverIndex]);
            var maxLimit = Convert.ToInt32(rxFilterMessageElements[MaxIndex]);
            var minLimit = Convert.ToInt32(rxFilterMessageElements[MinIndex]);
            _transceiverController.RxFilter(receiverPeriodicNumber, minLimit, maxLimit);
            return true;
        }

        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnRxFilterChanged -= TransceiverController_OnRxFilterChanged;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int MaxIndex = 3;
        private const int MinIndex = 2;
        private const int ReceiverIndex = 1;
        private const int CommandParameterCount = 5;
    }
}
