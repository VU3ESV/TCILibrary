namespace ExpertElectronics.Tci.TciCommands;

/// <summary>
/// Spec-conformant alias for <see cref="TciChannelCountCommand"/>. The TCI v1.6 / v2.0
/// PDFs document the command as <c>CHANNEL_COUNT</c> (singular). The reference ExpertSDR3
/// server actually sends the plural form, but we accept either by registering both wire
/// names.
/// </summary>
public class TciChannelCountSpecCommand : ITciCommand, IDisposable
{
    private readonly ITransceiverController _transceiverController;

    private TciChannelCountSpecCommand(ITransceiverController transceiverController)
        => _transceiverController = transceiverController;

    public static TciChannelCountSpecCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciChannelCountSpecCommand(transceiverController);
    }

    public static string Name => "channel_count";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 3) return false;
        var count = Convert.ToUInt32(parts[1]);
        (_transceiverController as TransceiverController)?.CreateChannel(count);
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
}
