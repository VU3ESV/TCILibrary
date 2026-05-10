namespace ExpertElectronics.Tci.TciCommands;

public class TciRxDseEnableCommand : ITciCommand, IDisposable
{
    private TciRxDseEnableCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciRxDseEnableCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciRxDseEnableCommand(transceiverController);
    }

    public static string Name => "rx_dse_enable";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 4) return false;
        var trx = _transceiverController.GetTransceiver(Convert.ToUInt32(parts[1]));
        if (trx != null) trx.RxDse = Convert.ToBoolean(parts[2]);
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
