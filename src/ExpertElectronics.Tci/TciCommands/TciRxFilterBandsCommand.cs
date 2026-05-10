namespace ExpertElectronics.Tci.TciCommands;

public class TciRxFilterBandsCommand : ITciCommand, IDisposable
{
    private TciRxFilterBandsCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciRxFilterBandsCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciRxFilterBandsCommand(transceiverController);
    }

    public static string Name => "rx_filter_band";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var enumerable = messages as string[] ?? [.. messages];
        if (!enumerable.Any(_ => _.Contains(Name)))
        {
            return false;
        }

        var rxFilterMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
        if (string.IsNullOrEmpty(rxFilterMessage))
        {
            return false;
        }

        var rxFilterMessageElements = rxFilterMessage.Split(':', ',', ';');
        if (rxFilterMessageElements.Length != CommandParameterCount)
        {
            return false;
        }

        var receiverPeriodicNumber = Convert.ToUInt32(rxFilterMessageElements[ReceiverIndex]);
        var maxLimit = Convert.ToInt32(rxFilterMessageElements[MaxIndex]);
        var minLimit = Convert.ToInt32(rxFilterMessageElements[MinIndex]);
        var transceiver = _transceiverController.GetTransceiver(receiverPeriodicNumber);
        if (transceiver != null)
        {
            transceiver.RxFilterHighLimit = maxLimit;
            transceiver.RxFilterLowLimit = minLimit;
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
    private const int MaxIndex = 3;
    private const int MinIndex = 2;
    private const int ReceiverIndex = 1;
    private const int CommandParameterCount = 5;
}
