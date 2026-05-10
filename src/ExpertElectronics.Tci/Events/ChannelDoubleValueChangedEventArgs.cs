namespace ExpertElectronics.Tci.Events;

public class ChannelDoubleValueChangedEventArgs(uint transceiverPeriodicNumber, uint channel, double value) : EventArgs
{
    public uint TransceiverPeriodicNumber { get; } = transceiverPeriodicNumber;

    public uint Channel { get; } = channel;

    public double Value { get; } = value;
}
