namespace ExpertElectronics.Tci;

/// <summary>
/// Represents a transceiver (radio) and its state. Holds channels and TX/RX state.
/// </summary>
public class Transceiver : ITransceiver
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Transceiver"/> class.
    /// </summary>
    /// <param name="periodicNumber">The periodic number (index) of this transceiver.</param>
    public Transceiver(uint periodicNumber)
    {
        _channels = new();
        PeriodicNumber = periodicNumber;
    }

    public uint PeriodicNumber { get; }

    public bool TxEnable
    {
        get => _txEnable;
        set
        {
            _txEnable = value;
            OnTxEnableChanged?.Invoke(this, new TrxEventArgs(PeriodicNumber, _txEnable));
        }
    }

    public bool TxFootSwitch
    {
        get => _txFootSwitch;
        set
        {
            _txFootSwitch = value;
            OnTxFootSwitch?.Invoke(this, new TrxEventArgs(PeriodicNumber, _txFootSwitch));
        }
    }

    public double DdsFrequency
    {
        get => _ddsFrequency;
        set
        {
            _ddsFrequency = value;
            OnDdsFreqChanged?.Invoke(this, new TrxDoubleValueChangedEventArgs(PeriodicNumber, _ddsFrequency));
        }
    }

    public bool Rit
    {
        get => _rit;
        set
        {
            _rit = value;
            OnRitEnableChanged?.Invoke(this, new TrxEventArgs(PeriodicNumber, _rit));
        }
    }

    public int RitOffset
    {
        get => _ritOffset;
        set
        {
            _ritOffset = value;
            OnRitOffsetChanged?.Invoke(this, new TrxIntValueChangedEventArgs(PeriodicNumber, _ritOffset));
        }
    }

    public string Modulation
    {
        get => _modulation;
        set
        {
            _modulation = value;
            OnModulationChanged?.Invoke(this, new TrxStringValueChangedEventArgs(PeriodicNumber, _modulation));
        }
    }

    public bool RxEnable
    {
        get => _rxEnable;
        set
        {
            _rxEnable = value;
            OnRxEnableChanged?.Invoke(this, new TrxEventArgs(PeriodicNumber, _rxEnable));
        }
    }

    public bool Xit
    {
        get => _xit;
        set
        {
            _xit = value;
            OnXitEnableChanged?.Invoke(this, new TrxEventArgs(PeriodicNumber, _xit));
        }
    }

    public int XitOffset
    {
        get => _xitOffset;
        set
        {
            _xitOffset = value;
            OnXitOffsetChanged?.Invoke(this, new TrxIntValueChangedEventArgs(PeriodicNumber, _xitOffset));
        }
    }

    public bool Split
    {
        get => _split;
        set
        {
            _split = value;
            OnSplitEnableChanged?.Invoke(this, new TrxEventArgs(PeriodicNumber, _split));
        }
    }

    public int RxFilterLowLimit
    {
        get => _rxFilterLowLimit;

        set
        {
            _rxFilterLowLimit = value;
            OnRxFilterChanged?.Invoke(this, new RxFilterChangedEventArgs(PeriodicNumber, _rxFilterLowLimit, _rxFilterHighLimit));
        }
    }

    public int RxFilterHighLimit
    {
        get => _rxFilterHighLimit;

        set
        {
            _rxFilterHighLimit = value;
            OnRxFilterChanged?.Invoke(this, new RxFilterChangedEventArgs(PeriodicNumber, _rxFilterLowLimit, _rxFilterHighLimit));
        }
    }

    public bool Trx
    {
        get => _trxEnable;

        set
        {
            _trxEnable = value;
            OnTrx?.Invoke(this, new TrxEventArgs(PeriodicNumber, _trxEnable));
        }
    }

    public bool Tune
    {
        get => _tune;

        set
        {
            _tune = value;
            OnTune?.Invoke(this, new TrxEventArgs(PeriodicNumber, _tune));
        }
    }

    public bool IqEnable
    {
        get => _iqEnable;

        set
        {
            _iqEnable = value;
            OnIqEnableChanged?.Invoke(this, new TrxEventArgs(PeriodicNumber, _iqEnable));
        }
    }

    public bool AudioEnable
    {
        get => _audioEnable;

        set
        {
            _audioEnable = value;
            OnAudioEnableChanged?.Invoke(this, new TrxEventArgs(PeriodicNumber, _audioEnable));
        }
    }

    public bool Squelch
    {
        get => _squelch;

        set
        {
            _squelch = value;
            OnSquelchChanged?.Invoke(this, new TrxEventArgs(PeriodicNumber, _squelch));
        }
    }

    public int SquelchThreshold
    {
        get => _squelchThreshold;

        set
        {
            _squelchThreshold = value;
            OnSquelchThresholdChanged?.Invoke(this, new TrxIntValueChangedEventArgs(PeriodicNumber, _squelchThreshold));
        }
    }

    public bool RxMute
    {
        get => _rxMute;

        set
        {
            _rxMute = value;
            OnRxMute?.Invoke(this, new TrxEventArgs(PeriodicNumber, _rxMute));
        }
    }

    public IEnumerable<Channel> Channels => _channels;

    public void AddChannel(uint channelNumbers)
    {
        for (uint i = 0; i < channelNumbers; i++)
        {
            _channels.Add(new Channel(i, this));
        }
    }

    public event EventHandler<TrxEventArgs> OnTxEnableChanged;
    public event EventHandler<TrxEventArgs> OnTxFootSwitch;
    public event EventHandler<TrxDoubleValueChangedEventArgs> OnDdsFreqChanged;
    public event EventHandler<TrxEventArgs> OnRitEnableChanged;
    public event EventHandler<TrxIntValueChangedEventArgs> OnRitOffsetChanged;
    public event EventHandler<TrxStringValueChangedEventArgs> OnModulationChanged;
    public event EventHandler<TrxEventArgs> OnRxEnableChanged;
    public event EventHandler<TrxEventArgs> OnXitEnableChanged;
    public event EventHandler<TrxIntValueChangedEventArgs> OnXitOffsetChanged;
    public event EventHandler<TrxEventArgs> OnSplitEnableChanged;
    public event EventHandler<RxFilterChangedEventArgs> OnRxFilterChanged;
    public event EventHandler<TrxEventArgs> OnTrx;
    public event EventHandler<TrxEventArgs> OnTune;
    public event EventHandler<TrxEventArgs> OnIqEnableChanged;
    public event EventHandler<TrxEventArgs> OnAudioEnableChanged;
    public event EventHandler<TrxEventArgs> OnSquelchChanged;
    public event EventHandler<TrxIntValueChangedEventArgs> OnSquelchThresholdChanged;
    public event EventHandler<TrxEventArgs> OnRxMute;

    private readonly List<Channel> _channels;
    private bool _txEnable;
    private bool _txFootSwitch;
    private double _ddsFrequency;
    private bool _rit;
    private int _ritOffset;
    private string _modulation;
    private bool _rxEnable;
    private bool _xit;
    private bool _split;
    private int _xitOffset;
    private int _rxFilterLowLimit;
    private int _rxFilterHighLimit;
    private bool _trxEnable;
    private bool _tune;
    private bool _iqEnable;
    private bool _audioEnable;
    private bool _squelch;
    private int _squelchThreshold;
    private bool _rxMute;
}
