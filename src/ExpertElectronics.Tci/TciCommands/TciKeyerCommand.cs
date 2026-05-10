namespace ExpertElectronics.Tci.TciCommands;

public class TciKeyerCommand : ITciCommand, IDisposable
{
    private TciKeyerCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciKeyerCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciKeyerCommand(transceiverController);
    }

    public static string Name => "keyer";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length < 4) return false;
        var trxNum = Convert.ToUInt32(parts[1]);
        var pressed = Convert.ToBoolean(parts[2]);
        var duration = parts.Length >= 5 ? Convert.ToInt32(parts[3]) : 0;
        if (_transceiverController is TransceiverController c)
            c.RaiseKeyer(new KeyerEventArgs(trxNum, pressed, duration));
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
