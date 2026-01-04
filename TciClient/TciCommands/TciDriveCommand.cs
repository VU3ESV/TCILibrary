namespace ExpertElectronics.Tci.TciCommands;

public class TciDriveCommand : ITciCommand, IDisposable
{
    private TciDriveCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciDriveCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciDriveCommand(transceiverController);
    }

    public static string Name => "drive";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var enumerable = messages as string[] ?? [.. messages];
        if (!enumerable.Any(_ => _.Contains(Name)))
        {
            return false;
        }

        var driveMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
        if (string.IsNullOrEmpty(driveMessage))
        {
            return false;
        }

        var driveMessageElements = driveMessage.Split(':', ',', ';');
        if (driveMessageElements.Length != CommandParameterCount)
        {
            return false;
        }

        var drive = Convert.ToUInt32(driveMessageElements[DriveIndex]);
        _transceiverController.Drive = drive;
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
    private const int DriveIndex = 1;
    private const int CommandParameterCount = 3;
}
