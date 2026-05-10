namespace ExpertElectronics.Tci.TciCommands.Audio;

public class TciLineOutRecorderSaveCommand : ITciCommand, IDisposable
{
    private TciLineOutRecorderSaveCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciLineOutRecorderSaveCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciLineOutRecorderSaveCommand(transceiverController);
    }

    public static string Name => "line_out_recorder_save";

    public bool ProcessCommandResponses(IEnumerable<string> messages) => true;

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
