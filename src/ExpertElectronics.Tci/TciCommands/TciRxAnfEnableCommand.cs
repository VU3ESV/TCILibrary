namespace ExpertElectronics.Tci.TciCommands;

public class TciRxAnfEnableCommand : ITciCommand, IDisposable
{
    private TciRxAnfEnableCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciRxAnfEnableCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciRxAnfEnableCommand(transceiverController);
    }

    public static string Name => "rx_anf_enable";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 4) return false;
        var trx = _transceiverController.GetTransceiver(Convert.ToUInt32(parts[1]));
        if (trx != null) trx.RxAnf = Convert.ToBoolean(parts[2]);
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
