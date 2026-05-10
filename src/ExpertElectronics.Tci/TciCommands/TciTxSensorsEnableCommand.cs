namespace ExpertElectronics.Tci.TciCommands;

public class TciTxSensorsEnableCommand : ITciCommand, IDisposable
{
    private TciTxSensorsEnableCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciTxSensorsEnableCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciTxSensorsEnableCommand(transceiverController);
    }

    public static string Name => "tx_sensors_enable";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length < 3) return false;
        _transceiverController.TxSensorsEnable = Convert.ToBoolean(parts[1]);
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
