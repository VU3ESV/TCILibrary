namespace ExpertElectronics.Tci.Interfaces;

public interface IChannel
{
    bool Enable { get; set; }

    uint PeriodicNumber { get; }

    double IfFilter { get; set; }

    int RxSMeter { get; set; }

    long Vfo { get; set; }

    bool ReceiveOnly { get; set; }

    bool VfoLock { get; set; }

    int RxVolume { get; set; }

    int RxBalance { get; set; }

    double RxSignalLevelDbm { get; set; }

    event EventHandler<VfoChangeEventArgs> OnVfoChange;
    event EventHandler<IfFrequencyChangedEventArgs> OnIfFreqChanged;
    event EventHandler<ChannelSMeterChangeEventArgs> OnChannelSMeterChanged;
    event EventHandler<ChannelEnableChangeEventArgs> OnChannelEnableChanged;
    event EventHandler<ChannelStateChangedEventArgs> OnVfoLockChanged;
    event EventHandler<ChannelIntValueChangedEventArgs> OnRxVolumeChanged;
    event EventHandler<ChannelIntValueChangedEventArgs> OnRxBalanceChanged;
    event EventHandler<ChannelDoubleValueChangedEventArgs> OnRxSignalLevelChanged;
}
