using System.Buffers.Binary;

namespace ExpertElectronics.Tci.Streaming;

/// <summary>
/// Parsed view of a TCI binary stream packet. Header layout (little-endian, 64 bytes):
///
/// <code>
///   uint32 receiver
///   uint32 sample_rate
///   uint32 format       // <see cref="TciSampleFormat"/>
///   uint32 codec        // always 0
///   uint32 crc          // always 0
///   uint32 length       // sample count
///   uint32 type         // <see cref="TciStreamType"/>
///   uint32 channels     // 1 or 2 (v2.0; was reserved in v1.6)
///   uint32 reserv[8]    // reserved
/// </code>
///
/// Followed by <c>length * channels * SampleSizeBytes</c> bytes of sample data.
/// </summary>
public sealed class DataStreamPacket
{
    public const int HeaderSize = 64;

    public uint ReceiverNumber { get; init; }
    public uint SampleRate { get; init; }
    public TciSampleFormat SampleFormat { get; init; }
    public uint Codec { get; init; }
    public uint Crc { get; init; }
    public uint Length { get; init; }
    public TciStreamType StreamType { get; init; }
    public uint Channels { get; init; }

    /// <summary>
    /// Raw little-endian sample bytes. Length is <c>Length * Channels * SampleSizeBytes</c>.
    /// </summary>
    public ReadOnlyMemory<byte> SampleBytes { get; init; }

    public int SampleSizeBytes => SampleFormat switch
    {
        TciSampleFormat.Int16 => 2,
        TciSampleFormat.Int24 => 3,
        TciSampleFormat.Int32 => 4,
        TciSampleFormat.Float32 => 4,
        _ => 4,
    };

    /// <summary>
    /// Parses a binary websocket payload into a <see cref="DataStreamPacket"/>.
    /// Returns <c>null</c> if the payload is too small to contain a header.
    /// </summary>
    public static DataStreamPacket Parse(byte[] payload)
    {
        if (payload == null || payload.Length < HeaderSize)
        {
            return null;
        }

        var receiver = BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(0, 4));
        var sampleRate = BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(4, 4));
        var format = BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(8, 4));
        var codec = BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(12, 4));
        var crc = BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(16, 4));
        var length = BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(20, 4));
        var type = BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(24, 4));
        var channels = BinaryPrimitives.ReadUInt32LittleEndian(payload.AsSpan(28, 4));

        // Take the rest of the buffer as the sample data, capped to the announced size if present.
        var sampleStart = HeaderSize;
        var sampleEnd = payload.Length;
        if (channels == 0)
        {
            channels = 1;
        }

        var samples = payload.AsMemory(sampleStart, sampleEnd - sampleStart);

        return new DataStreamPacket
        {
            ReceiverNumber = receiver,
            SampleRate = sampleRate,
            SampleFormat = (TciSampleFormat)format,
            Codec = codec,
            Crc = crc,
            Length = length,
            StreamType = (TciStreamType)type,
            Channels = channels,
            SampleBytes = samples,
        };
    }

    /// <summary>
    /// Returns the samples decoded as <see cref="float"/> in the range roughly [-1, 1] for integer
    /// formats. For IQ streams, samples are interleaved I/Q. Mono audio is single-channel.
    /// </summary>
    public float[] ToFloatSamples()
    {
        var bytes = SampleBytes.Span;
        var sampleCount = bytes.Length / SampleSizeBytes;
        var output = new float[sampleCount];

        switch (SampleFormat)
        {
            case TciSampleFormat.Int16:
                for (var i = 0; i < sampleCount; i++)
                {
                    var v = BinaryPrimitives.ReadInt16LittleEndian(bytes.Slice(i * 2, 2));
                    output[i] = v / 32768f;
                }
                break;

            case TciSampleFormat.Int24:
                for (var i = 0; i < sampleCount; i++)
                {
                    var s = bytes.Slice(i * 3, 3);
                    int v = s[0] | (s[1] << 8) | (s[2] << 16);
                    if ((v & 0x800000) != 0) v |= unchecked((int)0xFF000000);
                    output[i] = v / 8388608f;
                }
                break;

            case TciSampleFormat.Int32:
                for (var i = 0; i < sampleCount; i++)
                {
                    var v = BinaryPrimitives.ReadInt32LittleEndian(bytes.Slice(i * 4, 4));
                    output[i] = v / 2147483648f;
                }
                break;

            case TciSampleFormat.Float32:
            default:
                for (var i = 0; i < sampleCount; i++)
                {
                    output[i] = BinaryPrimitives.ReadSingleLittleEndian(bytes.Slice(i * 4, 4));
                }
                break;
        }

        return output;
    }

    /// <summary>
    /// Builds a binary TX_AUDIO_STREAM packet from float samples for sending to the server.
    /// </summary>
    public static byte[] BuildTxAudioPacket(uint receiverNumber, uint sampleRate, uint channels, float[] samples)
    {
        var sampleBytes = samples.Length * 4;
        var buffer = new byte[HeaderSize + sampleBytes];

        BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(0, 4), receiverNumber);
        BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(4, 4), sampleRate);
        BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(8, 4), (uint)TciSampleFormat.Float32);
        BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(12, 4), 0);
        BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(16, 4), 0);
        BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(20, 4), (uint)samples.Length);
        BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(24, 4), (uint)TciStreamType.TxAudioStream);
        BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(28, 4), channels);

        for (var i = 0; i < samples.Length; i++)
        {
            BinaryPrimitives.WriteSingleLittleEndian(buffer.AsSpan(HeaderSize + i * 4, 4), samples[i]);
        }

        return buffer;
    }
}
