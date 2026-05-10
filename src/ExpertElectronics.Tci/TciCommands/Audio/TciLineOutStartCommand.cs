namespace ExpertElectronics.Tci.TciCommands.Audio;

public class TciLineOutStartCommand : ITciCommand, IDisposable
{
    private TciLineOutStartCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciLineOutStartCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciLineOutStartCommand(transceiverController);
    }

    public static string Name => "line_out_start";

    public bool ProcessCommandResponses(IEnumerable<string> messages) => true;

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
