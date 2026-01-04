namespace ExpertElectronics.Tci.TciCommands;

/// <summary>
/// Represents a modulation command for the TCI device.
/// </summary>
public class TciModulationCommand : ITciCommand, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TciModulationCommand"/> class.
    /// </summary>
    /// <param name="transceiverController">The transceiver controller used to update modulation state.</param>
    private TciModulationCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciModulationCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciModulationCommand(transceiverController);
    }

    public static string Name => "modulation";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {

        var enumerable = messages as string[] ?? [.. messages];
        if (!enumerable.Any(_ => _.Contains(Name)))
        {
            return false;
        }

        var modulationMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
        if (string.IsNullOrEmpty(modulationMessage))
        {
            return false;
        }

        var modulationMessageElements = modulationMessage.Split(':', ',', ';');
        if (modulationMessageElements.Length != CommandParameterCount)
        {
            return false;
        }

        var transceiverPeriodicNumber = Convert.ToUInt32(modulationMessageElements[TransceiverIndex]);

        var modulation = modulationMessageElements[ModulationIndex];
        var transceiver = _transceiverController.GetTransceiver(transceiverPeriodicNumber);
        if (transceiver != null)
        {
            transceiver.Modulation = modulation;
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
    private const int ModulationIndex = 2;
    private const int CommandParameterCount = 4;
}
