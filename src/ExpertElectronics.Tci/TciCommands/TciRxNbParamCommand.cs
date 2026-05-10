namespace ExpertElectronics.Tci.TciCommands;

public class TciRxNbParamCommand : ITciCommand, IDisposable
{
    private TciRxNbParamCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciRxNbParamCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciRxNbParamCommand(transceiverController);
    }

    public static string Name => "rx_nb_param";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 5) return false;
        var trxNum = Convert.ToUInt32(parts[1]);
        var threshold = Convert.ToInt32(parts[2]);
        var pulse = Convert.ToInt32(parts[3]);
        var trx = _transceiverController.GetTransceiver(trxNum);
        if (trx != null)
        {
            trx.RxNbThreshold = threshold;
            trx.RxNbPulseDuration = pulse;
        }
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);

    private readonly ITransceiverController _transceiverController;
}
