namespace ExpertElectronics.Tci;

/// <summary>
/// Represents a channel belonging to a transceiver. A channel carries
/// per-channel state such as VFO, S-meter and IF filter settings.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Channel"/> class.
/// </remarks>
/// <param name="periodicNumber">The periodic number (index) of the channel within the transceiver.</param>
/// <param name="transceiver">The parent <see cref="ITransceiver"/> instance.</param>
public class Channel(uint periodicNumber, ITransceiver transceiver) : IChannel
{
    public uint PeriodicNumber { get; } = periodicNumber;
    public bool Enable
    {
        get => _enable;
        set
        {
            _enable = value;
            OnChannelEnableChanged?.Invoke(this, new ChannelEnableChangeEventArgs(_transceiver.PeriodicNumber, PeriodicNumber, _enable));
        }
    }
    public double IfFilter
    {
        get => _ifFilter;
        set
        {
            _ifFilter = value;
            OnIfFreqChanged?.Invoke(this, new IfFrequencyChangedEventArgs(_transceiver.PeriodicNumber, PeriodicNumber, _ifFilter));
        }
    }
    public int RxSMeter
    {
        get => _rxSMeter;
        set
        {
            _rxSMeter = value;
            OnChannelSMeterChanged?.Invoke(this, new ChannelSMeterChangeEventArgs(_transceiver.PeriodicNumber, PeriodicNumber, _rxSMeter));
        }
    }
    public long Vfo
    {
        get => _vfo;
        set
        {
            _vfo = value;
            OnVfoChange?.Invoke(this, new VfoChangeEventArgs(_transceiver.PeriodicNumber, PeriodicNumber, _vfo));
        }
    }

    public bool ReceiveOnly { get; set; }

    public event EventHandler<VfoChangeEventArgs> OnVfoChange;
    public event EventHandler<IfFrequencyChangedEventArgs> OnIfFreqChanged;
    public event EventHandler<ChannelSMeterChangeEventArgs> OnChannelSMeterChanged;
    public event EventHandler<ChannelEnableChangeEventArgs> OnChannelEnableChanged;

    private readonly ITransceiver _transceiver = transceiver;

    private long _vfo;
    private double _ifFilter;
    private int _rxSMeter;
    private bool _enable;
}