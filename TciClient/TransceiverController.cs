using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ExpertElectronics.Tci.Events;
using ExpertElectronics.Tci.Interfaces;
using ExpertElectronics.Tci.TciCommands;

namespace ExpertElectronics.Tci;

/// <summary>
/// Controller orchestration class that translates high-level operations into TCI commands
/// and maintains state for transceivers and channels.
/// </summary>
public class TransceiverController : ITransceiverController, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TransceiverController"/> class.
    /// </summary>
    /// <param name="messageHandler">Message handler used to receive socket events and messages.</param>
    /// <param name="tciClient">The owning <see cref="ITciClient"/> instance used to send commands.</param>
    public TransceiverController(ITciMessageHandler messageHandler, ITciClient tciClient)
    {
        Debug.Assert(messageHandler != null);
        Debug.Assert(tciClient != null);

        _messageHandler = messageHandler;
        TciClient = tciClient;
        _transceivers = new();
        _messageHandler.OnSocketConnectionChanged += MessageHandler_OnSocketConnected;
        _messageHandler.OnSocketMessageReceived += MessageHandler_OnSocketMessageReceived;
        _commands = new();
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

    public ITransceiver GetTransceiver(uint transceiverPeriodicNumber)
    {
        return Transceivers.FirstOrDefault(_ => _.PeriodicNumber == transceiverPeriodicNumber);
    }

    public bool TxEnable(uint transceiverPeriodicNumber)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
        if (transceiver == null)
        {
            return false;
        }

        return transceiver.TxEnable;
    }

    public bool TxFootSwitch(uint transceiverPeriodicNumber)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
        if (transceiver == null)
        {
            return false;
        }

        return transceiver.TxFootSwitch;
    }

    /// <summary>
    /// Sends a command to start the transceiver system.
    /// </summary>
    /// <returns>A task that completes when the start command has been sent.</returns>
    public async Task StartTransceiver()
    {
        if (_start)
        {
            return;
        }
        await TciClient.SendMessageAsync($"{TciStartCommand.Name};");
    }

    /// <summary>
    /// Sends a command to stop the transceiver system.
    /// </summary>
    /// <returns>A task that completes when the stop command has been sent.</returns>
    public async Task StopTransceiver()
    {
        await TciClient.SendMessageAsync($"{TciStopCommand.Name};");
    }

    /// <summary>
    /// Requests the transceiver to set the DDS frequency.
    /// </summary>
    /// <param name="transceiverPeriodicNumber">The periodic number (index) of the target transceiver.</param>
    /// <param name="frequency">Frequency in Hz to set on the DDS.</param>
    /// <returns>A task that completes when the command has been sent.</returns>
    public async Task SetDdsFrequency(uint transceiverPeriodicNumber, double frequency)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
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
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
        if (transceiver == null)
        {
            return 0;
        }

        return transceiver.DdsFrequency;
    }

    /// <summary>
    /// Sets the IF filter frequency for a specific receiver channel.
    /// </summary>
    /// <param name="receiverPeriodicNumber">Receiver (transceiver) periodic number.</param>
    /// <param name="channelPeriodicNumber">Channel periodic number within the receiver.</param>
    /// <param name="frequency">IF frequency in Hz.</param>
    /// <returns>A task that completes when the command has been sent.</returns>
    public async Task IfFilter(uint receiverPeriodicNumber, uint channelPeriodicNumber, double frequency)
    {
        var transceiver = GetTransceiver(receiverPeriodicNumber);

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
        var transceiver = GetTransceiver(receiverPeriodicNumber);

        var channel = transceiver?.Channels.FirstOrDefault(_ => _.PeriodicNumber == channelPeriodicNumber);
        if (channel == null)
        {
            return 0;
        }

        return channel.IfFilter;
    }

    /// <summary>
    /// Enables or disables RIT for the specified transceiver.
    /// </summary>
    /// <param name="transceiverPeriodicNumber">The transceiver periodic number.</param>
    /// <param name="state">True to enable RIT; false to disable.</param>
    /// <returns>A task that completes when the command has been sent.</returns>
    public async Task RitEnable(uint transceiverPeriodicNumber, bool state)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
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
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
        return transceiver != null && transceiver.Rit;
    }

    /// <summary>
    /// Changes the modulation mode for a transceiver.
    /// </summary>
    /// <param name="transceiverPeriodicNumber">The transceiver periodic number.</param>
    /// <param name="mode">The modulation mode to set (must exist in <see cref="ModulationsList"/>).</param>
    /// <returns>A task that completes when the command has been sent.</returns>
    public async Task Modulation(uint transceiverPeriodicNumber, string mode)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
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
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
        return transceiver == null ? string.Empty : transceiver.Modulation;
    }

    /// <summary>
    /// Enables or disables receiver audio for the specified receiver.
    /// </summary>
    /// <param name="receiverPeriodicNumber">Receiver (transceiver) periodic number.</param>
    /// <param name="state">True to enable; false to disable.</param>
    /// <returns>A task that completes when the command has been sent.</returns>
    public async Task RxEnable(uint receiverPeriodicNumber, bool state)
    {
        var transceiver = GetTransceiver(receiverPeriodicNumber);
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
        var transceiver = GetTransceiver(receiverPeriodicNumber);
        return transceiver?.RxEnable ?? false;
    }

    /// <summary>
    /// Enables or disables XIT for the specified transceiver.
    /// </summary>
    /// <param name="transceiverPeriodicNumber">The transceiver periodic number.</param>
    /// <param name="state">True to enable; false to disable.</param>
    /// <returns>A task that completes when the command has been sent.</returns>
    public async Task XitEnable(uint transceiverPeriodicNumber, bool state)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
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
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
        return transceiver?.Xit ?? false;
    }

    /// <summary>
    /// Enables or disables split operation on the specified transceiver.
    /// </summary>
    /// <param name="transceiverPeriodicNumber">The transceiver periodic number.</param>
    /// <param name="state">True to enable split; false to disable.</param>
    /// <returns>A task that completes when the command has been sent.</returns>
    public async Task SplitEnable(uint transceiverPeriodicNumber, bool state)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
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
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
        return transceiver?.Split ?? false;
    }

    /// <summary>
    /// Sets the XIT offset frequency for the specified transceiver.
    /// </summary>
    /// <param name="transceiverPeriodicNumber">The transceiver periodic number.</param>
    /// <param name="offsetFrequencyInHz">Offset frequency in Hz.</param>
    /// <returns>A task that completes when the command has been sent.</returns>
    public async Task XitOffset(uint transceiverPeriodicNumber, int offsetFrequencyInHz)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
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
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
        return transceiver?.XitOffset ?? 0;
    }

    /// <summary>
    /// Enables or disables a channel on a specified transceiver.
    /// </summary>
    /// <param name="transceiverPeriodicNumber">The transceiver periodic number.</param>
    /// <param name="channelNumber">The channel periodic number to target.</param>
    /// <param name="state">True to enable the channel; false to disable.</param>
    /// <returns>A task that completes when the command has been sent.</returns>
    public async Task ChannelEnable(uint transceiverPeriodicNumber, uint channelNumber, bool state)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);

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
        var transceiver = GetTransceiver(transceiverPeriodicNumber);

        var channel = transceiver?.Channels.FirstOrDefault(_ => _.PeriodicNumber == channelNumber);
        return channel != null && channel.Enable;
    }

    /// <summary>
    /// Requests RX filter band information for a transceiver.
    /// </summary>
    /// <param name="transceiverPeriodicNumber">The transceiver periodic number.</param>
    /// <returns>A task that completes when the request has been sent.</returns>
    public async Task RxFilter(uint transceiverPeriodicNumber)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
        if (transceiver != null)
        {
            await TciClient.SendMessageAsync($"{TciRxFilterBandsCommand.Name}:{transceiverPeriodicNumber};");
        }
    }

    public int RxFilterLowLimit(uint transceiverPeriodicNumber)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
        return transceiver?.RxFilterLowLimit ?? 0;
    }

    public int RxFilterHighLimit(uint transceiverPeriodicNumber)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
        return transceiver?.RxFilterHighLimit ?? 0;
    }

    /// <summary>
    /// Requests the S-meter reading for a specific receiver channel.
    /// </summary>
    /// <param name="transceiverPeriodicNumber">Receiver (transceiver) periodic number.</param>
    /// <param name="channelPeriodicNumber">Channel periodic number within the receiver.</param>
    /// <returns>A task that completes when the request has been sent.</returns>
    public async Task ReadRxSMeter(uint transceiverPeriodicNumber, uint channelPeriodicNumber)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);

        var channel = transceiver?.Channels.FirstOrDefault(_ => _.PeriodicNumber == channelPeriodicNumber);
        if (channel == null)
        {
            return;
        }

        await TciClient.SendMessageAsync($"{TciRxSMeterCommand.Name}:{transceiverPeriodicNumber},{channel.PeriodicNumber};");
    }

    public int RxSMeter(uint transceiverPeriodicNumber, uint channelPeriodicNumber)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);

        var channel = transceiver?.Channels.FirstOrDefault(_ => _.PeriodicNumber == channelPeriodicNumber);
        return channel?.RxSMeter ?? 0;
    }

    /// <summary>
    /// Sets the CW macro speed (1..60).
    /// </summary>
    /// <param name="value">Speed value in the range 1..60.</param>
    /// <returns>A task that completes when the command has been sent.</returns>
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

    /// <summary>
    /// Increments the CW macro speed by sending the corresponding command.
    /// </summary>
    /// <param name="value">Target value (1..60).</param>
    /// <returns>A task that completes when the command has been sent.</returns>
    public async Task CwMacroSpeedUp(uint value)
    {
        if (value < 1 || value > 60)
        {
            return;
        }
        await TciClient.SendMessageAsync($"{TciCwMacroSpeedDownCommand.Name}:{value};");
    }

    /// <summary>
    /// Decrements the CW macro speed by sending the corresponding command.
    /// </summary>
    /// <param name="value">Target value (1..60).</param>
    /// <returns>A task that completes when the command has been sent.</returns>
    public async Task CwMacroSpeedDown(uint value)
    {
        if (value < 1 || value > 60)
        {
            return;
        }

        await TciClient.SendMessageAsync($"{TciCwMacroSpeedUpCommand.Name}:{value};");
    }

    /// <summary>
    /// Sets the delay for CW macros (10..1000 ms).
    /// </summary>
    /// <param name="value">Delay in milliseconds.</param>
    /// <returns>A task that completes when the command has been sent.</returns>
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

    /// <summary>
    /// Enables or disables transmission (TRX) on a transceiver.
    /// </summary>
    /// <param name="transceiverPeriodicNumber">The transceiver periodic number.</param>
    /// <param name="enable">True to enable transmission; false to disable.</param>
    /// <param name="signalSource">Optional signal source (default "mic").</param>
    /// <returns>A task that completes when the command has been sent.</returns>
    public async Task Trx(uint transceiverPeriodicNumber, bool enable, string signalSource = "mic")
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
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
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
        return transceiver?.Trx ?? false;
    }

    /// <summary>
    /// Enables or disables tune mode on the specified transceiver.
    /// </summary>
    /// <param name="transceiverPeriodicNumber">The transceiver periodic number.</param>
    /// <param name="enable">True to enable tuning; false to disable.</param>
    /// <returns>A task that completes when the command has been sent.</returns>
    public async Task Tune(uint transceiverPeriodicNumber, bool enable)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
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
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
        return transceiver?.Tune ?? false;
    }

    /// <summary>
    /// Sets the transmit drive level (0..100).
    /// </summary>
    /// <param name="level">Drive level (0..100).</param>
    /// <returns>A task that completes when the command has been sent.</returns>
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


    /// <summary>
    /// Sets the tune drive level (0..100).
    /// </summary>
    /// <param name="level">Tune drive level (0..100).</param>
    /// <returns>A task that completes when the command has been sent.</returns>
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

    /// <summary>
    /// Starts IQ streaming for the specified transceiver.
    /// </summary>
    /// <param name="transceiverPeriodicNumber">The transceiver periodic number.</param>
    /// <returns>A task that completes when the start command has been sent.</returns>
    public async Task IqStart(uint transceiverPeriodicNumber)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
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

    /// <summary>
    /// Stops IQ streaming for the specified transceiver.
    /// </summary>
    /// <param name="transceiverPeriodicNumber">The transceiver periodic number.</param>
    /// <returns>A task that completes when the stop command has been sent.</returns>
    public async Task IqStop(uint transceiverPeriodicNumber)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
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

    /// <summary>
    /// Sets the IQ sample rate for streaming audio (valid values: 48000, 96000, 192000).
    /// </summary>
    /// <param name="sampleRateInHz">Sample rate in Hz.</param>
    /// <returns>A task that completes when the command has been sent.</returns>
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

    /// <summary>
    /// Starts audio streaming for the specified transceiver.
    /// </summary>
    /// <param name="transceiverPeriodicNumber">The transceiver periodic number.</param>
    /// <returns>A task that completes when the start command has been sent.</returns>
    public async Task AudioStart(uint transceiverPeriodicNumber)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
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

    /// <summary>
    /// Stops audio streaming for the specified transceiver.
    /// </summary>
    /// <param name="transceiverPeriodicNumber">The transceiver periodic number.</param>
    /// <returns>A task that completes when the stop command has been sent.</returns>
    public async Task AudioStop(uint transceiverPeriodicNumber)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
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

    /// <summary>
    /// Sets the audio sample rate for streaming (valid values: 8000, 12000, 24000, 48000).
    /// </summary>
    /// <param name="sampleRateInHz">Sample rate in Hz.</param>
    /// <returns>A task that completes when the command has been sent.</returns>
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

    /// <summary>
    /// Sends a spot message to the server.
    /// </summary>
    /// <param name="callSign">Call sign to spot.</param>
    /// <param name="mode">Operating mode (e.g., SSB, CW).</param>
    /// <param name="frequencyInHz">Frequency in Hz for the spot.</param>
    /// <param name="color">Color used for the spot marker in the UI.</param>
    /// <param name="additionalText">Optional additional text.</param>
    /// <returns>A task that completes when the spot command has been sent.</returns>
    public async Task Spot(string callSign, string mode, long frequencyInHz, Color color, string additionalText)
    {
        var colorToUi = color.ToRgbString();
        await TciClient.SendMessageAsync($"{TciSpotCommand.Name}:{callSign},{mode},{frequencyInHz},{colorToUi},{additionalText};");
    }

    /// <summary>
    /// Requests deletion of a spot by call sign.
    /// </summary>
    /// <param name="callSign">Call sign of the spot to delete.</param>
    /// <returns>A task that completes when the delete command has been sent.</returns>
    public async Task SpotDelete(string callSign)
    {
        await TciClient.SendMessageAsync($"{TciSpotDeleteCommand.Name}:{callSign};");
    }

    /// <summary>
    /// Clears all spots on the server/UI.
    /// </summary>
    /// <returns>A task that completes when the clear command has been sent.</returns>
    public async Task SpotClear()
    {
        await TciClient.SendMessageAsync($"{TciSpotClearCommand.Name};");
    }

    /// <summary>
    /// Sets the audio volume (in dB).
    /// </summary>
    /// <param name="volumeValueIndB">Volume in dB (typically between -60 and 0).</param>
    /// <returns>A task that completes when the command has been sent.</returns>
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

    /// <summary>
    /// Enables or disables squelch on the specified transceiver.
    /// </summary>
    /// <param name="transceiverPeriodicNumber">The transceiver periodic number.</param>
    /// <param name="state">True to enable squelch; false to disable.</param>
    /// <returns>A task that completes when the command has been sent.</returns>
    public async Task SquelchEnable(uint transceiverPeriodicNumber, bool state)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
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
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
        return transceiver?.Squelch ?? false;
    }

    /// <summary>
    /// Sets the squelch threshold for a transceiver.
    /// </summary>
    /// <param name="transceiverPeriodicNumber">The transceiver periodic number.</param>
    /// <param name="thresholdIndB">Threshold in dB (valid range -140..0).</param>
    /// <returns>A task that completes when the command has been sent.</returns>
    public async Task SquelchLevel(uint transceiverPeriodicNumber, int thresholdIndB)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
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
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
        return transceiver?.SquelchThreshold ?? 0;
    }

    /// <summary>
    /// Sets the VFO (tuning frequency) for a given transceiver channel.
    /// </summary>
    /// <param name="transceiverPeriodicNumber">The transceiver periodic number.</param>
    /// <param name="channelPeriodicNumber">The channel periodic number within the transceiver.</param>
    /// <param name="tuningFrequencyInHz">Target tuning frequency in Hz.</param>
    /// <returns>A task that completes when the command has been sent.</returns>
    public async Task Vfo(uint transceiverPeriodicNumber, uint channelPeriodicNumber, long tuningFrequencyInHz)
    {

        var transceiver = GetTransceiver(transceiverPeriodicNumber);

        var channel = transceiver?.Channels.FirstOrDefault(_ => _.PeriodicNumber == channelPeriodicNumber);
        if (channel == null)
        {
            return;
        }

        if (channel.Vfo == tuningFrequencyInHz)
        {
            return;
        }

        await TciClient.SendMessageAsync($"{TciVfoCommand.Name}:{transceiverPeriodicNumber},{channelPeriodicNumber},{tuningFrequencyInHz};");
    }

    /// <summary>
    /// Reads the current VFO (tuning frequency) for a given channel.
    /// </summary>
    /// <param name="transceiverPeriodicNumber">The transceiver periodic number.</param>
    /// <param name="channelPeriodicNumber">The channel periodic number.</param>
    /// <returns>The tuning frequency in Hz, or 0 if the channel does not exist.</returns>
    public long Vfo(uint transceiverPeriodicNumber, uint channelPeriodicNumber)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
        var channel = transceiver?.Channels.FirstOrDefault(_ => _.PeriodicNumber == channelPeriodicNumber);
        return channel?.Vfo ?? 0;
    }


    /// <summary>
    /// Sets the global mute state.
    /// </summary>
    /// <param name="state">True to mute; false to unmute.</param>
    /// <returns>A task that completes when the command has been sent.</returns>
    public async Task SetMute(bool state)
    {
        if (Mute == state)
        {
            return;
        }

        await TciClient.SendMessageAsync($"{TciMuteCommand.Name}:{state};");
    }

    /// <summary>
    /// Enables or disables RX mute for a receiver.
    /// </summary>
    /// <param name="receiverPeriodicNumber">Receiver (transceiver) periodic number.</param>
    /// <param name="state">True to mute RX; false to unmute.</param>
    /// <returns>A task that completes when the command has been sent.</returns>
    public async Task RxMute(uint receiverPeriodicNumber, bool state)
    {
        var transceiver = GetTransceiver(receiverPeriodicNumber);
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
        var transceiver = GetTransceiver(receiverPeriodicNumber);
        return transceiver?.RxMute ?? false;
    }


    public bool IsStarted()
    {
        return _start;
    }


    /// <summary>
    /// Sets the RIT offset for a transceiver.
    /// </summary>
    /// <param name="transceiverPeriodicNumber">The transceiver periodic number.</param>
    /// <param name="value">RIT offset value in Hz.</param>
    /// <returns>A task that completes when the command has been sent.</returns>
    public async Task RitOffset(uint transceiverPeriodicNumber, int value)
    {
        var transceiver = GetTransceiver(transceiverPeriodicNumber);
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

    /// <summary>
    /// Sends a CW message composed from parts.
    /// </summary>
    /// <param name="transceiverPeriodicNumber">The transceiver periodic number.</param>
    /// <param name="before">Text to send before the call sign.</param>
    /// <param name="callSign">Call sign to include in the CW message.</param>
    /// <param name="after">Text to send after the call sign.</param>
    /// <returns>A task that completes when the message command has been sent.</returns>
    public async Task CwMessage(uint transceiverPeriodicNumber, string before, string callSign, string after)
    {
        await TciClient.SendMessageAsync($"cw_msg:{transceiverPeriodicNumber},{before},{callSign},{after};");
    }

    /// <summary>
    /// Adds a CW message call sign to the remote server.
    /// </summary>
    /// <param name="callSign">Call sign to add.</param>
    /// <returns>A task that completes when the command has been sent.</returns>
    public async Task AddCwMessageCallSign(string callSign)
    {
        await TciClient.SendMessageAsync($"callsign_send:{callSign};");
    }

    /// <summary>
    /// Sets CW macros text for a transceiver.
    /// </summary>
    /// <param name="transceiverPeriodicNumber">The transceiver periodic number.</param>
    /// <param name="text">Macros text to set.</param>
    /// <returns>A task that completes when the command has been sent.</returns>
    public async Task SetMacros(uint transceiverPeriodicNumber, string text)
    {
        await TciClient.SendMessageAsync($"cw_macros: {transceiverPeriodicNumber},{text};");
    }

    /// <summary>
    /// Stops CW macros playback on the server.
    /// </summary>
    /// <returns>A task that completes when the stop command has been sent.</returns>
    public async Task SetCwMacrosStop()
    {
        await TciClient.SendMessageAsync($"cw_macros_stop;");
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

    public async Task VfoAToB(uint transceiverPeriodicNumber)
    {
        var vfoA = Vfo(transceiverPeriodicNumber, 0);

        await Vfo(transceiverPeriodicNumber, 1, vfoA);
    }

    public async Task VfoBToA(uint transceiverPeriodicNumber)
    {
        var vfoB = Vfo(transceiverPeriodicNumber, 1);
        await Vfo(transceiverPeriodicNumber, 0, vfoB);
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

