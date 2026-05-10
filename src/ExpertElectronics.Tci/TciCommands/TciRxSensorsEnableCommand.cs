namespace ExpertElectronics.Tci.TciCommands;

public class TciRxSensorsEnableCommand : ITciCommand, IDisposable
{
    private TciRxSensorsEnableCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciRxSensorsEnableCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciRxSensorsEnableCommand(transceiverController);
    }

    public static string Name => "rx_sensors_enable";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length < 3) return false;
        _transceiverController.RxSensorsEnable = Convert.ToBoolean(parts[1]);
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
