using System.Globalization;

namespace ExpertElectronics.Tci.TciCommands;

public class TciTxSensorsCommand : ITciCommand, IDisposable
{
    private TciTxSensorsCommand(ITransceiverController transceiverController) => _transceiverController = transceiverController;

    public static TciTxSensorsCommand Create(ITransceiverController transceiverController)
    {
        Debug.Assert(transceiverController != null);
        return new TciTxSensorsCommand(transceiverController);
    }

    public static string Name => "tx_sensors";

    public bool ProcessCommandResponses(IEnumerable<string> messages)
    {
        var msg = messages.FirstOrDefault(m => m.StartsWith(Name + ":", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(msg)) return false;
        var parts = msg.Split(':', ',', ';');
        if (parts.Length < 7) return false;
        var inv = CultureInfo.InvariantCulture;
        var trxNum = Convert.ToUInt32(parts[1]);
        if (!double.TryParse(parts[2], NumberStyles.Any, inv, out var mic)) return false;
        if (!double.TryParse(parts[3], NumberStyles.Any, inv, out var signal)) return false;
        if (!double.TryParse(parts[4], NumberStyles.Any, inv, out var peak)) return false;
        if (!double.TryParse(parts[5], NumberStyles.Any, inv, out var swr)) return false;
        var trx = _transceiverController.GetTransceiver(trxNum);
        if (_transceiverController is TransceiverController c)
            c.RaiseTxSensors(new TxSensorsEventArgs(trxNum, mic, signal, peak, swr));
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    private readonly ITransceiverController _transceiverController;
}
