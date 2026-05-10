namespace ExpertElectronics.Tci.TciCommands;

public class TciDiglOffsetCommand : ITciCommand, IDisposable
{
    private TciDiglOffsetCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciDiglOffsetCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciDiglOffsetCommand(transceiverController);
    }

    public static string Name => "digl_offset";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 3) return false;
        _transceiverController.DiglOffset = Convert.ToInt32(parts[1]);
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
