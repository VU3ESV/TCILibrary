namespace ExpertElectronics.Tci.TciCommands;

public class TciSplitEnableCommand : ITciCommand, IDisposable
{
    private TciSplitEnableCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciSplitEnableCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciSplitEnableCommand(transceiverController);
    }

    public static string Name => "split_enable";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var enumerable = messages as string[] ?? [.. messages];
        if (!enumerable.Any(_ => _.Contains(Name)))
        {
            return false;
        }

        var splitEnableMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
        if (string.IsNullOrEmpty(splitEnableMessage))
        {
            return false;
        }

        var splitEnableMessageElements = splitEnableMessage.Split(':', ',', ';');
        if (splitEnableMessageElements.Length != CommandParameterCount)
        {
            return false;
        }

        var transceiverPeriodicNumber = Convert.ToUInt32(splitEnableMessageElements[TransceiverIndex]);
        var splitEnable = Convert.ToBoolean(splitEnableMessageElements[SplitEnableIndex]);
        var transceiver = _transceiverController.GetTransceiver(transceiverPeriodicNumber);
        if (transceiver != null)
        {
            transceiver.Split = splitEnable;
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
    private const int SplitEnableIndex = 2;
    private const int CommandParameterCount = 4;
}
