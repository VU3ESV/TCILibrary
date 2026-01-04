namespace ExpertElectronics.Tci.TciCommands;

/// <summary>
/// Represents an IF limits command for the TCI device.
/// </summary>
public class TciIfLimitsCommand : ITciCommand, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TciIfLimitsCommand"/> class.
    /// </summary>
    /// <param name="transceiverController">The transceiver controller used to set IF limits.</param>
    private TciIfLimitsCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciIfLimitsCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciIfLimitsCommand(transceiverController);
    }

    public static string Name => "if_limits";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var enumerable = messages as string[] ?? [.. messages];
        if (!enumerable.Any(_ => _.Contains(Name)))
        {
            return false;
        }

        var ifLimitsMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
        if (string.IsNullOrEmpty(ifLimitsMessage))
        {
            return false;
        }

        var ifLimitsMessageElements = ifLimitsMessage.Split(':', ',', ';');
        if (ifLimitsMessageElements.Length != CommandParameterCount)
        {
            return false;
        }

        _transceiverController.IfMax = Convert.ToInt64(ifLimitsMessageElements[MaxIndex]);
        _transceiverController.IfMin = Convert.ToInt64(ifLimitsMessageElements[MinIndex]);
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
    private const int MaxIndex = 2;
    private const int MinIndex = 1;
    private const int CommandParameterCount = 4;
}
