namespace ExpertElectronics.Tci.Events;

public class VfoLimitsChangedEventArgs(long min, long max) : EventArgs
{
    public long Min { get; } = min;

    public long Max { get; } = max;
}