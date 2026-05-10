namespace ExpertElectronics.Tci.TciCommands.Audio;

/// <summary>
/// Represents an audio start command for the TCI device.
/// </summary>
public class TciAudioStartCommand : ITciCommand, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TciAudioStartCommand"/> class.
    /// </summary>
    /// <param name="transceiverController">The transceiver controller used to enable audio.</param>
    private TciAudioStartCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciAudioStartCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciAudioStartCommand(transceiverController);
    }

    public static string Name => "audio_start";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var enumerable = messages as string[] ?? [.. messages];
        if (!enumerable.Any(_ => _.Contains(Name)))
        {
            return false;
        }

        var audioStartMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
        if (string.IsNullOrEmpty(audioStartMessage))
        {
            return false;
        }

        var audioStartMessageElements = audioStartMessage.Split(':', ',', ';');
        if (audioStartMessageElements.Length != CommandParameterCount)
        {
            return false;
        }

        var transceiverPeriodicNumber = Convert.ToUInt32(audioStartMessageElements[TransceiverIndex]);
        var transceiver = _transceiverController.GetTransceiver(transceiverPeriodicNumber); // Fixed typo: GetTransceiver -> GetTransceiver
        if (transceiver != null)
        {
            transceiver.AudioEnable = true;
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
    private const int CommandParameterCount = 3;
}
