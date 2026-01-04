namespace ExpertElectronics.Tci.TciCommands;

public class TciChannelCountCommand : ITciCommand, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TciChannelCountCommand"/> class.
    /// </summary>
    /// <param name="transceiverController">The transceiver controller used to create channels.</param>
    private TciChannelCountCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciChannelCountCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciChannelCountCommand(transceiverController);
    }

    /// <summary>
    /// Gets the command name used in TCI responses.
    /// </summary>
    public static string Name => "channels_count";

    /// <summary>
    /// Processes a collection of TCI response messages and handles the channels_count response.
    /// </summary>
    /// <param name="messages">The incoming response messages to inspect.</param>
    /// <returns>True if a channels_count response was found and processed; otherwise false.</returns>
    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var enumerable = messages as string[] ?? [.. messages];
        if (!enumerable.Any(_ => _.Contains(Name)))
        {
            return false;
        }

        var channelCountMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
        if (string.IsNullOrEmpty(channelCountMessage))
        {
            return false;
        }

        var channelCountMessageElements = channelCountMessage.Split(':', ',', ';');
        if (channelCountMessageElements.Length != CommandParameterCount)
        {
            return false;
        }

        var channelCount = Convert.ToUInt32(channelCountMessageElements[TransceiverIndex]);
        (_transceiverController as TransceiverController).CreateChannel(channelCount);
        return true;
    }


    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// Suppresses finalization for this instance.
    /// </summary>
    public void Dispose()
    {
        if (_transceiverController == null)
        {
            return;
        }

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// The transceiver controller used to interact with transceiver channels.
    /// </summary>
    private readonly ITransceiverController _transceiverController;

    /// <summary>
    /// Index of the transceiver id within the parsed command parameters.
    /// </summary>
    private const int TransceiverIndex = 1;

    /// <summary>
    /// Expected number of parameters in the channels_count command message.
    /// </summary>
    private const int CommandParameterCount = 3;
}
