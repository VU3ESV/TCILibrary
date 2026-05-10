namespace ExpertElectronics.Tci.TciCommands;

public class TciRxApfEnableCommand : ITciCommand, IDisposable
{
    private TciRxApfEnableCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciRxApfEnableCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciRxApfEnableCommand(transceiverController);
    }

    public static string Name => "rx_apf_enable";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 4) return false;
        var trx = _transceiverController.GetTransceiver(Convert.ToUInt32(parts[1]));
        if (trx != null) trx.RxApf = Convert.ToBoolean(parts[2]);
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
