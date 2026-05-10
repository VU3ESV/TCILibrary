namespace ExpertElectronics.Tci.Events;

public class LongValueChangedEventArgs(long value) : EventArgs
{
    public long Value { get; } = value;
}
