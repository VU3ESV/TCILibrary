namespace ExpertElectronics.Tci.TciCommands;

public class TciTxPowerCommand : ITciCommand, IDisposable
{
    public static TciTxPowerCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciTxPowerCommand(transceiverController);
    }

    private TciTxPowerCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;


    public static string Name => "tx_power";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var enumerable = messages as string[] ?? [.. messages];
        if (!enumerable.Any(_ => _.Contains(Name)))
        {
            return false;
        }

        var txPowerMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
        if (string.IsNullOrEmpty(txPowerMessage))
        {
            return false;
        }

        var txPowerMessageElements = txPowerMessage.Split(':', ',', ';');
        if (txPowerMessageElements.Length != CommandParameterCount)
        {
            return false;
        }

        _transceiverController.TxPower = (float)Convert.ToDouble(txPowerMessageElements[TxPowerIndex]);
        return true;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private readonly ITransceiverController _transceiverController;
    private const uint CommandParameterCount = 2;
    private const uint TxPowerIndex = 1;
}
