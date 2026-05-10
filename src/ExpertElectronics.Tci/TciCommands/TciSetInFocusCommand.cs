namespace ExpertElectronics.Tci.TciCommands;

public class TciSetInFocusCommand : ITciCommand, IDisposable
{
    private TciSetInFocusCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciSetInFocusCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciSetInFocusCommand(transceiverController);
    }

    public static string Name => "set_in_focus";

    public bool ProcessCommandResponses(IEnumerable<string> messages) => true;

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
