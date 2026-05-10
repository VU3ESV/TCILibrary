namespace ExpertElectronics.Tci.TciCommands;

public class TciClickedOnSpotCommand : ITciCommand, IDisposable
{
    private TciClickedOnSpotCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciClickedOnSpotCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciClickedOnSpotCommand(transceiverController);
    }

    public static string Name => "clicked_on_spot";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 4) return false;
        var args = new SpotClickedEventArgs(parts[1], Convert.ToInt64(parts[2]));
        if (_transceiverController is TransceiverController c) c.RaiseSpotClicked(args);
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
