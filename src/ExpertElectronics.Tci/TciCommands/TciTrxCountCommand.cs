namespace ExpertElectronics.Tci.TciCommands;

public class TciTrxCountCommand : ITciCommand, IDisposable
{
    private TciTrxCountCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciTrxCountCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciTrxCountCommand(transceiverController);
    }

    public static string Name => "trx_count";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var enumerable = messages as string[] ?? [.. messages];
        if (!enumerable.Any(_ => _.Contains(Name)))
        {
            return false;
        }

        var trxCountMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
        if (string.IsNullOrEmpty(trxCountMessage))
        {
            return false;
        }

        var trxCountMessageElements = trxCountMessage.Split(':', ',', ';');
        if (trxCountMessageElements.Length != CommandParameterCount)
        {
            return false;
        }

        var transceiverCount = Convert.ToUInt32(trxCountMessageElements[TransceiverIndex]);
        _transceiverController.CreateTransceivers(transceiverCount);
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
    private const int CommandParameterCount = 3;
}
