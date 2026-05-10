namespace ExpertElectronics.Tci.Streaming;

/// <summary>
/// Sample-format values per the TCI binary frame header (Format field).
/// </summary>
public enum TciSampleFormat : uint
{
    Int16 = 0,
    Int24 = 1,
    Int32 = 2,
    Float32 = 3,
}
