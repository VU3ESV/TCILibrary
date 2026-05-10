namespace ExpertElectronics.Tci.TciCommands;

public class TciAppFocusCommand : ITciCommand, IDisposable
{
    private TciAppFocusCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciAppFocusCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciAppFocusCommand(transceiverController);
    }

    public static string Name => "app_focus";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 3) return false;
        _transceiverController.AppFocus = Convert.ToBoolean(parts[1]);
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
