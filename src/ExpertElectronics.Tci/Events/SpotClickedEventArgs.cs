namespace ExpertElectronics.Tci.Events;

public class SpotClickedEventArgs(string callSign, long frequencyInHz) : EventArgs
{
    public string CallSign { get; } = callSign;

    public long FrequencyInHz { get; } = frequencyInHz;
}
