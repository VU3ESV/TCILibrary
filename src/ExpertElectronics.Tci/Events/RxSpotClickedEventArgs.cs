namespace ExpertElectronics.Tci.Events;

public class RxSpotClickedEventArgs(uint transceiverPeriodicNumber, uint channel, string callSign, long frequencyInHz) : EventArgs
{
    public uint TransceiverPeriodicNumber { get; } = transceiverPeriodicNumber;

    public uint Channel { get; } = channel;

    public string CallSign { get; } = callSign;

    public long FrequencyInHz { get; } = frequencyInHz;
}
