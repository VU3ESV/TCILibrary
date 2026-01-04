namespace ExpertElectronics.Tci.Interfaces;

public interface ITransceiver
{
    uint PeriodicNumber { get; }

    bool TxEnable { get; set; }

    bool TxFootSwitch { get; set; }

    double DdsFrequency { get; set; }

    IEnumerable<Channel> Channels { get; }

    bool Rit { get; set; }

    int RitOffset { get; set; }

    string Modulation { get; set; }

    bool RxEnable { get; set; }

    bool Xit { get; set; }

    int XitOffset { get; set; }

    bool Split { get; set; }

    int RxFilterLowLimit { get; set; }

    int RxFilterHighLimit { get; set; }

    bool Trx { get; set; }

    bool Tune { get; set; }

    bool IqEnable { get; set; }

    bool AudioEnable { get; set; }

    bool Squelch { get; set; }

    int SquelchThreshold { get; set; }

    bool RxMute { get; set; }

    void AddChannel(uint channelNumbers);

    event EventHandler<TrxEventArgs> OnTxEnableChanged;

    event EventHandler<TrxEventArgs> OnTxFootSwitch;

    event EventHandler<TrxDoubleValueChangedEventArgs> OnDdsFreqChanged;

    event EventHandler<TrxEventArgs> OnRitEnableChanged;

    event EventHandler<TrxIntValueChangedEventArgs> OnRitOffsetChanged;

    event EventHandler<TrxStringValueChangedEventArgs> OnModulationChanged;

    event EventHandler<TrxEventArgs> OnRxEnableChanged;

    event EventHandler<TrxEventArgs> OnXitEnableChanged;

    event EventHandler<TrxIntValueChangedEventArgs> OnXitOffsetChanged;

    event EventHandler<TrxEventArgs> OnSplitEnableChanged;

    event EventHandler<RxFilterChangedEventArgs> OnRxFilterChanged;

    event EventHandler<TrxEventArgs> OnTrx;

    event EventHandler<TrxEventArgs> OnTune;

    event EventHandler<TrxEventArgs> OnIqEnableChanged;

    event EventHandler<TrxEventArgs> OnAudioEnableChanged;

    event EventHandler<TrxEventArgs> OnSquelchChanged;

    event EventHandler<TrxIntValueChangedEventArgs> OnSquelchThresholdChanged;

    event EventHandler<TrxEventArgs> OnRxMute;
}