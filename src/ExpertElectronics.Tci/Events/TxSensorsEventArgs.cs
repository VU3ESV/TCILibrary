namespace ExpertElectronics.Tci.Events;

public class TxSensorsEventArgs(
    uint transceiverPeriodicNumber,
    double micLevelDbm,
    double signalPowerWatts,
    double peakPowerWatts,
    double swr) : EventArgs
{
    public uint TransceiverPeriodicNumber { get; } = transceiverPeriodicNumber;

    public double MicLevelDbm { get; } = micLevelDbm;

    public double SignalPowerWatts { get; } = signalPowerWatts;

    public double PeakPowerWatts { get; } = peakPowerWatts;

    public double Swr { get; } = swr;
}
