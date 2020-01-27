using System;
using System.Collections.Generic;
using System.Drawing;
using ExpertElectronics.Tci.Events;

namespace ExpertElectronics.Tci.Interfaces
{
    public interface ITransceiverController
    {
        ITciClient TciClient { get; }

        string SoftwareName { get; set; }
        string SoftwareVersion { get; set; }

        // Few Parameters are broadcast by the TCIServer when a client connects

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

        IEnumerable<string> ModulationsList { get; }

        IEnumerable<ITransceiver> Transceivers { get; }

        ITransceiver GeTransceiver(uint transceiverPeriodicNumber);

        bool TxEnable(uint transceiverPeriodicNumber, bool state);

        void TxFootSwitch(uint transceiverPeriodicNumber, bool footSwitchState);

        void Start();

        void Stop();

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

        void CwMacroSpeed(uint value);

        uint CwMacroSpeed();

        void CwMacroSpeedUp(uint value);

        void CwMacroSpeedDown(uint value);

        void CwMacrosDelay(uint value);

        uint CwMacrosDelay();

        void Trx(uint transceiverPeriodicNumber, bool enable, string signalSource = "mic");

        bool Trx(uint transceiverPeriodicNumber);

        void Tune(uint transceiverPeriodicNumber, bool enable);

        bool Tune(uint transceiverPeriodicNumber);

        void Drive(uint level);

        uint Drive();

        void TuneDrive(uint powerOutput);

        uint TuneDrive();

        bool IqStart(uint transceiverPeriodicNumber);

        bool IqStop(uint transceiverPeriodicNumber);

        void IqSampleRate(uint sampleRateInHz);

        uint IqSampleRate();

        void AudioStart(uint transceiverPeriodicNumber);

        void AudioStop(uint transceiverPeriodicNumber);

        void AudioSampleRate(uint sampleRateInHz);

        uint AudioSampleRate();

        void Spot(string callSign, string mode, long frequencyInHz, Color color, string additionalText);

        void SpotDelete(string callSign);

        void SpotClear();

        void Volume(int volumeValueIndB);

        int Volume();

        void SquelchEnable(uint transceiverPeriodicNumber, bool state);

        bool SquelchEnable(uint transceiverPeriodicNumber);

        void SquelchLevel(uint transceiverPeriodicNumber, int thresholdIndB);

        int SquelchLevel(uint transceiverPeriodicNumber);


        void Vfo(uint transceiverPeriodicNumber, uint channelNumber, long tuningFrequencyInHz);

        long Vfo(uint transceiverPeriodicNumber, uint channelNumber);

        void Mute(bool state);

        bool Mute();

        void RxMute(uint receiverPeriodicNumber, bool state);

        bool RxMute(uint receiverPeriodicNumber);

        TransceiverConnectionState ConnectionState { get; }

        bool Ready { get; set; }

        bool IsStarted();


        void SetMacros(uint transceiverPeriodicNumber, string text);

        void SetCwMacrosStop();

        void RitOffset(uint transceiverPeriodicNumber, int value);


        void CwMessage(uint transceiverPeriodicNumber, string before, string callSign, string after);

        void AddCwMessageCallSign(string callSign);


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

        void CreateTransceivers(uint transceiverCount);
    }
}