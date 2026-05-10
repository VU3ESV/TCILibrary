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

    public bool Lock
    {
        get => _lock;
        set
        {
            _lock = value;
            OnLockChanged?.Invoke(this, new TrxEventArgs(PeriodicNumber, _lock));
        }
    }

    public string AgcMode
    {
        get => _agcMode;
        set
        {
            _agcMode = value;
            OnAgcModeChanged?.Invoke(this, new TrxStringValueChangedEventArgs(PeriodicNumber, _agcMode));
        }
    }

    public int AgcGain
    {
        get => _agcGain;
        set
        {
            _agcGain = value;
            OnAgcGainChanged?.Invoke(this, new TrxIntValueChangedEventArgs(PeriodicNumber, _agcGain));
        }
    }

    public bool RxNb
    {
        get => _rxNb;
        set
        {
            _rxNb = value;
            OnRxNbEnableChanged?.Invoke(this, new TrxEventArgs(PeriodicNumber, _rxNb));
        }
    }

    public int RxNbThreshold
    {
        get => _rxNbThreshold;
        set
        {
            _rxNbThreshold = value;
            OnRxNbParamChanged?.Invoke(this, new RxNbParamEventArgs(PeriodicNumber, _rxNbThreshold, _rxNbPulseDuration));
        }
    }

    public int RxNbPulseDuration
    {
        get => _rxNbPulseDuration;
        set
        {
            _rxNbPulseDuration = value;
            OnRxNbParamChanged?.Invoke(this, new RxNbParamEventArgs(PeriodicNumber, _rxNbThreshold, _rxNbPulseDuration));
        }
    }

    public bool RxBin
    {
        get => _rxBin;
        set
        {
            _rxBin = value;
            OnRxBinEnableChanged?.Invoke(this, new TrxEventArgs(PeriodicNumber, _rxBin));
        }
    }

    public bool RxNr
    {
        get => _rxNr;
        set
        {
            _rxNr = value;
            OnRxNrEnableChanged?.Invoke(this, new TrxEventArgs(PeriodicNumber, _rxNr));
        }
    }

    public bool RxAnc
    {
        get => _rxAnc;
        set
        {
            _rxAnc = value;
            OnRxAncEnableChanged?.Invoke(this, new TrxEventArgs(PeriodicNumber, _rxAnc));
        }
    }

    public bool RxAnf
    {
        get => _rxAnf;
        set
        {
            _rxAnf = value;
            OnRxAnfEnableChanged?.Invoke(this, new TrxEventArgs(PeriodicNumber, _rxAnf));
        }
    }

    public bool RxApf
    {
        get => _rxApf;
        set
        {
            _rxApf = value;
            OnRxApfEnableChanged?.Invoke(this, new TrxEventArgs(PeriodicNumber, _rxApf));
        }
    }

    public bool RxDse
    {
        get => _rxDse;
        set
        {
            _rxDse = value;
            OnRxDseEnableChanged?.Invoke(this, new TrxEventArgs(PeriodicNumber, _rxDse));
        }
    }

    public bool RxNf
    {
        get => _rxNf;
        set
        {
            _rxNf = value;
            OnRxNfEnableChanged?.Invoke(this, new TrxEventArgs(PeriodicNumber, _rxNf));
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
    public event EventHandler<TrxEventArgs> OnLockChanged;
    public event EventHandler<TrxStringValueChangedEventArgs> OnAgcModeChanged;
    public event EventHandler<TrxIntValueChangedEventArgs> OnAgcGainChanged;
    public event EventHandler<TrxEventArgs> OnRxNbEnableChanged;
    public event EventHandler<RxNbParamEventArgs> OnRxNbParamChanged;
    public event EventHandler<TrxEventArgs> OnRxBinEnableChanged;
    public event EventHandler<TrxEventArgs> OnRxNrEnableChanged;
    public event EventHandler<TrxEventArgs> OnRxAncEnableChanged;
    public event EventHandler<TrxEventArgs> OnRxAnfEnableChanged;
    public event EventHandler<TrxEventArgs> OnRxApfEnableChanged;
    public event EventHandler<TrxEventArgs> OnRxDseEnableChanged;
    public event EventHandler<TrxEventArgs> OnRxNfEnableChanged;

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
    private bool _lock;
    private string _agcMode;
    private int _agcGain;
    private bool _rxNb;
    private int _rxNbThreshold;
    private int _rxNbPulseDuration;
    private bool _rxBin;
    private bool _rxNr;
    private bool _rxAnc;
    private bool _rxAnf;
    private bool _rxApf;
    private bool _rxDse;
    private bool _rxNf;
}
