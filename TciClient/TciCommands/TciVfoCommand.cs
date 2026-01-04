namespace ExpertElectronics.Tci.TciCommands;

public class TciVfoCommand : ITciCommand, IDisposable
{
    public TciVfoCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciVfoCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciVfoCommand(transceiverController);
    }

    // ToDo: Need to check the name
    public static string Name => "vfo";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {

        var enumerable = messages as string[] ?? [.. messages];
        if (!enumerable.Any(_ => _.Contains(Name)))
        {
            return false;
        }

        var vfoMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
        if (string.IsNullOrEmpty(vfoMessage))
        {
            return false;
        }

        var vfoMessageElements = vfoMessage.Split(':', ',', ';');
        if (vfoMessageElements.Length != CommandParameterCount)
        {
            return false;
        }

        var transceiverPeriodicNumber = Convert.ToUInt32(vfoMessageElements[TransceiverIndex]);
        var channelNumber = Convert.ToUInt32(vfoMessageElements[ChannelIndex]);
        var vfo = Convert.ToInt64(vfoMessageElements[VfoIndex]);
        var transceiver = _transceiverController.GetTransceiver(transceiverPeriodicNumber);
        var channel = transceiver?.Channels?.FirstOrDefault(_ => _.PeriodicNumber == channelNumber);
        if (channel != null)
        {
            channel.Vfo = vfo;
        }
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
    private const int TransceiverIndex = 1;
    private const int ChannelIndex = 2;
    private const int VfoIndex = 3;
    private const int CommandParameterCount = 5;
}
