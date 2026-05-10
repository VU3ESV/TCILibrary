using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using ExpertElectronics.Tci.Client.Audio;
using ExpertElectronics.Tci.Events;
using ExpertElectronics.Tci.Interfaces;
using ExpertElectronics.Tci.Streaming;
using Silk.NET.OpenAL;

namespace ExpertElectronics.Tci.Client.ViewModels;

/// <summary>
/// One per discovered transceiver. Owns its own OpenAL playback queue and (optionally) a
/// mic capture pipeline for TX.
/// </summary>
public sealed class TransceiverViewModel : ViewModelBase, IDisposable
{
    private readonly ITransceiverController _controller;
    private readonly ITransceiver _transceiver;
    private readonly AL _al;
    private readonly bool _enableTx;
    private readonly OpenAlPlayback _playback;
    private OpenAlCapture? _capture;
    private bool _rxAudioRunning;
    private bool _txAudioRunning;
    private string _status = "Idle";
    private long _bytesReceived;
    private uint _sampleRate;
    private string _modulation = "—";
    private long _vfoA;
    private long _vfoB;
    private long _ddsFrequency;

    public TransceiverViewModel(ITransceiverController controller, ITransceiver transceiver, AL al, bool enableTx)
    {
        _controller = controller;
        _transceiver = transceiver;
        _al = al;
        _enableTx = enableTx;
        _playback = new OpenAlPlayback(al);

        // Snapshot current state — by the time this card is built, the server has already
        // sent DDS / VFO / MODULATION for this transceiver, so reading them via events alone
        // would miss the initial values.
        _modulation = string.IsNullOrWhiteSpace(_transceiver.Modulation) ? "—" : _transceiver.Modulation;
        _ddsFrequency = (long)_transceiver.DdsFrequency;
        foreach (var ch in _transceiver.Channels)
        {
            if (ch.PeriodicNumber == 0) _vfoA = ch.Vfo;
            else if (ch.PeriodicNumber == 1) _vfoB = ch.Vfo;
        }

        _controller.OnRxAudioStreamReceived += Controller_OnRxAudioStreamReceived;
        _controller.OnTxChronoReceived += Controller_OnTxChronoReceived;
        _transceiver.OnModulationChanged += (s, e) => Dispatcher.UIThread.Post(() =>
            Modulation = string.IsNullOrWhiteSpace(e.Value) ? "—" : e.Value);
        _transceiver.OnDdsFreqChanged += (s, e) => Dispatcher.UIThread.Post(() => DdsFrequency = (long)e.Value);

        foreach (var ch in _transceiver.Channels)
        {
            var captured = ch;
            captured.OnVfoChange += (s, e) =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (e.Channel == 0) VfoA = e.Vfo;
                    else if (e.Channel == 1) VfoB = e.Vfo;
                });
            };
        }
    }

    public uint PeriodicNumber => _transceiver.PeriodicNumber;

    public string DisplayName => $"TRX {_transceiver.PeriodicNumber}";

    public string Status { get => _status; private set => SetField(ref _status, value); }

    public string Modulation { get => _modulation; private set => SetField(ref _modulation, value); }

    public long DdsFrequency
    {
        get => _ddsFrequency;
        private set
        {
            if (SetField(ref _ddsFrequency, value)) OnPropertyChanged(nameof(DdsFrequencyDisplay));
        }
    }
    public string DdsFrequencyDisplay => FormatHz(_ddsFrequency);

    public long VfoA
    {
        get => _vfoA;
        private set
        {
            if (SetField(ref _vfoA, value)) OnPropertyChanged(nameof(VfoADisplay));
        }
    }
    public string VfoADisplay => FormatHz(_vfoA);

    public long VfoB
    {
        get => _vfoB;
        private set
        {
            if (SetField(ref _vfoB, value)) OnPropertyChanged(nameof(VfoBDisplay));
        }
    }
    public string VfoBDisplay => FormatHz(_vfoB);

    public uint SampleRate { get => _sampleRate; private set => SetField(ref _sampleRate, value); }

    public long BytesReceived
    {
        get => _bytesReceived;
        private set
        {
            if (SetField(ref _bytesReceived, value)) OnPropertyChanged(nameof(BytesReceivedDisplay));
        }
    }
    public string BytesReceivedDisplay => FormatBytes(_bytesReceived);

    public bool RxAudioRunning { get => _rxAudioRunning; private set => SetField(ref _rxAudioRunning, value); }

    public bool TxAudioRunning { get => _txAudioRunning; private set => SetField(ref _txAudioRunning, value); }

    public bool TxFeatureEnabled => _enableTx;

    public async Task StartRxAudioAsync()
    {
        if (RxAudioRunning) return;
        Status = "Starting RX audio…";
        await _controller.AudioStart(_transceiver.PeriodicNumber);
        RxAudioRunning = true;
        Status = "RX audio streaming";
    }

    public async Task StopRxAudioAsync()
    {
        if (!RxAudioRunning) return;
        Status = "Stopping RX audio…";
        await _controller.AudioStop(_transceiver.PeriodicNumber);
        _playback.Stop();
        RxAudioRunning = false;
        Status = "RX audio stopped";
    }

    public async Task StartTxAudioAsync()
    {
        if (!_enableTx)
        {
            Status = "TX disabled (start the app with --enable-tx to allow keying).";
            return;
        }
        if (TxAudioRunning) return;
        _capture = new OpenAlCapture(48000, 1);
        if (!_capture.Start(out var err))
        {
            Status = err ?? "Failed to start TX capture.";
            _capture.Dispose();
            _capture = null;
            return;
        }
        await _controller.Trx(_transceiver.PeriodicNumber, true, "tci");
        TxAudioRunning = true;
        Status = "TX audio capturing & streaming";
    }

    public async Task StopTxAudioAsync()
    {
        if (!TxAudioRunning) return;
        TxAudioRunning = false;
        try
        {
            await _controller.Trx(_transceiver.PeriodicNumber, false);
        }
        catch { /* best-effort */ }
        _capture?.Stop();
        _capture?.Dispose();
        _capture = null;
        Status = "TX audio stopped";
    }

    private void Controller_OnRxAudioStreamReceived(object? sender, StreamPacketEventArgs e)
    {
        if (e.Packet.ReceiverNumber != _transceiver.PeriodicNumber) return;
        var samples = e.Packet.ToFloatSamples();
        _playback.Submit(samples, e.Packet.SampleRate, (int)e.Packet.Channels);
        Dispatcher.UIThread.Post(() =>
        {
            SampleRate = e.Packet.SampleRate;
            BytesReceived += e.Packet.SampleBytes.Length;
        });
    }

    private void Controller_OnTxChronoReceived(object? sender, StreamPacketEventArgs e)
    {
        if (!_enableTx || !TxAudioRunning || _capture == null) return;
        if (e.Packet.ReceiverNumber != _transceiver.PeriodicNumber) return;

        var requestedSamples = (int)Math.Max(e.Packet.Length, 1);
        var pcm = new short[requestedSamples * _capture.Channels];
        var got = _capture.Read(pcm, pcm.Length);
        var floatSamples = new float[got];
        for (var i = 0; i < got; i++) floatSamples[i] = pcm[i] / 32768f;
        _ = _controller.SendTxAudioPacket(_transceiver.PeriodicNumber, _capture.SampleRate,
                                          (uint)_capture.Channels, floatSamples);
    }

    private static string FormatHz(long hz)
    {
        if (hz == 0) return "—";
        var mhz = hz / 1_000_000.0;
        return $"{mhz:F6} MHz";
    }

    private static string FormatBytes(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024L * 1024) return $"{bytes / 1024.0:F1} KB";
        if (bytes < 1024L * 1024 * 1024) return $"{bytes / (1024.0 * 1024):F1} MB";
        return $"{bytes / (1024.0 * 1024 * 1024):F2} GB";
    }

    public void Dispose()
    {
        _controller.OnRxAudioStreamReceived -= Controller_OnRxAudioStreamReceived;
        _controller.OnTxChronoReceived -= Controller_OnTxChronoReceived;
        _capture?.Dispose();
        _playback.Dispose();
    }
}
