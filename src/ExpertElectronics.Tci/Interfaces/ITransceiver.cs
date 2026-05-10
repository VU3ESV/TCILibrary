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

    bool Lock { get; set; }

    string AgcMode { get; set; }

    int AgcGain { get; set; }

    bool RxNb { get; set; }

    int RxNbThreshold { get; set; }

    int RxNbPulseDuration { get; set; }

    bool RxBin { get; set; }

    bool RxNr { get; set; }

    bool RxAnc { get; set; }

    bool RxAnf { get; set; }

    bool RxApf { get; set; }

    bool RxDse { get; set; }

    bool RxNf { get; set; }

    bool CtcssEnable { get; set; }

    int CtcssMode { get; set; }

    int CtcssRxTone { get; set; }

    int CtcssTxTone { get; set; }

    int CtcssLevel { get; set; }

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

    event EventHandler<TrxEventArgs> OnLockChanged;

    event EventHandler<TrxStringValueChangedEventArgs> OnAgcModeChanged;

    event EventHandler<TrxIntValueChangedEventArgs> OnAgcGainChanged;

    event EventHandler<TrxEventArgs> OnRxNbEnableChanged;

    event EventHandler<RxNbParamEventArgs> OnRxNbParamChanged;

    event EventHandler<TrxEventArgs> OnRxBinEnableChanged;

    event EventHandler<TrxEventArgs> OnRxNrEnableChanged;

    event EventHandler<TrxEventArgs> OnRxAncEnableChanged;

    event EventHandler<TrxEventArgs> OnRxAnfEnableChanged;

    event EventHandler<TrxEventArgs> OnRxApfEnableChanged;

    event EventHandler<TrxEventArgs> OnRxDseEnableChanged;

    event EventHandler<TrxEventArgs> OnRxNfEnableChanged;

    event EventHandler<TrxEventArgs> OnCtcssEnableChanged;

    event EventHandler<TrxIntValueChangedEventArgs> OnCtcssModeChanged;

    event EventHandler<TrxIntValueChangedEventArgs> OnCtcssRxToneChanged;

    event EventHandler<TrxIntValueChangedEventArgs> OnCtcssTxToneChanged;

    event EventHandler<TrxIntValueChangedEventArgs> OnCtcssLevelChanged;
}
