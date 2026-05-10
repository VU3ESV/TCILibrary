namespace ExpertElectronics.Tci.TciCommands;

public class TciRxClickedOnSpotCommand : ITciCommand, IDisposable
{
    private TciRxClickedOnSpotCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciRxClickedOnSpotCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciRxClickedOnSpotCommand(transceiverController);
    }

    public static string Name => "rx_clicked_on_spot";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 6) return false;
        var args = new RxSpotClickedEventArgs(
            Convert.ToUInt32(parts[1]),
            Convert.ToUInt32(parts[2]),
            parts[3],
            Convert.ToInt64(parts[4]));
        if (_transceiverController is TransceiverController c) c.RaiseRxSpotClicked(args);
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
