namespace ExpertElectronics.Tci.TciCommands;

public class TciCtcssRxToneCommand : ITciCommand, IDisposable
{
    private TciCtcssRxToneCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciCtcssRxToneCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciCtcssRxToneCommand(transceiverController);
    }

    public static string Name => "ctcss_rx_tone";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 4) return false;
        var trx = _transceiverController.GetTransceiver(Convert.ToUInt32(parts[1]));
        if (trx != null) trx.CtcssRxTone = Convert.ToInt32(parts[2]);
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
