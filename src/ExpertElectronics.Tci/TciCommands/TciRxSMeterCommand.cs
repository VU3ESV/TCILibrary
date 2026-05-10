namespace ExpertElectronics.Tci.TciCommands;

public class TciRxSMeterCommand : ITciCommand, IDisposable
{
    private TciRxSMeterCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciRxSMeterCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciRxSMeterCommand(transceiverController);
    }

    public static string Name => "rx_smeter";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var enumerable = messages as string[] ?? [.. messages];
        if (!enumerable.Any(_ => _.Contains(Name)))
        {
            return false;
        }

        var rxSMeterMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
        if (string.IsNullOrEmpty(rxSMeterMessage))
        {
            return false;
        }

        var rxSMeterMessageElements = rxSMeterMessage.Split(':', ',', ';');
        if (rxSMeterMessageElements.Length != CommandParameterCount)
        {
            return false;
        }

        var transceiverPeriodicNumber = Convert.ToUInt32(rxSMeterMessageElements[TransceiverIndex]);
        var channelNumber = Convert.ToUInt32(rxSMeterMessageElements[ChannelIndex]);
        var rxSMeter = Convert.ToInt32(rxSMeterMessageElements[RxChannelEnableIndex]);
        var transceiver = _transceiverController.GetTransceiver(transceiverPeriodicNumber);
        var channel = transceiver?.Channels?.FirstOrDefault(_ => _.PeriodicNumber == channelNumber);
        if (channel != null)
        {
            channel.RxSMeter = rxSMeter;
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
    private const int RxChannelEnableIndex = 3;
    private const int CommandParameterCount = 5;
}
