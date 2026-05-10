namespace ExpertElectronics.Tci.TciCommands;

public class TciECoderSwitchRxCommand : ITciCommand, IDisposable
{
    private TciECoderSwitchRxCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciECoderSwitchRxCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciECoderSwitchRxCommand(transceiverController);
    }

    public static string Name => "ecoder_switch_rx";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 4) return false;
        var args = new ECoderSwitchEventArgs(Convert.ToUInt32(parts[1]), Convert.ToUInt32(parts[2]));
        if (_transceiverController is TransceiverController c) c.RaiseECoderRxSwitched(args);
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
