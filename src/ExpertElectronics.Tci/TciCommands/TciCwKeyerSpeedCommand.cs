namespace ExpertElectronics.Tci.TciCommands;

public class TciCwKeyerSpeedCommand : ITciCommand, IDisposable
{
    private TciCwKeyerSpeedCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciCwKeyerSpeedCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciCwKeyerSpeedCommand(transceiverController);
    }

    public static string Name => "cw_keyer_speed";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 3) return false;
        _transceiverController.CwKeyerSpeed = Convert.ToUInt32(parts[1]);
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
