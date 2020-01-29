using System;
using System.Collections.Generic;
using System.Drawing;
using ExpertElectronics.Tci.Events;

namespace ExpertElectronics.Tci.Interfaces
{
    public interface ITransceiverController
    {
        #region Properties

        ITciClient TciClient { get; }

        string SoftwareName { get; set; }

        string SoftwareVersion { get; set; }

        long VfoMin { get; }

        long VfoMax { get; }


        long IfMin { get; }

        long IfMax { get; }

        uint TrxCount { get; }

        uint ChannelsCount { get; }

        string Device { get; set; }

        bool ReceiveOnly { get; set; }

        float TxPower { get; set; }

        float TxSwr { get; set; }

        uint CwMacroSpeed { get; }

        uint CwMacrosDelay { get; }

        uint Drive { get; }

        uint TuneDrive { get; }

        int Volume { get; }

        uint IqSampleRate { get; }

        uint AudioSampleRate { get; }

        bool Ready { get; set; }

        bool Mute { get; }

        IEnumerable<string> ModulationsList { get; }

        IEnumerable<ITransceiver> Transceivers { get; }

        TransceiverConnectionState ConnectionState { get; }

        #endregion

        #region Methods      

        ITransceiver GeTransceiver(uint transceiverPeriodicNumber);

        bool TxEnable(uint transceiverPeriodicNumber, bool state);

        void TxFootSwitch(uint transceiverPeriodicNumber, bool footSwitchState);

        void Start();

        void Stop();

        void CreateTransceivers(uint transceiverCount);

        void SetDdsFrequency(uint transceiverPeriodicNumber, double frequency);

        double ReadDdsFrequency(uint transceiverPeriodicNumber);

        void IfFilter(uint receiverPeriodicNumber, uint channelPeriodicNumber, double frequency);

        double IfFilter(uint receiverPeriodicNumber, uint channelPeriodicNumber);

        void RitEnable(uint transceiverPeriodicNumber, bool state);
        bool RitEnable(uint transceiverPeriodicNumber);

        void Modulation(uint transceiverPeriodicNumber, string mode);

        string Modulation(uint transceiverPeriodicNumber);

        void RxEnable(uint receiverPeriodicNumber, bool state);

        bool RxEnable(uint receiverPeriodicNumber);

        void XitEnable(uint transceiverPeriodicNumber, bool state);

        bool XitEnable(uint transceiverPeriodicNumber);

        void SplitEnable(uint transceiverPeriodicNumber, bool state);

        bool SplitEnable(uint transceiverPeriodicNumber);

        void XitOffset(uint transceiverPeriodicNumber, int offsetFrequencyInHz);

        int XitOffset(uint transceiverPeriodicNumber);

        void ChannelEnable(uint transceiverPeriodicNumber, uint channel, bool state);

        bool ChannelEnable(uint transceiverPeriodicNumber, uint channel);

        void RxFilter(uint transceiverPeriodicNumber, int bottomFrequencyLimitInHz, int topFrequencyLimitInHz);

        int RxFilterLowLimit(uint transceiverPeriodicNumber);

        int RxFilterHighLimit(uint transceiverPeriodicNumber);

        void RxSMeter(uint transceiverPeriodicNumber, uint channelPeriodicNumber, int signalLevel);

        int RxSMeter(uint transceiverPeriodicNumber, uint channelPeriodicNumber);

        void SetCwMacroSpeed(uint value);

        void CwMacroSpeedUp(uint value);

        void CwMacroSpeedDown(uint value);

        void SetCwMacrosDelay(uint value);

        void Trx(uint transceiverPeriodicNumber, bool enable, string signalSource = "mic");

        bool Trx(uint transceiverPeriodicNumber);

        void Tune(uint transceiverPeriodicNumber, bool enable);

        bool Tune(uint transceiverPeriodicNumber);

        void SetDrive(uint level);

        void SetTuneDrive(uint powerOutput);

        bool IqStart(uint transceiverPeriodicNumber);

        bool IqStop(uint transceiverPeriodicNumber);

        void SetIqSampleRate(uint sampleRateInHz);

        void AudioStart(uint transceiverPeriodicNumber);

        void AudioStop(uint transceiverPeriodicNumber);

        void SetAudioSampleRate(uint sampleRateInHz);

        void Spot(string callSign, string mode, long frequencyInHz, Color color, string additionalText);

        void SpotDelete(string callSign);

        void SpotClear();

        void SetVolume(int volumeValueIndB);

