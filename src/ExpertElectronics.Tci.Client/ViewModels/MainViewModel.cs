using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using ExpertElectronics.Tci.Client.Audio;
using ExpertElectronics.Tci.Events;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.Client.ViewModels;

public sealed class MainViewModel : ViewModelBase, IDisposable
{
    private readonly OpenAlEngine _audioEngine = new();
    private readonly bool _enableTx;
    private TciClient? _tciClient;
    private CancellationTokenSource? _cts;
    private string _host = "localhost";
    private uint _port = 40001;
    private string _connectionStatus = "Disconnected";
    private string _device = string.Empty;
    private string _protocol = string.Empty;
    private bool _isConnected;
    private bool _isReady;
    private string _audioStatus;

    public MainViewModel()
    {
        _enableTx = AppOptions.EnableTx;
        if (_audioEngine.Initialize(out var err))
        {
            _audioStatus = "OpenAL ready";
        }
        else
        {
            _audioStatus = err ?? "OpenAL unavailable";
        }
    }

    public string Host { get => _host; set => SetField(ref _host, value); }

    public uint Port { get => _port; set => SetField(ref _port, value); }

    /// <summary>
    /// Two-way string view over <see cref="Port"/> so the connection panel can use a plain
    /// TextBox (NumericUpDown spinner buttons truncate 5-digit ports).
    /// </summary>
    public string PortText
    {
        get => _port.ToString(System.Globalization.CultureInfo.InvariantCulture);
        set
        {
            if (uint.TryParse(value, System.Globalization.NumberStyles.Integer,
                              System.Globalization.CultureInfo.InvariantCulture, out var parsed)
                && parsed is >= 1 and <= 65535)
            {
                Port = parsed;
                OnPropertyChanged();
            }
            else
            {
                // Keep the previous valid value; emit change so the TextBox re-renders.
                OnPropertyChanged();
            }
        }
    }

    public string ConnectionStatus { get => _connectionStatus; private set => SetField(ref _connectionStatus, value); }
    public string Device { get => _device; private set => SetField(ref _device, value); }
    public string Protocol { get => _protocol; private set => SetField(ref _protocol, value); }
    public bool IsConnected { get => _isConnected; private set => SetField(ref _isConnected, value); }
    public bool IsReady { get => _isReady; private set => SetField(ref _isReady, value); }
    public string AudioStatus { get => _audioStatus; private set => SetField(ref _audioStatus, value); }
    public bool TxEnabled => _enableTx;

    public ObservableCollection<TransceiverViewModel> Transceivers { get; } = new();

    public string Title => _enableTx
        ? "TCI Client (TX enabled)"
        : "TCI Client (RX-only — start with --enable-tx for TX)";

    public async Task ConnectAsync()
    {
        if (IsConnected) return;
        _cts = new CancellationTokenSource();
        ConnectionStatus = $"Connecting to ws://{Host}:{Port}…";
        try
        {
            _tciClient = await TciClient.CreateAsync(Host, Port, _cts.Token);
            HookEvents(_tciClient);
            await _tciClient.ConnectAsync();
            IsConnected = true;
            ConnectionStatus = $"Connected to ws://{Host}:{Port}";
            await _tciClient.TransceiverController.StartTransceiver();
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"Connect failed: {ex.Message}";
            IsConnected = false;
        }
    }

    public async Task DisconnectAsync()
    {
        if (_tciClient is null) return;
        ConnectionStatus = "Disconnecting…";
        try
        {
            foreach (var trx in Transceivers)
            {
                await trx.StopRxAudioAsync();
                await trx.StopTxAudioAsync();
                trx.Dispose();
            }
            Transceivers.Clear();
            await _tciClient.TransceiverController.StopTransceiver();
            await _tciClient.DisConnectAsync();
        }
        catch { /* best-effort */ }
        finally
        {
            _tciClient?.Dispose();
            _tciClient = null;
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            IsConnected = false;
            IsReady = false;
            ConnectionStatus = "Disconnected";
        }
    }

    private void HookEvents(TciClient client)
    {
        var c = client.TransceiverController;
        c.OnStarted += (_, _) => Dispatcher.UIThread.Post(BuildTransceiverList);
    }

    private void BuildTransceiverList()
    {
        if (_tciClient is null) return;
        var c = _tciClient.TransceiverController;
        IsReady = c.Ready;
        Device = c.Device ?? string.Empty;

        // Avoid duplicating if event fires multiple times.
        if (Transceivers.Count > 0) return;
        foreach (var trx in c.Transceivers)
        {
            var vm = new TransceiverViewModel(c, trx, _audioEngine.Al, _enableTx);
            Transceivers.Add(vm);
        }
        ConnectionStatus = Transceivers.Count > 0
            ? $"Ready — {Transceivers.Count} transceiver(s) discovered"
            : "Ready — no transceivers reported";
    }

    public void Dispose()
    {
        _ = DisconnectAsync();
        _audioEngine.Dispose();
    }
}
