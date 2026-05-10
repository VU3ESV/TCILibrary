using ExpertElectronics.Tci.Streaming;

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

    Task SetCwTerminalMode(bool enable);

    // ---------- TCI v2.0 extensions ----------

    uint CwKeyerSpeed { get; set; }
    int MonVolume { get; set; }
    bool MonEnable { get; set; }
    int DiglOffset { get; set; }
    int DiguOffset { get; set; }
    string AudioStreamSampleType { get; set; }
    uint AudioStreamChannels { get; set; }
    uint AudioStreamSamples { get; set; }
    uint TxStreamAudioBuffering { get; set; }
    bool RxSensorsEnable { get; set; }
    bool TxSensorsEnable { get; set; }
    bool AppFocus { get; set; }
    long TxFrequency { get; set; }

    Task SetCwKeyerSpeed(uint wpm);
    Task SetMonVolume(int valueInDb);
    Task SetMonEnable(bool state);
    Task SetAgcMode(uint transceiverPeriodicNumber, string mode);
    Task SetAgcGain(uint transceiverPeriodicNumber, int gainDb);
    Task RxNbEnable(uint transceiverPeriodicNumber, bool state);
    Task RxNbParam(uint transceiverPeriodicNumber, int threshold, int pulseDurationUs);
    Task RxBinEnable(uint transceiverPeriodicNumber, bool state);
    Task RxNrEnable(uint transceiverPeriodicNumber, bool state);
    Task RxAncEnable(uint transceiverPeriodicNumber, bool state);
    Task RxAnfEnable(uint transceiverPeriodicNumber, bool state);
    Task RxApfEnable(uint transceiverPeriodicNumber, bool state);
    Task RxDseEnable(uint transceiverPeriodicNumber, bool state);
    Task RxNfEnable(uint transceiverPeriodicNumber, bool state);
    Task Lock(uint transceiverPeriodicNumber, bool state);
    Task VfoLock(uint transceiverPeriodicNumber, uint channelNumber, bool state);
    Task RxVolume(uint transceiverPeriodicNumber, uint channelNumber, int valueInDb);
    Task RxBalance(uint transceiverPeriodicNumber, uint channelNumber, int valueInDb);
    Task SetDiglOffset(int offsetHz);
    Task SetDiguOffset(int offsetHz);
    Task LineOutStart(uint receiverPeriodicNumber);
    Task LineOutStop(uint receiverPeriodicNumber);
    Task LineOutRecorderStart(uint receiverPeriodicNumber, uint maxRecordingSeconds);
    Task LineOutRecorderSave(uint receiverPeriodicNumber, string filePath);
    Task LineOutRecorderBreak(uint receiverPeriodicNumber);
    Task SetAudioStreamSampleType(string format);
    Task SetAudioStreamChannels(uint channels);
    Task SetAudioStreamSamples(uint samples);
    Task SetTxStreamAudioBuffering(uint timeoutMs);
    Task SetInFocus();
    Task SetRxSensorsEnable(bool state, uint sendingIntervalMs = 0);
    Task SetTxSensorsEnable(bool state, uint sendingIntervalMs = 0);
    Task Keyer(uint transceiverPeriodicNumber, bool pressed, int previousCharacterDurationMs = 0);

    Task SendTxAudioPacket(uint receiverNumber, uint sampleRate, uint channels, float[] samples);

    Task SetCtcssEnable(uint transceiverPeriodicNumber, bool state);
    Task SetCtcssMode(uint transceiverPeriodicNumber, int mode);
    Task SetCtcssRxTone(uint transceiverPeriodicNumber, int toneNumber);
    Task SetCtcssTxTone(uint transceiverPeriodicNumber, int toneNumber);
    Task SetCtcssLevel(uint transceiverPeriodicNumber, int levelPercent);
    Task ECoderSwitchRx(uint ecoderPeriodicNumber, uint receiverPeriodicNumber);
    Task ECoderSwitchChannel(uint ecoderPeriodicNumber, uint channelPeriodicNumber);

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

    event EventHandler<UintValueChangedEventArgs> OnCwKeyerSpeedChanged;
    event EventHandler<IntValueChangedEventArgs> OnMonVolumeChanged;
    event EventHandler<StateChangeEventArgs> OnMonEnableChanged;
    event EventHandler<IntValueChangedEventArgs> OnDiglOffsetChanged;
    event EventHandler<IntValueChangedEventArgs> OnDiguOffsetChanged;
    event EventHandler<StringValueChangedEventArgs> OnAudioStreamSampleTypeChanged;
    event EventHandler<UintValueChangedEventArgs> OnAudioStreamChannelsChanged;
    event EventHandler<UintValueChangedEventArgs> OnAudioStreamSamplesChanged;
    event EventHandler<UintValueChangedEventArgs> OnTxStreamAudioBufferingChanged;
    event EventHandler<StateChangeEventArgs> OnRxSensorsEnableChanged;
    event EventHandler<StateChangeEventArgs> OnTxSensorsEnableChanged;
    event EventHandler<StateChangeEventArgs> OnAppFocusChanged;
    event EventHandler<LongValueChangedEventArgs> OnTxFrequencyChanged;
    event EventHandler<SpotClickedEventArgs> OnSpotClicked;
    event EventHandler<RxSpotClickedEventArgs> OnRxSpotClicked;
    event EventHandler<KeyerEventArgs> OnKeyer;
    event EventHandler<TxSensorsEventArgs> OnTxSensorsChanged;
    event EventHandler<EventArgs> OnCwMacrosEmpty;
    event EventHandler<ECoderSwitchEventArgs> OnECoderRxSwitched;
    event EventHandler<ECoderSwitchEventArgs> OnECoderChannelSwitched;
    event EventHandler<StreamPacketEventArgs> OnIqStreamReceived;
    event EventHandler<StreamPacketEventArgs> OnRxAudioStreamReceived;
    event EventHandler<StreamPacketEventArgs> OnTxAudioStreamReceived;
    event EventHandler<StreamPacketEventArgs> OnTxChronoReceived;
    event EventHandler<StreamPacketEventArgs> OnLineOutStreamReceived;

    #endregion
}