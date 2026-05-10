using System;
using System.Buffers.Binary;
using ExpertElectronics.Tci.Streaming;
using Xunit;

namespace ExpertElectronics.Tci.Tests;

public class DataStreamPacketTests
{
    private static byte[] BuildHeader(
        uint receiver = 0,
        uint sampleRate = 48000,
        TciSampleFormat format = TciSampleFormat.Float32,
        uint length = 0,
        TciStreamType type = TciStreamType.RxAudioStream,
        uint channels = 1)
    {
        var header = new byte[DataStreamPacket.HeaderSize];
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(0, 4), receiver);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(4, 4), sampleRate);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(8, 4), (uint)format);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(20, 4), length);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(24, 4), (uint)type);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(28, 4), channels);
        return header;
    }

    [Fact]
    public void Parse_returns_null_for_short_payload()
    {
        Assert.Null(DataStreamPacket.Parse(new byte[10]));
        Assert.Null(DataStreamPacket.Parse(null));
    }

    [Fact]
    public void Parse_extracts_header_fields()
    {
        var header = BuildHeader(receiver: 1, sampleRate: 96000, format: TciSampleFormat.Int16,
                                  length: 256, type: TciStreamType.IqStream, channels: 2);
        var samples = new byte[256 * 2 * 2];
        var packet = DataStreamPacket.Parse(Concat(header, samples));

        Assert.NotNull(packet);
        Assert.Equal(1u, packet.ReceiverNumber);
        Assert.Equal(96000u, packet.SampleRate);
        Assert.Equal(TciSampleFormat.Int16, packet.SampleFormat);
        Assert.Equal(256u, packet.Length);
        Assert.Equal(TciStreamType.IqStream, packet.StreamType);
        Assert.Equal(2u, packet.Channels);
        Assert.Equal(samples.Length, packet.SampleBytes.Length);
    }

    [Fact]
    public void ToFloatSamples_decodes_int16()
    {
        var header = BuildHeader(format: TciSampleFormat.Int16, length: 4, channels: 1);
        // Two samples: max-positive then max-negative
        var samples = new byte[8];
        BinaryPrimitives.WriteInt16LittleEndian(samples.AsSpan(0, 2), short.MaxValue);
        BinaryPrimitives.WriteInt16LittleEndian(samples.AsSpan(2, 2), 0);
        BinaryPrimitives.WriteInt16LittleEndian(samples.AsSpan(4, 2), short.MinValue);
        BinaryPrimitives.WriteInt16LittleEndian(samples.AsSpan(6, 2), 16384);

        var packet = DataStreamPacket.Parse(Concat(header, samples));
        var f = packet.ToFloatSamples();

        Assert.Equal(4, f.Length);
        Assert.InRange(f[0], 0.999f, 1.0f);
        Assert.Equal(0f, f[1], 4);
        Assert.Equal(-1.0f, f[2], 3);
        Assert.Equal(0.5f, f[3], 3);
    }

    [Fact]
    public void ToFloatSamples_passes_through_float32()
    {
        var header = BuildHeader(format: TciSampleFormat.Float32, length: 3, channels: 1);
        var samples = new byte[12];
        BinaryPrimitives.WriteSingleLittleEndian(samples.AsSpan(0, 4), 0.25f);
        BinaryPrimitives.WriteSingleLittleEndian(samples.AsSpan(4, 4), -0.75f);
        BinaryPrimitives.WriteSingleLittleEndian(samples.AsSpan(8, 4), 1.0f);

        var packet = DataStreamPacket.Parse(Concat(header, samples));
        var f = packet.ToFloatSamples();

        Assert.Equal(3, f.Length);
        Assert.Equal(0.25f, f[0], 5);
        Assert.Equal(-0.75f, f[1], 5);
        Assert.Equal(1.0f, f[2], 5);
    }

    [Fact]
    public void BuildTxAudioPacket_writes_expected_header_and_samples()
    {
        var samples = new[] { 0.1f, -0.2f, 0.3f };
        var bytes = DataStreamPacket.BuildTxAudioPacket(receiverNumber: 0, sampleRate: 48000, channels: 1, samples);
        Assert.Equal(DataStreamPacket.HeaderSize + samples.Length * 4, bytes.Length);

        var roundtrip = DataStreamPacket.Parse(bytes);
        Assert.Equal(0u, roundtrip.ReceiverNumber);
        Assert.Equal(48000u, roundtrip.SampleRate);
        Assert.Equal(TciSampleFormat.Float32, roundtrip.SampleFormat);
        Assert.Equal(TciStreamType.TxAudioStream, roundtrip.StreamType);
        Assert.Equal((uint)samples.Length, roundtrip.Length);
        Assert.Equal(1u, roundtrip.Channels);

        var decoded = roundtrip.ToFloatSamples();
        Assert.Equal(samples.Length, decoded.Length);
        for (var i = 0; i < samples.Length; i++)
        {
            Assert.Equal(samples[i], decoded[i], 5);
        }
    }

    private static byte[] Concat(byte[] a, byte[] b)
    {
        var result = new byte[a.Length + b.Length];
        Buffer.BlockCopy(a, 0, result, 0, a.Length);
        Buffer.BlockCopy(b, 0, result, a.Length, b.Length);
        return result;
    }
}