        void SquelchEnable(uint transceiverPeriodicNumber, bool state);

        bool SquelchEnable(uint transceiverPeriodicNumber);

        void SquelchLevel(uint transceiverPeriodicNumber, int thresholdIndB);

        int SquelchLevel(uint transceiverPeriodicNumber);

        void Vfo(uint transceiverPeriodicNumber, uint channelNumber, long tuningFrequencyInHz);

        long Vfo(uint transceiverPeriodicNumber, uint channelNumber);

        void SetMute(bool state);

        void RxMute(uint receiverPeriodicNumber, bool state);

        bool RxMute(uint receiverPeriodicNumber);

        bool IsStarted();

        void SetMacros(uint transceiverPeriodicNumber, string text);

        void SetCwMacrosStop();

        void RitOffset(uint transceiverPeriodicNumber, int value);

        void CwMessage(uint transceiverPeriodicNumber, string before, string callSign, string after);

        void AddCwMessageCallSign(string callSign);
        #endregion
        #region Events

        event EventHandler<EventArgs> OnStarted;

        event EventHandler<EventArgs> OnStopped;

        event EventHandler<TrxDoubleValueChangedEventArgs> OnDdsFreqChanged;

        event EventHandler<IfFrequencyChangedEventArgs> OnIfFreqChanged;

        event EventHandler<TrxEventArgs> OnTrx;

        event EventHandler<TrxEventArgs> OnMute;

        event EventHandler<TrxEventArgs> OnRxMute;

        event EventHandler<VfoLimitsChangedEventArgs> OnVfoLimitsChanged;

        event EventHandler<IfLimitsChangedEventArgs> OnIfLimitsChanged;

        event EventHandler<ChannelSMeterChangeEventArgs> OnChannelSMeterChanged;

        event EventHandler<TrxStringValueChangedEventArgs> OnModulationChanged;

        event EventHandler<UintValueChangedEventArgs> OnIqOutSampleRateChanged;

        event EventHandler<UintValueChangedEventArgs> OnIqStartChanged;

        event EventHandler<UintValueChangedEventArgs> OnIqStopChanged;

        event EventHandler<UintValueChangedEventArgs> OnAudioStartChanged;

        event EventHandler<UintValueChangedEventArgs> OnAudioStopChanged;

        event EventHandler<IntValueChangedEventArgs> OnVolumeChanged;

        event EventHandler<UintValueChangedEventArgs> OnAudioSampleRateChanged;

        event EventHandler<TrxIntValueChangedEventArgs> OnSqlLevelChanged;

        event EventHandler<TrxEventArgs> OnRxEnableChanged;

        event EventHandler<TrxEventArgs> OnSqlEnableChanged;

        event EventHandler<TrxEventArgs> OnTune;

        event EventHandler<SpotEventArgs> OnSpot;

        event EventHandler<StringValueChangedEventArgs> OnSpotDelete;

        event EventHandler<TrxEventArgs> OnSpotClear;

        event EventHandler<UintValueChangedEventArgs> OnDrive;

        event EventHandler<UintValueChangedEventArgs> OnTuneDrive;

        event EventHandler<TrxEventArgs> OnTxEnableChanged;

        event EventHandler<TrxStringValueChangedEventArgs> OnCwMacros;

        event EventHandler<EventArgs> OnCwMacrosStop;

        event EventHandler<UintValueChangedEventArgs> OnCwSpeedChanged;

        event EventHandler<UintValueChangedEventArgs> OnCwMacroSpeedUp;

        event EventHandler<UintValueChangedEventArgs> OnCwMacroSpeedDown;

        event EventHandler<UintValueChangedEventArgs> OnCwMacrosDelayChanged;

        event EventHandler<TrxEventArgs> OnRitEnableChanged;

        event EventHandler<TrxEventArgs> OnXitEnableChanged;

        event EventHandler<TrxEventArgs> OnSplitEnableChanged;

        event EventHandler<TrxIntValueChangedEventArgs> OnRitOffsetChanged;

        event EventHandler<TrxIntValueChangedEventArgs> OnXitOffsetChanged;

        event EventHandler<ChannelEnableChangeEventArgs> OnChannelEnableChanged;

        event EventHandler<RxFilterChangedEventArgs> OnRxFilterChanged;

        event EventHandler<StringValueChangedEventArgs> OnCwMessageCallSign;

        event EventHandler<VfoChangeEventArgs> OnVfoChange;

        #endregion
    }
}