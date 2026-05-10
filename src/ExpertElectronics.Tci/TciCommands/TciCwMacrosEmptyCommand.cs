namespace ExpertElectronics.Tci.TciCommands;

public class TciCwMacrosEmptyCommand : ITciCommand, IDisposable
{
    private TciCwMacrosEmptyCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciCwMacrosEmptyCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciCwMacrosEmptyCommand(transceiverController);
    }

    public static string Name => "cw_macros_empty";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        if (messages.Any(m => m.StartsWith(Name, StringComparison.OrdinalIgnoreCase)))
        {
            if (_transceiverController is TransceiverController c) c.RaiseCwMacrosEmpty();
            return true;
        }
        return false;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
