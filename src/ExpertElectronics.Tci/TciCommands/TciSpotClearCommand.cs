namespace ExpertElectronics.Tci.TciCommands;

public class TciSpotClearCommand : ITciCommand, IDisposable
{
    private TciSpotClearCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciSpotClearCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciSpotClearCommand(transceiverController);
    }

    public static string Name => "spot_clear";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        //WriteOnlyCommand
        return true;
    }

    public void Dispose()
    {
        if (_transceiverController == null)
        {
            return;
        }

        GC.SuppressFinalize(this);
    }

    private readonly ITransceiverController _transceiverController;
}
