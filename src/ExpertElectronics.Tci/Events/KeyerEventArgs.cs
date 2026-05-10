namespace ExpertElectronics.Tci.Events;

public class KeyerEventArgs(uint transceiverPeriodicNumber, bool pressed, int previousCharacterDurationMs) : EventArgs
{
    public uint TransceiverPeriodicNumber { get; } = transceiverPeriodicNumber;

    public bool Pressed { get; } = pressed;

    public int PreviousCharacterDurationMs { get; } = previousCharacterDurationMs;
}
