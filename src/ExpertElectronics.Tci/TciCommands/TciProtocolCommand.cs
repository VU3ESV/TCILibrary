namespace ExpertElectronics.Tci.TciCommands;

/// <summary>
/// Represents a protocol command for the TCI device.
/// </summary>
public class TciProtocolCommand : ITciCommand, IDisposable
{
    public static TciProtocolCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciProtocolCommand(transceiverController);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TciProtocolCommand"/> class.
    /// </summary>
    /// <param name="transceiverController">The transceiver controller used to set protocol/revision info.</param>
    private TciProtocolCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static string Name => "protocol";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var enumerable = messages as string[] ?? [.. messages];
        if (!enumerable.Any(_ => _.Contains(Name)))
        {
            return false;
        }

        var protocolMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
        if (string.IsNullOrEmpty(protocolMessage))
        {
            return false;
        }

        var protocolMessageElements = protocolMessage.Split(':', ',', ';');
        if (protocolMessageElements.Length != CommandParameterCount)
        {
            return false;
        }

        _transceiverController.SoftwareName = protocolMessageElements[SoftwareNameIndex];
        _transceiverController.SoftwareVersion = protocolMessageElements[VersionIndex];
        return true;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private readonly ITransceiverController _transceiverController;
    private const uint CommandParameterCount = 4;
    private const uint SoftwareNameIndex = 1;
    private const uint VersionIndex = 2;
}
