namespace ExpertElectronics.Tci.TciCommands;

public class TciReceiveOnlyCommand : ITciCommand, IDisposable
{
    private TciReceiveOnlyCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciReceiveOnlyCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciReceiveOnlyCommand(transceiverController);
    }

    public static string Name => "receive_only";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var enumerable = messages as string[] ?? [.. messages];
        if (!enumerable.Any(_ => _.Contains(Name)))
        {
            return false;
        }

        var receiveOnlyMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
        if (string.IsNullOrEmpty(receiveOnlyMessage))
        {
            return false;
        }

        var receiveOnlyMessageElements = receiveOnlyMessage.Split(':', ',', ';');
        if (receiveOnlyMessageElements.Length != CommandParameterCount)
        {
            return false;
        }

        var receiveOnly = Convert.ToBoolean(receiveOnlyMessageElements[ReceiveOnlyIndex]);
        _transceiverController.ReceiveOnly = receiveOnly;
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
    private const int ReceiveOnlyIndex = 1;
    private const int CommandParameterCount = 3;
}
