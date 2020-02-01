using System;
using ExpertElectronics.Tci.Events;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci
{
    public class Channel : IChannel
    {
        public Channel(uint periodicNumber, ITransceiver transceiver)
        {
            PeriodicNumber = periodicNumber;
            _transceiver = transceiver;
        }

        public uint PeriodicNumber { get; }
        public bool Enable
        {
            get => _enable;
            set
            {
                _enable = value;
                OnChannelEnableChanged?.Invoke(this, new ChannelEnableChangeEventArgs(_transceiver.PeriodicNumber, PeriodicNumber, _enable));
            }
        }
        public double IfFilter
        {
            get => _ifFilter;
            set
            {
                _ifFilter = value;
                OnIfFreqChanged?.Invoke(this, new IfFrequencyChangedEventArgs(_transceiver.PeriodicNumber, PeriodicNumber, _ifFilter));
            }
        }
        public int RxSMeter
        {
            get => _rxSMeter;
            set
            {
                _rxSMeter = value;
                OnChannelSMeterChanged?.Invoke(this, new ChannelSMeterChangeEventArgs(_transceiver.PeriodicNumber, PeriodicNumber, _rxSMeter));
            }
        }
        public long Vfo
        {
            get => _vfo;
            set
            {
                _vfo = value;
                OnVfoChange?.Invoke(this, new VfoChangeEventArgs(_transceiver.PeriodicNumber, PeriodicNumber, _vfo));
            }
        }

        public bool ReceiveOnly { get; set; }

        public event EventHandler<VfoChangeEventArgs> OnVfoChange;
        public event EventHandler<IfFrequencyChangedEventArgs> OnIfFreqChanged;
        public event EventHandler<ChannelSMeterChangeEventArgs> OnChannelSMeterChanged;
        public event EventHandler<ChannelEnableChangeEventArgs> OnChannelEnableChanged;

        private readonly ITransceiver _transceiver;

        private long _vfo;
        private double _ifFilter;
        private int _rxSMeter;
        private bool _enable;
    }
}