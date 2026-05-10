using System.Globalization;

namespace ExpertElectronics.Tci.TciCommands;

/// <summary>
/// Parses RX_SENSORS push notifications (TCI 1.5+). The receiver's signal level
/// is reported for VFO A only; we write it to channel 0's <see cref="Channel.RxSignalLevelDbm"/>.
/// In TCI 2.0 this command is deprecated in favour of RX_CHANNEL_SENSORS.
/// </summary>
public class TciRxSensorsCommand : ITciCommand, IDisposable
{
    private TciRxSensorsCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciRxSensorsCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciRxSensorsCommand(transceiverController);
    }

    public static string Name => "rx_sensors";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length != 4) return false;
        var trx = _transceiverController.GetTransceiver(Convert.ToUInt32(parts[1]));
        var ch = trx?.Channels.FirstOrDefault(c => c.PeriodicNumber == 0);
        if (ch != null && double.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var dbm))
        {
            ch.RxSignalLevelDbm = dbm;
        }
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
