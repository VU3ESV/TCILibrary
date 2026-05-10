namespace ExpertElectronics.Tci.TciCommands;

public class TciECoderSwitchChannelCommand : ITciCommand, IDisposable
{
    private TciECoderSwitchChannelCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciECoderSwitchChannelCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciECoderSwitchChannelCommand(transceiverController);
    }

    public static string Name => "ecoder_switch_channel";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 4) return false;
        var args = new ECoderSwitchEventArgs(Convert.ToUInt32(parts[1]), Convert.ToUInt32(parts[2]));
        if (_transceiverController is TransceiverController c) c.RaiseECoderChannelSwitched(args);
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
