using System.Globalization;

namespace ExpertElectronics.Tci.TciCommands;

public class TciTxSwrCommand : ITciCommand, IDisposable
{
    public static TciTxSwrCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciTxSwrCommand(transceiverController);
    }

    private TciTxSwrCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;


    public static string Name => "tx_swr";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var enumerable = messages as string[] ?? [.. messages];
        if (!enumerable.Any(_ => _.Contains(Name, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var txSwrMessage = enumerable.FirstOrDefault(_ => _.Contains(Name, StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(txSwrMessage))
        {
            return false;
        }

        var txSwrMessageElements = txSwrMessage.Split(':', ',', ';');
        if (txSwrMessageElements.Length != CommandParameterCount)
        {
            return false;
        }

        _transceiverController.TxSwr = (float)double.Parse(txSwrMessageElements[TxPowerIndex], CultureInfo.InvariantCulture);
        return true;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private readonly ITransceiverController _transceiverController;
    private const uint CommandParameterCount = 3;
    private const uint TxPowerIndex = 1;
}
