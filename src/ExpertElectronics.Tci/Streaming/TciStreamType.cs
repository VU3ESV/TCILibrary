namespace ExpertElectronics.Tci.Streaming;

/// <summary>
/// Stream-type values per the TCI binary frame header (StreamType field).
/// </summary>
public enum TciStreamType : uint
{
    /// <summary>Receiver IQ signal stream (server → client).</summary>
    IqStream = 0,

    /// <summary>Receiver audio stream (server → client).</summary>
    RxAudioStream = 1,

    /// <summary>Transmit audio stream (client → server).</summary>
    TxAudioStream = 2,

    /// <summary>Time markers used to pace TX audio (server → client; no payload).</summary>
    TxChrono = 3,

    /// <summary>Line-out audio stream (server → client). Mirrors the line-out output of the SDR.</summary>
    LineOutStream = 4,
}
