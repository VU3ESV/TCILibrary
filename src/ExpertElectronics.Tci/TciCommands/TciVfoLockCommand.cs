namespace ExpertElectronics.Tci.TciCommands;

public class TciVfoLockCommand : ITciCommand, IDisposable
{
    private TciVfoLockCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciVfoLockCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciVfoLockCommand(transceiverController);
    }

    public static string Name => "vfo_lock";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 5) return false;
        var trx = _transceiverController.GetTransceiver(Convert.ToUInt32(parts[1]));
        var ch = trx?.Channels.FirstOrDefault(c => c.PeriodicNumber == Convert.ToUInt32(parts[2]));
        if (ch != null) ch.VfoLock = Convert.ToBoolean(parts[3]);
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
