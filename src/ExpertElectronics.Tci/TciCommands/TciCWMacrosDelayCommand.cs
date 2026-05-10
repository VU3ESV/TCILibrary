namespace ExpertElectronics.Tci.TciCommands;

/// <summary>
/// Represents a CWMacros delay command for the TCI device.
/// </summary>
public class TciCwMacrosDelayCommand : ITciCommand, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TciCwMacrosDelayCommand"/> class.
    /// </summary>
    /// <param name="transceiverController">The transceiver controller used to update CW macros delay.</param>
    private TciCwMacrosDelayCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciCwMacrosDelayCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciCwMacrosDelayCommand(transceiverController);
    }

    public static string Name => "cw_macros_delay";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var enumerable = messages as string[] ?? [.. messages];
        if (!enumerable.Any(_ => _.Contains(Name)))
        {
            return false;
        }

        var cwDelayMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
        if (string.IsNullOrEmpty(cwDelayMessage))
        {
            return false;
        }

        var cwDelayElements = cwDelayMessage.Split(':', ',', ';');
        if (cwDelayElements.Length != CommandParameterCount)
        {
            return false;
        }

        var cwDelay = Convert.ToUInt32(cwDelayElements[CwDelayIndex]);
        _transceiverController.CwMacrosDelay = cwDelay;
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
    private const int CwDelayIndex = 1;
    private const int CommandParameterCount = 3;
}
