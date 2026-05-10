namespace ExpertElectronics.Tci.TciCommands.Audio;

public class TciTxStreamAudioBufferingCommand : ITciCommand, IDisposable
{
    private TciTxStreamAudioBufferingCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciTxStreamAudioBufferingCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciTxStreamAudioBufferingCommand(transceiverController);
    }

    public static string Name => "tx_stream_audio_buffering";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 3) return false;
        _transceiverController.TxStreamAudioBuffering = Convert.ToUInt32(parts[1]);
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
