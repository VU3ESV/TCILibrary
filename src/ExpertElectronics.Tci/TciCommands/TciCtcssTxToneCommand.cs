namespace ExpertElectronics.Tci.TciCommands;

public class TciCtcssTxToneCommand : ITciCommand, IDisposable
{
    private TciCtcssTxToneCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciCtcssTxToneCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciCtcssTxToneCommand(transceiverController);
    }

    public static string Name => "ctcss_tx_tone";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 4) return false;
        var trx = _transceiverController.GetTransceiver(Convert.ToUInt32(parts[1]));
        if (trx != null) trx.CtcssTxTone = Convert.ToInt32(parts[2]);
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
