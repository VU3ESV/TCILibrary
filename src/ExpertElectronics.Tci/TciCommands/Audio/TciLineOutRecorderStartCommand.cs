namespace ExpertElectronics.Tci.TciCommands.Audio;

public class TciLineOutRecorderStartCommand : ITciCommand, IDisposable
{
    private TciLineOutRecorderStartCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciLineOutRecorderStartCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciLineOutRecorderStartCommand(transceiverController);
    }

    public static string Name => "line_out_recorder_start";

    public bool ProcessCommandResponses(IEnumerable<string> messages) => true;

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
