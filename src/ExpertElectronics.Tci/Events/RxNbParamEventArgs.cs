namespace ExpertElectronics.Tci.Events;

public class RxNbParamEventArgs(uint transceiverPeriodicNumber, int threshold, int pulseDurationUs) : EventArgs
{
    public uint TransceiverPeriodicNumber { get; } = transceiverPeriodicNumber;

    public int Threshold { get; } = threshold;

    public int PulseDurationUs { get; } = pulseDurationUs;
}
