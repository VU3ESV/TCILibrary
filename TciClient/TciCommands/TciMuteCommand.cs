namespace ExpertElectronics.Tci.TciCommands;

public class TciMuteCommand : ITciCommand, IDisposable
{
    private TciMuteCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciMuteCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciMuteCommand(transceiverController);
    }

    public static string Name => "mute";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var enumerable = messages as string[] ?? [.. messages];
        if (!enumerable.Any(_ => _.Contains(Name)))
        {
            return false;
        }

        var muteMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
        if (string.IsNullOrEmpty(muteMessage))
        {
            return false;
        }

        var muteMessageElements = muteMessage.Split(':', ',', ';');
        if (muteMessageElements.Length != CommandParameterCount)
        {
            return false;
        }

        var mute = Convert.ToBoolean(muteMessageElements[MuteIndex]);
        _transceiverController.Mute = mute;
        return true;
    }

    public void Dispose()
    {
        if (_transceiverController == null)
        {
            return;
        }

        GC.SuppressFinalize(this);
    }

    private readonly ITransceiverController _transceiverController;
    private const int MuteIndex = 1;
    private const int CommandParameterCount = 3;
}
