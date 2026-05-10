namespace ExpertElectronics.Tci.TciCommands.Audio;

public class TciAudioStreamSamplesCommand : ITciCommand, IDisposable
{
    private TciAudioStreamSamplesCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciAudioStreamSamplesCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciAudioStreamSamplesCommand(transceiverController);
    }

    public static string Name => "audio_stream_samples";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 3) return false;
        _transceiverController.AudioStreamSamples = Convert.ToUInt32(parts[1]);
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
