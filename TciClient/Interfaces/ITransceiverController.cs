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

        ITransceiver GeTransceiver(uint transceiverPeriodicNumber);

        bool TxEnable(uint transceiverPeriodicNumber);

        bool TxFootSwitch(uint transceiverPeriodicNumber);

        void StartTransceiver();

        void StopTransceiver();

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

        void RxFilter(uint transceiverPeriodicNumber);

        int RxFilterLowLimit(uint transceiverPeriodicNumber);

        int RxFilterHighLimit(uint transceiverPeriodicNumber);

        void ReadRxSMeter(uint transceiverPeriodicNumber, uint channelPeriodicNumber);

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

        void SetTuneDrive(uint level);

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
}