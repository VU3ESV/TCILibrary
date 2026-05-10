namespace ExpertElectronics.Tci.Events;

public class ChannelStateChangedEventArgs(uint transceiverPeriodicNumber, uint channel, bool state) : EventArgs
{
    public uint TransceiverPeriodicNumber { get; } = transceiverPeriodicNumber;

    public uint Channel { get; } = channel;

    public bool State { get; } = state;
}
