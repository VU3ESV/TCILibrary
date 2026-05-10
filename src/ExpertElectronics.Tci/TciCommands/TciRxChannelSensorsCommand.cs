using System.Globalization;

namespace ExpertElectronics.Tci.TciCommands;

public class TciRxChannelSensorsCommand : ITciCommand, IDisposable
{
    private TciRxChannelSensorsCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciRxChannelSensorsCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciRxChannelSensorsCommand(transceiverController);
    }

    public static string Name => "rx_channel_sensors";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 5) return false;
        var trx = _transceiverController.GetTransceiver(Convert.ToUInt32(parts[1]));
        var ch = trx?.Channels.FirstOrDefault(c => c.PeriodicNumber == Convert.ToUInt32(parts[2]));
        if (ch != null && double.TryParse(parts[3], NumberStyles.Any, CultureInfo.InvariantCulture, out var dbm))
        {
            ch.RxSignalLevelDbm = dbm;
        }
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
