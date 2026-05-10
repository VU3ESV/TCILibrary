namespace ExpertElectronics.Tci.TciCommands;

public class TciTxFrequencyCommand : ITciCommand, IDisposable
{
    private TciTxFrequencyCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciTxFrequencyCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciTxFrequencyCommand(transceiverController);
    }

    public static string Name => "tx_frequency";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 3) return false;
        _transceiverController.TxFrequency = Convert.ToInt64(parts[1]);
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
