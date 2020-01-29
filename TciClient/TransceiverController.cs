using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using ExpertElectronics.Tci.Events;
using ExpertElectronics.Tci.Interfaces;
using ExpertElectronics.Tci.TciCommands;

namespace ExpertElectronics.Tci
{
    public class TransceiverController : ITransceiverController, IDisposable
    {
        public TransceiverController(ITciMessageHandler messageHandler, ITciClient tciClient)
        {
            Debug.Assert(messageHandler != null);
            Debug.Assert(tciClient != null);

            _messageHandler = messageHandler;
            TciClient = tciClient;
            _transceivers = new List<Transceiver>();
            _messageHandler.OnSocketConnected += MessageHandler_OnSocketConnected;
            _messageHandler.OnSocketMessageReceived += MessageHandler_OnSocketMessageReceived;
            _commands = new Dictionary<string, ITciCommand>();
            Initialize();
        }

        private void Initialize()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(ITciCommand).IsAssignableFrom(p) && !p.IsInterface);

            foreach (var type in types)
            {
                _commands.Add((string)type.GetProperty("Name").GetValue(null, null),
                    (ITciCommand)type.GetMethod("Create").Invoke(this, new object[] { this }));
            }
        }

        private void MessageHandler_OnSocketMessageReceived(object sender, TciMessageReceivedEventArgs e)
        {
            var message = e.Message;
            var commandId = message.Split(':', ',', ';')[0];
            if (_commands.Keys.Contains(commandId))
            {
                var command = _commands[commandId];
                command.ProcessCommandResponses(new List<string> { message });
            }
            else
            {
                //list the unsupported commands 
                Console.WriteLine($"No Command Implementation for {message}");
            }
        }

        private void MessageHandler_OnSocketConnected(object sender, TciConnectedEventArgs e)
        {
            ConnectionState = e.TciConnection
                ? TransceiverConnectionState.Connected
                : TransceiverConnectionState.Disconnected;
        }

        private readonly ITciMessageHandler _messageHandler;

        public ITciClient TciClient { get; }

        public string SoftwareName { get; set; }

        public string SoftwareVersion { get; set; }

        public long VfoMin { get; private set; }

        public long VfoMax { get; private set; }

        public long IfMin { get; private set; }

        public long IfMax { get; private set; }

        public uint TrxCount { get; private set; }

        public uint ChannelsCount { get; private set; }

        public string Device { get; set; }

        public bool Mute { get; private set; }

        public bool ReceiveOnly
        {
            get => _receiveOnly;
            set
            {
                _receiveOnly = value;
                if (!_receiveOnly)
                {
                    return;
                }

                foreach (var transceiver in Transceivers)
                {
                    transceiver.TxEnable = false;
                    foreach (var transceiverChannel in transceiver.Channels)
                    {
                        transceiverChannel.ReceiveOnly = true;
                    }
                }
            }
        }

        public float TxPower { get; set; }

        public float TxSwr { get; set; }

        public IEnumerable<string> ModulationsList { get; private set; }

         public uint CwMacroSpeed { get; private set; }

        public uint CwMacrosDelay { get; private set; }

        public uint Drive { get; private set; }

        public uint TuneDrive { get; private set; }

        public TransceiverConnectionState ConnectionState { get; private set; }
        public bool Ready { get; set; }
           
        public int Volume { get; set; }

        public uint AudioSampleRate { get; set; }

        public uint IqSampleRate { get; set; }

        internal void AddModulationList(List<string> modulationList)
        {
            ModulationsList = modulationList;
        }

        public IEnumerable<ITransceiver> Transceivers => _transceivers;

        public void CreateTransceivers(uint transceiverCount)
        {
            for (uint i = 0; i < transceiverCount; i++)
            {
                var transceiver = new Transceiver(i);
                _transceivers.Add(transceiver);
            }

            TrxCount = transceiverCount;
        }

        public void CreateChannel(uint channelCount)
        {
            foreach (var transceiver in Transceivers)
            {
                transceiver.AddChannel(channelCount);
            }

            ChannelsCount = channelCount;
        }

        public ITransceiver GeTransceiver(uint transceiverPeriodicNumber)
        {
            return Transceivers.FirstOrDefault(_ => _.PeriodicNumber == transceiverPeriodicNumber);
        }

        internal void IfLimits(long bottomFrequencyLimitInHz, long topFrequencyLimitInHz)
        {
            IfMin = bottomFrequencyLimitInHz;
            IfMax = topFrequencyLimitInHz;
            OnIfLimitsChanged?.Invoke(this, new IfLimitsChangedEventArgs(IfMin, IfMax));
        }

        public bool TxEnable(uint transceiverPeriodicNumber, bool state)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return false;
            }

            transceiver.TxEnable = state;
            OnTxEnableChanged?.Invoke(this, new TrxEventArgs(transceiverPeriodicNumber, state));

            return true;
        }

        public void TxFootSwitch(uint transceiverPeriodicNumber, bool footSwitchState)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return;
            }

            transceiver.TxFootSwitch = footSwitchState;
        }

        public void Start()
        {
            if (_start)
            {
                return;
            }

            _start = true;
            OnStarted?.Invoke(this, new EventArgs());
        }

        public void Stop()
        {
            if (_start == false)
            {
                return;
            }

            _start = false;
            OnStopped?.Invoke(this, new EventArgs());
        }

        public void SetDdsFrequency(uint transceiverPeriodicNumber, double frequency)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return;
            }

            if (Math.Abs(transceiver.DdsFrequency - frequency) < Tolerance)
            {
                return;
            }

            transceiver.DdsFrequency = frequency;
            OnDdsFreqChanged?.Invoke(this, new TrxDoubleValueChangedEventArgs(transceiverPeriodicNumber, frequency));
        }

        public double ReadDdsFrequency(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return 0;
            }

            return transceiver.DdsFrequency;
        }

        public void IfFilter(uint receiverPeriodicNumber, uint channelPeriodicNumber, double frequency)
        {
            var transceiver = GeTransceiver(receiverPeriodicNumber);

            var channel = transceiver?.Channels.FirstOrDefault(_ => _.PeriodicNumber == channelPeriodicNumber);
            if (channel == null)
            {
                return;
            }

            if (Math.Abs(channel.IfFilter - frequency) < Tolerance)
            {
                return;
            }

            channel.IfFilter = frequency;
            OnIfFreqChanged?.Invoke(this, new IfFrequencyChangedEventArgs(receiverPeriodicNumber, channelPeriodicNumber, frequency));
        }

        public double IfFilter(uint receiverPeriodicNumber, uint channelPeriodicNumber)
        {
            var transceiver = GeTransceiver(receiverPeriodicNumber);

            var channel = transceiver?.Channels.FirstOrDefault(_ => _.PeriodicNumber == channelPeriodicNumber);
            if (channel == null)
            {
                return 0;
            }

            return channel.IfFilter;
        }

        public void RitEnable(uint transceiverPeriodicNumber, bool state)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return;
            }

            if (transceiver.Rit == state)
            {
                return;
            }

            transceiver.Rit = state;
            OnRitEnableChanged?.Invoke(this, new TrxEventArgs(transceiverPeriodicNumber, state));
        }

        public bool RitEnable(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            return transceiver != null && transceiver.Rit;
        }

        public void Modulation(uint transceiverPeriodicNumber, string mode)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return;
            }

            if (transceiver.Modulation == mode)
            {
                return;
            }

            transceiver.Modulation = mode;
            OnModulationChanged?.Invoke(this, new TrxStringValueChangedEventArgs(transceiverPeriodicNumber, mode));
        }

        public string Modulation(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            return transceiver == null ? string.Empty : transceiver.Modulation;
        }

        public void RxEnable(uint receiverPeriodicNumber, bool state)
        {
            var transceiver = GeTransceiver(receiverPeriodicNumber);
            if (transceiver == null)
            {
                return;
            }

            if (transceiver.RxEnable == state)
            {
                return;
            }

            transceiver.RxEnable = state;
            OnRxEnableChanged?.Invoke(this, new TrxEventArgs(receiverPeriodicNumber, state));
        }

        public bool RxEnable(uint receiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(receiverPeriodicNumber);
            return transceiver?.RxEnable ?? false;
        }

        public void XitEnable(uint transceiverPeriodicNumber, bool state)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return;
            }

            if (transceiver.Xit == state)
            {
                return;
            }

            transceiver.Xit = state;
            OnXitEnableChanged?.Invoke(this, new TrxEventArgs(transceiverPeriodicNumber, state));
        }

        public bool XitEnable(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            return transceiver?.Xit ?? false;
        }

        public void SplitEnable(uint transceiverPeriodicNumber, bool state)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return;
            }

            if (transceiver.Split == state)
            {
                return;
            }

            transceiver.Split = state;
            OnSplitEnableChanged?.Invoke(this, new TrxEventArgs(transceiverPeriodicNumber, state));
        }

        public bool SplitEnable(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            return transceiver?.Split ?? false;
        }

        public void XitOffset(uint transceiverPeriodicNumber, int offsetFrequencyInHz)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return;
            }

            if (transceiver.XitOffset == offsetFrequencyInHz)
            {
                return;
            }

            transceiver.XitOffset = offsetFrequencyInHz;
            OnXitOffsetChanged?.Invoke(this, new TrxIntValueChangedEventArgs(transceiverPeriodicNumber, offsetFrequencyInHz));
        }

        public int XitOffset(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            return transceiver?.XitOffset ?? 0;
        }

        public void ChannelEnable(uint transceiverPeriodicNumber, uint channelNumber, bool state)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);

            var channel = transceiver?.Channels.FirstOrDefault(_ => _.PeriodicNumber == channelNumber);
            if (channel == null)
            {
                return;
            }

            if (channel.Enable == state)
            {
                return;
            }

            channel.Enable = state;
            OnChannelEnableChanged?.Invoke(this, new ChannelEnableChangeEventArgs(transceiverPeriodicNumber, channelNumber, state));
        }

        public bool ChannelEnable(uint transceiverPeriodicNumber, uint channelNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);

            var channel = transceiver?.Channels.FirstOrDefault(_ => _.PeriodicNumber == channelNumber);
            return channel != null && channel.Enable;
        }

        public void RxFilter(uint transceiverPeriodicNumber, int bottomFrequencyLimitInHz, int topFrequencyLimitInHz)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);

            if (transceiver.RxFilterLowLimit == bottomFrequencyLimitInHz &&
                transceiver.RxFilterHighLimit == topFrequencyLimitInHz)
            {
                return;
            }

            transceiver.RxFilterLowLimit = bottomFrequencyLimitInHz;
            transceiver.RxFilterHighLimit = topFrequencyLimitInHz;

            OnRxFilterChanged?.Invoke(this, new RxFilterChangedEventArgs(transceiverPeriodicNumber, bottomFrequencyLimitInHz, topFrequencyLimitInHz));
        }

        public int RxFilterLowLimit(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            return transceiver?.RxFilterLowLimit ?? 0;
        }

        public int RxFilterHighLimit(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            return transceiver?.RxFilterHighLimit ?? 0;
        }

        public void RxSMeter(uint transceiverPeriodicNumber, uint channelPeriodicNumber, int signalLevel)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);

            var channel = transceiver?.Channels.FirstOrDefault(_ => _.PeriodicNumber == channelPeriodicNumber);
            if (channel == null)
            {
                return;
            }

            if (channel.RxSMeter == signalLevel)
            {
                return;
            }

            channel.RxSMeter = signalLevel;

            OnChannelSMeterChanged?.Invoke(this, new ChannelSMeterChangeEventArgs(transceiverPeriodicNumber, channelPeriodicNumber, signalLevel));
        }

        public int RxSMeter(uint transceiverPeriodicNumber, uint channelPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);

            var channel = transceiver?.Channels.FirstOrDefault(_ => _.PeriodicNumber == channelPeriodicNumber);
            return channel?.RxSMeter ?? 0;
        }

        public void SetCwMacroSpeed(uint value)
        {
            if (value < 1 || value > 60)
            {
                return;
            }

            if (CwMacroSpeed == value)
            {
                return;
            }

            CwMacroSpeed = value;
            OnCwSpeedChanged?.Invoke(this, new UintValueChangedEventArgs(value));
        }

        public void CwMacroSpeedUp(uint value)
        {
            OnCwMacroSpeedUp?.Invoke(this, new UintValueChangedEventArgs(value));
        }

        public void CwMacroSpeedDown(uint value)
        {
            OnCwMacroSpeedDown?.Invoke(this, new UintValueChangedEventArgs(value));
        }

        public void SetCwMacrosDelay(uint value)
        {
            if (value < 10 || value > 1000)
            {
                return;
            }

            if (CwMacrosDelay == value)
            {
                return;
            }

            CwMacrosDelay = value;
            OnCwMacrosDelayChanged?.Invoke(this, new UintValueChangedEventArgs(value));
        }



        public void Trx(uint transceiverPeriodicNumber, bool enable, string signalSource = "mic")
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return;
            }

            if (transceiver.TrxEnable == enable)
            {
                return;
            }

            transceiver.TrxEnable = enable;
            OnTrx?.Invoke(this, new TrxEventArgs(transceiverPeriodicNumber, enable));
        }

        public bool Trx(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            return transceiver?.TrxEnable ?? false;
        }

        public void Tune(uint transceiverPeriodicNumber, bool enable)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return;
            }

            if (transceiver.Tune == enable)
            {
                return;
            }

            transceiver.Tune = enable;
            OnTune?.Invoke(this, new TrxEventArgs(transceiverPeriodicNumber, enable));
        }

        public bool Tune(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            return transceiver?.Tune ?? false;
        }

        public void SetDrive(uint level)
        {
            if (level > 100)
            {
                return;
            }

            if (Drive == level)
            {
                return;
            }

            Drive = level;
            OnDrive?.Invoke(this, new UintValueChangedEventArgs(level));
        }


        public void SetTuneDrive(uint powerOutput)
        {
            if (powerOutput > 100)
            {
                return;
            }

            if (TuneDrive == powerOutput)
            {
                return;
            }

            TuneDrive = powerOutput;
            OnTuneDrive?.Invoke(this, new UintValueChangedEventArgs(powerOutput));
        }

        public bool IqStart(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return false;
            }

            if (transceiver.IqEnable)
            {
                return true;
            }

            transceiver.IqEnable = true;
            OnIqStartChanged?.Invoke(this, new UintValueChangedEventArgs(transceiverPeriodicNumber));
            return true;
        }

        public bool IqStop(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return false;
            }

            if (transceiver.IqEnable == false)
            {
                return true;
            }

            transceiver.IqEnable = false;
            OnIqStopChanged?.Invoke(this, new UintValueChangedEventArgs(transceiverPeriodicNumber));
            return true;
        }

        public void SetIqSampleRate(uint sampleRateInHz)
        {
            if (IqSampleRate == sampleRateInHz)
            {
                return;
            }

            IqSampleRate = sampleRateInHz;
            OnIqOutSampleRateChanged?.Invoke(this, new UintValueChangedEventArgs(sampleRateInHz));
        }

        public void AudioStart(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return;
            }

            if (transceiver.AudioEnable)
            {
                return;
            }

            transceiver.AudioEnable = true;
            OnAudioStartChanged?.Invoke(this, new UintValueChangedEventArgs(transceiverPeriodicNumber));
        }

        public void AudioStop(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return;
            }

            if (transceiver.AudioEnable == false)
            {
                return;
            }

            transceiver.AudioEnable = false;
            OnAudioStopChanged?.Invoke(this, new UintValueChangedEventArgs(transceiverPeriodicNumber));
        }

        public void SetAudioSampleRate(uint sampleRateInHz)
        {
            if (AudioSampleRate == sampleRateInHz)
            {
                return;
            }

            AudioSampleRate = sampleRateInHz;
            OnAudioSampleRateChanged?.Invoke(this, new UintValueChangedEventArgs(sampleRateInHz));
        }

        public void Spot(string callSign, string mode, long frequencyInHz, Color color, string additionalText)
        {
            OnSpot?.Invoke(this, new SpotEventArgs(callSign, mode, frequencyInHz, color, additionalText));
        }

        public void SpotDelete(string callSign)
        {
            OnSpotDelete?.Invoke(this, new StringValueChangedEventArgs(callSign));
        }

        public void SpotClear()
        {
            OnSpotClear?.Invoke(this, new TrxEventArgs(0, true));
        }

        public void SetVolume(int volumeValueIndB)
        {
            if (Volume == volumeValueIndB)
            {
                return;
            }

            Volume = volumeValueIndB;
            OnVolumeChanged?.Invoke(this, new IntValueChangedEventArgs(volumeValueIndB));
        }

        public void SquelchEnable(uint transceiverPeriodicNumber, bool state)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return;
            }

            if (transceiver.Squelch == state)
            {
                return;
            }

            transceiver.Squelch = state;
            OnSqlEnableChanged?.Invoke(this, new TrxEventArgs(transceiverPeriodicNumber, state));
        }

        public bool SquelchEnable(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            return transceiver?.Squelch ?? false;
        }

        public void SquelchLevel(uint transceiverPeriodicNumber, int thresholdIndB)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return;
            }

            if (transceiver.SquelchThreshold == thresholdIndB)
            {
                return;
            }

            transceiver.SquelchThreshold = thresholdIndB;
            OnSqlLevelChanged?.Invoke(this, new TrxIntValueChangedEventArgs(transceiverPeriodicNumber, thresholdIndB));
        }

        public int SquelchLevel(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            return transceiver?.SquelchThreshold ?? 0;
        }

        public void Vfo(uint transceiverPeriodicNumber, uint channelPeriodicNumber, long tuningFrequencyInHz)
        {

            var transceiver = GeTransceiver(transceiverPeriodicNumber);

            var channel = transceiver?.Channels.FirstOrDefault(_ => _.PeriodicNumber == channelPeriodicNumber);
            if (channel == null)
            {
                return;
            }

            if (channel.Vfo == tuningFrequencyInHz)
            {
                return;
            }

            channel.Vfo = tuningFrequencyInHz;
            OnVfoChange?.Invoke(this, new VfoChangeEventArgs(transceiverPeriodicNumber, channelPeriodicNumber, tuningFrequencyInHz));
        }

        public long Vfo(uint transceiverPeriodicNumber, uint channelPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            var channel = transceiver?.Channels.FirstOrDefault(_ => _.PeriodicNumber == channelPeriodicNumber);
            return channel?.Vfo ?? 0;
        }


        public void SetMute(bool state)
        {
            if (Mute == state)
            {
                return;
            }

            Mute = state;
            OnMute?.Invoke(this, new TrxEventArgs(0, state));
        }

        public void RxMute(uint receiverPeriodicNumber, bool state)
        {
            var transceiver = GeTransceiver(receiverPeriodicNumber);
            if (transceiver == null)
            {
                return;
            }

            if (transceiver.RxMute == state)
            {
                return;
            }

            transceiver.RxMute = state;
            OnRxMute?.Invoke(this, new TrxEventArgs(receiverPeriodicNumber, state));
        }

        public bool RxMute(uint receiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(receiverPeriodicNumber);
            return transceiver?.RxMute ?? false;
        }


        public bool IsStarted()
        {
            return _start;
        }


        public void RitOffset(uint transceiverPeriodicNumber, int value)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return;
            }

            if (transceiver.RitOffset == value)
            {
                return;
            }

            transceiver.RitOffset = value;
            OnRitOffsetChanged?.Invoke(this, new TrxIntValueChangedEventArgs(transceiverPeriodicNumber, value));
        }

        public void CwMessage(uint transceiverPeriodicNumber, string before, string callSign, string after)
        {
            // ToDo
        }

        public void AddCwMessageCallSign(string callSign)
        {
            // ToDo
        }

        public void Dispose()
        {
            // ToDo
        }

        public void SetMacros(uint transceiverPeriodicNumber, string text)
        {
            throw new NotImplementedException();
        }

        public void SetCwMacrosStop()
        {
            throw new NotImplementedException();
        }


        internal void VfoLimits(long bottomFrequencyLimitInHz, long topFrequencyLimitInHz)
        {
            VfoMin = bottomFrequencyLimitInHz;
            VfoMax = topFrequencyLimitInHz;
            OnVfoLimitsChanged?.Invoke(this, new VfoLimitsChangedEventArgs(VfoMin, VfoMax));
        }

        public event EventHandler<EventArgs> OnStarted;
        public event EventHandler<EventArgs> OnStopped;
        public event EventHandler<TrxDoubleValueChangedEventArgs> OnDdsFreqChanged;
        public event EventHandler<IfFrequencyChangedEventArgs> OnIfFreqChanged;
        public event EventHandler<TrxEventArgs> OnTrx;
        public event EventHandler<TrxEventArgs> OnMute;
        public event EventHandler<TrxEventArgs> OnRxMute;
        public event EventHandler<VfoLimitsChangedEventArgs> OnVfoLimitsChanged;
        public event EventHandler<IfLimitsChangedEventArgs> OnIfLimitsChanged;
        public event EventHandler<ChannelSMeterChangeEventArgs> OnChannelSMeterChanged;
        public event EventHandler<TrxStringValueChangedEventArgs> OnModulationChanged;
        public event EventHandler<UintValueChangedEventArgs> OnIqOutSampleRateChanged;
        public event EventHandler<UintValueChangedEventArgs> OnIqStartChanged;
        public event EventHandler<UintValueChangedEventArgs> OnIqStopChanged;
        public event EventHandler<UintValueChangedEventArgs> OnAudioStartChanged;
        public event EventHandler<UintValueChangedEventArgs> OnAudioStopChanged;
        public event EventHandler<IntValueChangedEventArgs> OnVolumeChanged;
        public event EventHandler<UintValueChangedEventArgs> OnAudioSampleRateChanged;
        public event EventHandler<TrxIntValueChangedEventArgs> OnSqlLevelChanged;
        public event EventHandler<TrxEventArgs> OnRxEnableChanged;
        public event EventHandler<TrxEventArgs> OnSqlEnableChanged;
        public event EventHandler<TrxEventArgs> OnTune;
        public event EventHandler<SpotEventArgs> OnSpot;
        public event EventHandler<StringValueChangedEventArgs> OnSpotDelete;
        public event EventHandler<TrxEventArgs> OnSpotClear;
        public event EventHandler<UintValueChangedEventArgs> OnDrive;
        public event EventHandler<TrxEventArgs> OnTxEnableChanged;
        public event EventHandler<TrxStringValueChangedEventArgs> OnCwMacros;
        public event EventHandler<EventArgs> OnCwMacrosStop;
        public event EventHandler<UintValueChangedEventArgs> OnCwSpeedChanged;
        public event EventHandler<UintValueChangedEventArgs> OnCwMacroSpeedUp;
        public event EventHandler<UintValueChangedEventArgs> OnCwMacroSpeedDown;
        public event EventHandler<UintValueChangedEventArgs> OnCwMacrosDelayChanged;
        public event EventHandler<TrxEventArgs> OnRitEnableChanged;
        public event EventHandler<TrxEventArgs> OnXitEnableChanged;
        public event EventHandler<TrxEventArgs> OnSplitEnableChanged;
        public event EventHandler<TrxIntValueChangedEventArgs> OnRitOffsetChanged;
        public event EventHandler<TrxIntValueChangedEventArgs> OnXitOffsetChanged;
        public event EventHandler<ChannelEnableChangeEventArgs> OnChannelEnableChanged;
        public event EventHandler<RxFilterChangedEventArgs> OnRxFilterChanged;
        public event EventHandler<StringValueChangedEventArgs> OnCwMessageCallSign;
        public event EventHandler<VfoChangeEventArgs> OnVfoChange;
        public event EventHandler<UintValueChangedEventArgs> OnTuneDrive;

        private bool _start;
        private readonly Dictionary<string, ITciCommand> _commands;
        private readonly List<Transceiver> _transceivers;
        private bool _receiveOnly;
        private const double Tolerance = 0.00001;
    }
}
