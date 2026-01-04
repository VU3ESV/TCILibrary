namespace ExpertElectronics.Tci.Interfaces;

public interface ITransceiverController
{
    #region Properties

    ITciClient TciClient { get; }

    bool Start { get; set; }

    bool Stop { get; set; }

    string SoftwareName { get; set; }

    string SoftwareVersion { get; set; }

    long VfoMin { get; set; }

    long VfoMax { get; set; }

    long IfMin { get; set; }

    long IfMax { get; set; }

    uint TrxCount { get; set; }

    uint ChannelsCount { get; set; }

    string Device { get; set; }

    bool ReceiveOnly { get; set; }

    float TxPower { get; set; }

    float TxSwr { get; set; }

    uint CwMacroSpeed { get; set; }

    uint CwMacrosDelay { get; set; }

    uint Drive { get; set; }

    uint TuneDrive { get; set; }

    int Volume { get; set; }

    uint IqSampleRate { get; set; }

    uint AudioSampleRate { get; set; }

    bool Ready { get; set; }

    bool Mute { get; set; }

    IEnumerable<string> ModulationsList { get; set; }

    IEnumerable<ITransceiver> Transceivers { get; }

    TransceiverConnectionState ConnectionState { get; }
    uint CwMacrosSpeedDown { get; set; }
    uint CwMacrosSpeedUp { get; set; }

    #endregion

    #region Methods      

    ITransceiver GetTransceiver(uint transceiverPeriodicNumber);

    bool TxEnable(uint transceiverPeriodicNumber);

    bool TxFootSwitch(uint transceiverPeriodicNumber);

    Task StartTransceiver();

    Task StopTransceiver();

    void CreateTransceivers(uint transceiverCount);

    Task SetDdsFrequency(uint transceiverPeriodicNumber, double frequency);

    double ReadDdsFrequency(uint transceiverPeriodicNumber);

    Task IfFilter(uint receiverPeriodicNumber, uint channelPeriodicNumber, double frequency);

    double IfFilter(uint receiverPeriodicNumber, uint channelPeriodicNumber);

    Task RitEnable(uint transceiverPeriodicNumber, bool state);
    bool RitEnable(uint transceiverPeriodicNumber);

    Task Modulation(uint transceiverPeriodicNumber, string mode);

    string Modulation(uint transceiverPeriodicNumber);

    Task RxEnable(uint receiverPeriodicNumber, bool state);

    bool RxEnable(uint receiverPeriodicNumber);

    Task XitEnable(uint transceiverPeriodicNumber, bool state);

    bool XitEnable(uint transceiverPeriodicNumber);

    Task SplitEnable(uint transceiverPeriodicNumber, bool state);

    bool SplitEnable(uint transceiverPeriodicNumber);

    Task XitOffset(uint transceiverPeriodicNumber, int offsetFrequencyInHz);

    int XitOffset(uint transceiverPeriodicNumber);

    Task ChannelEnable(uint transceiverPeriodicNumber, uint channel, bool state);

    bool ChannelEnable(uint transceiverPeriodicNumber, uint channel);

    Task RxFilter(uint transceiverPeriodicNumber);

    int RxFilterLowLimit(uint transceiverPeriodicNumber);

    int RxFilterHighLimit(uint transceiverPeriodicNumber);

    Task ReadRxSMeter(uint transceiverPeriodicNumber, uint channelPeriodicNumber);

    int RxSMeter(uint transceiverPeriodicNumber, uint channelPeriodicNumber);

    Task SetCwMacroSpeed(uint value);

    Task CwMacroSpeedUp(uint value);

    Task CwMacroSpeedDown(uint value);

    Task SetCwMacrosDelay(uint value);

    Task Trx(uint transceiverPeriodicNumber, bool enable, string signalSource = "mic");

    bool Trx(uint transceiverPeriodicNumber);

    Task Tune(uint transceiverPeriodicNumber, bool enable);

    bool Tune(uint transceiverPeriodicNumber);

    Task SetDrive(uint level);

    Task SetTuneDrive(uint level);

    Task IqStart(uint transceiverPeriodicNumber);

    Task IqStop(uint transceiverPeriodicNumber);

    Task SetIqSampleRate(uint sampleRateInHz);

    Task AudioStart(uint transceiverPeriodicNumber);

    Task AudioStop(uint transceiverPeriodicNumber);

    Task SetAudioSampleRate(uint sampleRateInHz);

    Task Spot(string callSign, string mode, long frequencyInHz, Color color, string additionalText);

    Task SpotDelete(string callSign);

    Task SpotClear();

    Task SetVolume(int volumeValueIndB);

    Task SquelchEnable(uint transceiverPeriodicNumber, bool state);

    bool SquelchEnable(uint transceiverPeriodicNumber);

    Task SquelchLevel(uint transceiverPeriodicNumber, int thresholdIndB);

    int SquelchLevel(uint transceiverPeriodicNumber);

    Task Vfo(uint transceiverPeriodicNumber, uint channelNumber, long tuningFrequencyInHz);

    long Vfo(uint transceiverPeriodicNumber, uint channelNumber);

    Task VfoAToB(uint transceiverPeriodicNumber);

    Task VfoBToA(uint transceiverPeriodicNumber);

    Task SetMute(bool state);

    Task RxMute(uint receiverPeriodicNumber, bool state);

    bool RxMute(uint receiverPeriodicNumber);

    bool IsStarted();

    Task SetMacros(uint transceiverPeriodicNumber, string text);

    Task SetCwMacrosStop();

    Task RitOffset(uint transceiverPeriodicNumber, int value);

    Task CwMessage(uint transceiverPeriodicNumber, string before, string callSign, string after);

    Task AddCwMessageCallSign(string callSign);
    #endregion
    #region Events

    event EventHandler<EventArgs> OnStarted;

    event EventHandler<EventArgs> OnStopped;

    event EventHandler<StateChangeEventArgs> OnMute;

    event EventHandler<VfoLimitsChangedEventArgs> OnVfoLimitsChanged;

    event EventHandler<IfLimitsChangedEventArgs> OnIfLimitsChanged;

    event EventHandler<UintValueChangedEventArgs> OnIqOutSampleRateChanged;

    event EventHandler<IntValueChangedEventArgs> OnVolumeChanged;

    event EventHandler<UintValueChangedEventArgs> OnAudioSampleRateChanged;

    event EventHandler<UintValueChangedEventArgs> OnDrive;

    event EventHandler<UintValueChangedEventArgs> OnTuneDrive;

    event EventHandler<UintValueChangedEventArgs> OnCwSpeedChanged;

    event EventHandler<UintValueChangedEventArgs> OnCwMacroSpeedUp;

    event EventHandler<UintValueChangedEventArgs> OnCwMacroSpeedDown;

    event EventHandler<UintValueChangedEventArgs> OnCwMacrosDelayChanged;

    #endregion
}