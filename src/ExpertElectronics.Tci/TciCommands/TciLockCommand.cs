namespace ExpertElectronics.Tci.TciCommands;

public class TciLockCommand : ITciCommand, IDisposable
{
    private TciLockCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciLockCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciLockCommand(transceiverController);
    }

    public static string Name => "lock";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 4) return false;
        var trxNum = Convert.ToUInt32(parts[1]);
        var state = Convert.ToBoolean(parts[2]);
        var trx = _transceiverController.GetTransceiver(trxNum);
        if (trx != null) trx.Lock = state;
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);

    private readonly ITransceiverController _transceiverController;
}
