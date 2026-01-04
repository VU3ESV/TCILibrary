namespace ExpertElectronics.Tci.Events;

public class VfoLimitsChangedEventArgs : EventArgs
{
    public VfoLimitsChangedEventArgs(long min, long max)
    {
        Min = min;
        Max = max;
    }

    public long Min { get; }

    public long Max { get; }
}