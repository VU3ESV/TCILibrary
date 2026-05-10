namespace ExpertElectronics.Tci.Events;

public class ECoderSwitchEventArgs(uint ecoderPeriodicNumber, uint targetPeriodicNumber) : EventArgs
{
    public uint ECoderPeriodicNumber { get; } = ecoderPeriodicNumber;

    public uint TargetPeriodicNumber { get; } = targetPeriodicNumber;
}
