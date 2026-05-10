namespace ExpertElectronics.Tci.TciCommands.Audio;

public class TciLineOutRecorderBreakCommand : ITciCommand, IDisposable
{
    private TciLineOutRecorderBreakCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciLineOutRecorderBreakCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciLineOutRecorderBreakCommand(transceiverController);
    }

    public static string Name => "line_out_recorder_break";

    public bool ProcessCommandResponses(IEnumerable<string> messages) => true;

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
