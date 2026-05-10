namespace ExpertElectronics.Tci.TciCommands;

public class TciMonEnableCommand : ITciCommand, IDisposable
{
    private TciMonEnableCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciMonEnableCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciMonEnableCommand(transceiverController);
    }

    public static string Name => "mon_enable";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 3) return false;
        _transceiverController.MonEnable = Convert.ToBoolean(parts[1]);
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
