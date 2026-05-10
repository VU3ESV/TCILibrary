namespace ExpertElectronics.Tci.TciCommands;

public class TciStartCommand : ITciCommand, IDisposable
{
    public static TciStartCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciStartCommand(transceiverController);
    }

    private TciStartCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static string Name => "start";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        if (!messages.Any(_ => _.Contains(Name)))
        {
            return false;
        }

        _transceiverController.Start = true;
        _transceiverController.Stop = false;
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
