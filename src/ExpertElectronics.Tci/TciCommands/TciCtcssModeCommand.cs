namespace ExpertElectronics.Tci.TciCommands;

public class TciCtcssModeCommand : ITciCommand, IDisposable
{
    private TciCtcssModeCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciCtcssModeCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciCtcssModeCommand(transceiverController);
    }

    public static string Name => "ctcss_mode";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 4) return false;
        var trx = _transceiverController.GetTransceiver(Convert.ToUInt32(parts[1]));
        if (trx != null) trx.CtcssMode = Convert.ToInt32(parts[2]);
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
