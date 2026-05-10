namespace ExpertElectronics.Tci.Events;

public class ChannelIntValueChangedEventArgs(uint transceiverPeriodicNumber, uint channel, int value) : EventArgs
{
    public uint TransceiverPeriodicNumber { get; } = transceiverPeriodicNumber;

    public uint Channel { get; } = channel;

    public int Value { get; } = value;
}
