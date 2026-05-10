namespace ExpertElectronics.Tci.TciCommands.Audio;

public class TciLineOutStopCommand : ITciCommand, IDisposable
{
    private TciLineOutStopCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciLineOutStopCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciLineOutStopCommand(transceiverController);
    }

    public static string Name => "line_out_stop";

    public bool ProcessCommandResponses(IEnumerable<string> messages) => true;

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
