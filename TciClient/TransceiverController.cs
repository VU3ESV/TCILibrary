using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
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

        public ITciClient TciClient { get; }

        public string SoftwareName { get; set; }

        public string SoftwareVersion { get; set; }

        public long VfoMin
        {
            get => _vfoMin;
            set
            {
                _vfoMin = value;
                OnVfoLimitsChanged?.Invoke(this, new VfoLimitsChangedEventArgs(_vfoMin, _vfoMax));
            }
        }

        public long VfoMax
        {
            get => _vfoMax;

            set
            {
                _vfoMax = value;
                OnVfoLimitsChanged?.Invoke(this, new VfoLimitsChangedEventArgs(_vfoMin, _vfoMax));
            }
        }

        public long IfMin
        {
            get => _ifMin;
            set
            {
                _ifMin = value;
                OnIfLimitsChanged?.Invoke(this, new IfLimitsChangedEventArgs(_ifMin, _ifMax));
            }
        }

        public long IfMax
        {
            get => _ifMax;
            set
            {
                _ifMax = value;
                OnIfLimitsChanged?.Invoke(this, new IfLimitsChangedEventArgs(_ifMin, _ifMax));
            }
        }

        public uint TrxCount { get; set; }

        public uint ChannelsCount { get; set; }

        public string Device { get; set; }

        public bool Mute
        {
            get => _mute;
            set
            {
                _mute = value;
                OnMute?.Invoke(this, new StateChangeEventArgs(_mute));
            }
        }

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

        public IEnumerable<string> ModulationsList { get; set; }

        public uint CwMacroSpeed
        {
            get => _cwMacroSpeed;
            set
            {
                _cwMacroSpeed = value;
                OnCwSpeedChanged?.Invoke(this, new UintValueChangedEventArgs(_cwMacroSpeed));
            }
        }


        public uint CwMacrosDelay
        {
            get => _cwMacroDelay;
            set
            {
                _cwMacroDelay = value;
                OnCwMacrosDelayChanged?.Invoke(this, new UintValueChangedEventArgs(_cwMacroDelay));
            }
        }

        public uint Drive
        {
            get => _drive;
            set
            {
                _drive = value;
                OnDrive?.Invoke(this, new UintValueChangedEventArgs(_drive));
            }
        }

        public uint TuneDrive
        {
            get => _tuneDrive;
            set
            {
                _tuneDrive = value;
                OnTuneDrive?.Invoke(this, new UintValueChangedEventArgs(_tuneDrive));
            }
        }

        public TransceiverConnectionState ConnectionState { get; private set; }
        public bool Ready { get; set; }

        public int Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                OnVolumeChanged?.Invoke(this, new IntValueChangedEventArgs(_volume));
            }
        }

        public uint AudioSampleRate
        {
            get => _audioSampleRate;
            set
            {
                _audioSampleRate = value;
                OnAudioSampleRateChanged?.Invoke(this, new UintValueChangedEventArgs(_audioSampleRate));
            }
        }

        public uint IqSampleRate
        {
            get => _iqSampleRate;
            set
            {
                _iqSampleRate = value;
                OnIqOutSampleRateChanged?.Invoke(this, new UintValueChangedEventArgs(_iqSampleRate));
            }
        }

        internal void AddModulationList(List<string> modulationList)
        {
            ModulationsList = modulationList;
        }

        public IEnumerable<ITransceiver> Transceivers => _transceivers;

        public bool Start
        {
            get => _start;
            set
            {
                _start = value;
                OnStarted?.Invoke(this, new EventArgs());
            }
        }
        public bool Stop
        {
            get => _stop;
            set
            {
                _stop = value;
                OnStopped?.Invoke(this, new EventArgs());
            }
        }
        public uint CwMacrosSpeedDown
        {
            get => _cwMacroSpeedUp;
            set
            {
                _cwMacroSpeedUp = value;
                OnCwMacroSpeedDown?.Invoke(this, new UintValueChangedEventArgs(_cwMacroSpeedUp));
            }
        }
        public uint CwMacrosSpeedUp
        {
            get => _cwMacroSpeedUp;
            set
            {
                _cwMacroSpeedUp = value;
                OnCwMacroSpeedUp?.Invoke(this, new UintValueChangedEventArgs(_cwMacroSpeedUp));
            }
        }

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

        public bool TxEnable(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return false;
            }

            return transceiver.TxEnable;
        }

        public bool TxFootSwitch(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return false;
            }

            return transceiver.TxFootSwitch;
        }

        public async Task StartTransceiver()
        {
            if (_start)
            {
                return;
            }
            await TciClient.SendMessageAsync($"{TciStartCommand.Name};");
        }

        public async Task StopTransceiver()
        {
            await TciClient.SendMessageAsync($"{TciStopCommand.Name};");
        }

        public async Task SetDdsFrequency(uint transceiverPeriodicNumber, double frequency)
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

            await TciClient.SendMessageAsync($"{TciDdsCommand.Name}:{transceiverPeriodicNumber},{frequency};");
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

        public async Task IfFilter(uint receiverPeriodicNumber, uint channelPeriodicNumber, double frequency)
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

            await TciClient.SendMessageAsync($"{TciIfCommand.Name}:{receiverPeriodicNumber},{channel.PeriodicNumber},{frequency};");
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

        public async Task RitEnable(uint transceiverPeriodicNumber, bool state)
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

            if (transceiverPeriodicNumber < TrxCount)
            {
                await TciClient.SendMessageAsync($"{TciRitEnableCommand.Name}:{transceiverPeriodicNumber},{state};");
            }
        }
        public bool RitEnable(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            return transceiver != null && transceiver.Rit;
        }

        public async Task Modulation(uint transceiverPeriodicNumber, string mode)
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
            if (transceiver.PeriodicNumber >= TrxCount)
            {
                return;
            }
            if (!ModulationsList.Contains(mode))
            {
                return;
            }

            await TciClient.SendMessageAsync($"{TciModulationCommand.Name}:{transceiverPeriodicNumber},{mode};");
        }

        public string Modulation(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            return transceiver == null ? string.Empty : transceiver.Modulation;
        }

        public async Task RxEnable(uint receiverPeriodicNumber, bool state)
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

            if (receiverPeriodicNumber < TrxCount)
            {
                await TciClient.SendMessageAsync($"{TciRxEnableCommand.Name}:{receiverPeriodicNumber},{state};");
            }
        }

        public bool RxEnable(uint receiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(receiverPeriodicNumber);
            return transceiver?.RxEnable ?? false;
        }

        public async Task XitEnable(uint transceiverPeriodicNumber, bool state)
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
            await TciClient.SendMessageAsync($"{TciXitEnableCommand.Name}:{transceiverPeriodicNumber},{state};");
        }

        public bool XitEnable(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            return transceiver?.Xit ?? false;
        }

        public async Task SplitEnable(uint transceiverPeriodicNumber, bool state)
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
            if (transceiverPeriodicNumber < TrxCount)
            {
                await TciClient.SendMessageAsync($"{TciSplitEnableCommand.Name}:{transceiverPeriodicNumber},{state};");
            }
        }

        public bool SplitEnable(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            return transceiver?.Split ?? false;
        }

        public async Task XitOffset(uint transceiverPeriodicNumber, int offsetFrequencyInHz)
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

            await TciClient.SendMessageAsync($"{TciXitOffsetCommand.Name}:{transceiverPeriodicNumber},{offsetFrequencyInHz};");
        }

        public int XitOffset(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            return transceiver?.XitOffset ?? 0;
        }

        public async Task ChannelEnable(uint transceiverPeriodicNumber, uint channelNumber, bool state)
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

            await TciClient.SendMessageAsync($"{TciRxChannelEnableCommand.Name}:{transceiverPeriodicNumber},{channel.PeriodicNumber},{state};");
        }

        public bool ChannelEnable(uint transceiverPeriodicNumber, uint channelNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);

            var channel = transceiver?.Channels.FirstOrDefault(_ => _.PeriodicNumber == channelNumber);
            return channel != null && channel.Enable;
        }

        public async Task RxFilter(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver != null)
            {
                await TciClient.SendMessageAsync($"{TciRxFilterBandsCommand.Name}:{transceiverPeriodicNumber};");
            }
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

        public async Task ReadRxSMeter(uint transceiverPeriodicNumber, uint channelPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);

            var channel = transceiver?.Channels.FirstOrDefault(_ => _.PeriodicNumber == channelPeriodicNumber);
            if (channel == null)
            {
                return;
            }

            await TciClient.SendMessageAsync($"{TciRxSMeterCommand.Name}:{transceiverPeriodicNumber},{channel.PeriodicNumber};");
        }

        public int RxSMeter(uint transceiverPeriodicNumber, uint channelPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);

            var channel = transceiver?.Channels.FirstOrDefault(_ => _.PeriodicNumber == channelPeriodicNumber);
            return channel?.RxSMeter ?? 0;
        }

        public async Task SetCwMacroSpeed(uint value)
        {
            if (value < 1 || value > 60)
            {
                return;
            }

            if (CwMacroSpeed == value)
            {
                return;
            }
            await TciClient.SendMessageAsync($"{TciCwMacrosSpeedCommand.Name}:{value};");
        }

        public async Task CwMacroSpeedUp(uint value)
        {
            if (value < 1 || value > 60)
            {
                return;
            }
            await TciClient.SendMessageAsync($"{TciCwMacroSpeedDownCommand.Name}:{value};");
        }

        public async Task CwMacroSpeedDown(uint value)
        {
            if (value < 1 || value > 60)
            {
                return;
            }

            await TciClient.SendMessageAsync($"{TciCwMacroSpeedUpCommand.Name}:{value};");
        }

        public async Task SetCwMacrosDelay(uint value)
        {
            if (value < 10 || value > 1000)
            {
                return;
            }

            if (CwMacrosDelay == value)
            {
                return;
            }
            await TciClient.SendMessageAsync($"{TciCwMacrosDelayCommand.Name}:{value};");
        }

        public async Task Trx(uint transceiverPeriodicNumber, bool enable, string signalSource = "mic")
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return;
            }

            if (transceiver.Trx == enable)
            {
                return;
            }
            await TciClient.SendMessageAsync($"{TciTrxCommand.Name}:{transceiverPeriodicNumber},{enable};");
        }

        public bool Trx(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            return transceiver?.Trx ?? false;
        }

        public async Task Tune(uint transceiverPeriodicNumber, bool enable)
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
            await TciClient.SendMessageAsync($"{TciTuneCommand.Name}:{transceiverPeriodicNumber},{enable};");
        }

        public bool Tune(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            return transceiver?.Tune ?? false;
        }

        public async Task SetDrive(uint level)
        {
            if (level > 100)
            {
                return;
            }

            if (Drive == level)
            {
                return;
            }
            await TciClient.SendMessageAsync($"{TciDriveCommand.Name}:{level};");
        }


        public async Task SetTuneDrive(uint level)
        {
            if (level > 100)
            {
                return;
            }

            if (TuneDrive == level)
            {
                return;
            }

            await TciClient.SendMessageAsync($"{TciTuneDriveCommand.Name}:{level};");
        }

        public async Task IqStart(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return;
            }

            if (transceiver.IqEnable)
            {
                return;
            }

            if (transceiverPeriodicNumber < TrxCount)
            {
                await TciClient.SendMessageAsync($"{TciIqStartCommand.Name}:{transceiverPeriodicNumber};");
            }
        }

        public async Task IqStop(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            if (transceiver == null)
            {
                return;
            }

            if (transceiver.IqEnable == false)
            {
                return;
            }

            if (transceiverPeriodicNumber < TrxCount)
            {
                await TciClient.SendMessageAsync($"{TciIqStopCommand.Name}:{transceiverPeriodicNumber};");
            }
        }

        public async Task SetIqSampleRate(uint sampleRateInHz)
        {
            if (IqSampleRate == sampleRateInHz)
            {
                return;
            }

            if (sampleRateInHz == 48000 || sampleRateInHz == 96000 || sampleRateInHz == 192000)
            {
                await TciClient.SendMessageAsync($"{TciIqSampleRateCommand.Name}:{sampleRateInHz};");
            }
        }

        public async Task AudioStart(uint transceiverPeriodicNumber)
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
            if (transceiverPeriodicNumber < TrxCount)
            {
                await TciClient.SendMessageAsync($"{TciAudioStartCommand.Name}:{transceiverPeriodicNumber};");
            }
        }

        public async Task AudioStop(uint transceiverPeriodicNumber)
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

            if (transceiverPeriodicNumber < TrxCount)
            {
                await TciClient.SendMessageAsync($"{TciAudioStopCommand.Name}:{transceiverPeriodicNumber};");
            }
        }

        public async Task SetAudioSampleRate(uint sampleRateInHz)
        {
            if (AudioSampleRate == sampleRateInHz)
            {
                return;
            }

            if (sampleRateInHz == 8000 || sampleRateInHz == 12000 || sampleRateInHz == 24000 || sampleRateInHz == 48000)
            {
                await TciClient.SendMessageAsync($"{TciAudioSampleRateCommand.Name}:{sampleRateInHz};");
            }
        }

        public async Task Spot(string callSign, string mode, long frequencyInHz, Color color, string additionalText)
        {
            var colorToUi = color.ToRgbString();
            await TciClient.SendMessageAsync($"{TciSpotCommand.Name}:{callSign},{mode},{frequencyInHz},{colorToUi},{additionalText};");
        }

        public async Task SpotDelete(string callSign)
        {
            await TciClient.SendMessageAsync($"{TciSpotDeleteCommand.Name}:{callSign};");
        }

        public async Task SpotClear()
        {
            await TciClient.SendMessageAsync($"{TciSpotClearCommand.Name};");
        }

        public async Task SetVolume(int volumeValueIndB)
        {
            if (Volume == volumeValueIndB)
            {
                return;
            }

            if (volumeValueIndB >= -60 && volumeValueIndB <= 0)
            {
                await TciClient.SendMessageAsync($"{TciVolumeCommand.Name}:{volumeValueIndB};");
            }
        }

        public async Task SquelchEnable(uint transceiverPeriodicNumber, bool state)
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

            if (transceiver.PeriodicNumber < TrxCount)
            {
                await TciClient.SendMessageAsync($"{TciSqlEnableCommand.Name}:{transceiverPeriodicNumber},{state};");
            }
        }

        public bool SquelchEnable(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            return transceiver?.Squelch ?? false;
        }

        public async Task SquelchLevel(uint transceiverPeriodicNumber, int thresholdIndB)
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

            if (thresholdIndB >= -140 && thresholdIndB <= 0)
            {
                await TciClient.SendMessageAsync($"{TciSqlLevelCommand.Name}:{transceiverPeriodicNumber},{thresholdIndB};");
            }
        }

        public int SquelchLevel(uint transceiverPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            return transceiver?.SquelchThreshold ?? 0;
        }

        public async Task Vfo(uint transceiverPeriodicNumber, uint channelPeriodicNumber, long tuningFrequencyInHz)
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

            await TciClient.SendMessageAsync($"{TciVfoCommand.Name}:{transceiverPeriodicNumber},{channel},{channel.Vfo};");
        }

        public long Vfo(uint transceiverPeriodicNumber, uint channelPeriodicNumber)
        {
            var transceiver = GeTransceiver(transceiverPeriodicNumber);
            var channel = transceiver?.Channels.FirstOrDefault(_ => _.PeriodicNumber == channelPeriodicNumber);
            return channel?.Vfo ?? 0;
        }


        public async Task SetMute(bool state)
        {
            if (Mute == state)
            {
                return;
            }

            await TciClient.SendMessageAsync($"{TciMuteCommand.Name}:{state};");
        }

        public async Task RxMute(uint receiverPeriodicNumber, bool state)
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

            await TciClient.SendMessageAsync($"{TciRxMuteCommand.Name}:{receiverPeriodicNumber},{state};");
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


        public async Task RitOffset(uint transceiverPeriodicNumber, int value)
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

            await TciClient.SendMessageAsync($"{TciRitOffsetCommand.Name}:{transceiverPeriodicNumber},{value};");
        }

        public Task CwMessage(uint transceiverPeriodicNumber, string before, string callSign, string after)
        {
            // ToDo
            return Task.CompletedTask;
        }

        public Task AddCwMessageCallSign(string callSign)
        {
            // ToDo
            return Task.CompletedTask;
        }

        public Task SetMacros(uint transceiverPeriodicNumber, string text)
        {
            // ToDo
            return Task.CompletedTask;
        }

        public Task SetCwMacrosStop()
        {
            // ToDo
            return Task.CompletedTask;           
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public event EventHandler<EventArgs> OnStarted;
        public event EventHandler<EventArgs> OnStopped;
        public event EventHandler<StateChangeEventArgs> OnMute;
        public event EventHandler<VfoLimitsChangedEventArgs> OnVfoLimitsChanged;
        public event EventHandler<IfLimitsChangedEventArgs> OnIfLimitsChanged;
        public event EventHandler<UintValueChangedEventArgs> OnIqOutSampleRateChanged;
        public event EventHandler<IntValueChangedEventArgs> OnVolumeChanged;
        public event EventHandler<UintValueChangedEventArgs> OnAudioSampleRateChanged;
        public event EventHandler<UintValueChangedEventArgs> OnDrive;
        public event EventHandler<UintValueChangedEventArgs> OnTuneDrive;
        public event EventHandler<UintValueChangedEventArgs> OnCwSpeedChanged;
        public event EventHandler<UintValueChangedEventArgs> OnCwMacroSpeedUp;
        public event EventHandler<UintValueChangedEventArgs> OnCwMacroSpeedDown;
        public event EventHandler<UintValueChangedEventArgs> OnCwMacrosDelayChanged;

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

        private bool _start;
        private readonly Dictionary<string, ITciCommand> _commands;
        private readonly List<Transceiver> _transceivers;
        private long _ifMax;
        private bool _receiveOnly;
        private long _vfoMin;
        private long _vfoMax;
        private long _ifMin;
        private bool _mute;
        private uint _cwMacroSpeed;
        private uint _cwMacroDelay;
        private uint _drive;
        private uint _tuneDrive;
        private int _volume;
        private uint _audioSampleRate;
        private uint _iqSampleRate;
        private bool _stop;
        private uint _cwMacroSpeedUp;
        private const double Tolerance = 0.00001;
    }
}
