namespace ExpertElectronics.Tci.TciCommands.Audio;

public class TciAudioStreamSampleTypeCommand : ITciCommand, IDisposable
{
    private TciAudioStreamSampleTypeCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciAudioStreamSampleTypeCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciAudioStreamSampleTypeCommand(transceiverController);
    }

    public static string Name => "audio_stream_sample_type";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 3) return false;
        _transceiverController.AudioStreamSampleType = parts[1];
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
