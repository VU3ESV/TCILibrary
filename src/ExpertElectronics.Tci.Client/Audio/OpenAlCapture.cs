using System;
using Silk.NET.OpenAL;
using Silk.NET.OpenAL.Extensions.EXT;

namespace ExpertElectronics.Tci.Client.Audio;

/// <summary>
/// Microphone capture via the ALC_EXT_CAPTURE extension. The extension is queried at start;
/// if the host's OpenAL implementation doesn't expose capture, <see cref="Start"/> returns false.
/// </summary>
public sealed unsafe class OpenAlCapture : IDisposable
{
    private readonly ALContext _alc;
    private Capture? _capture;
    private Device* _device;
    private bool _disposed;
    public uint SampleRate { get; }
    public int Channels { get; }

    public OpenAlCapture(uint sampleRate = 48000, int channels = 1)
    {
        SampleRate = sampleRate;
        Channels = channels;
        _alc = ALContext.GetApi(true);
    }

    public bool Start(out string? error)
    {
        try
        {
            if (!_alc.TryGetExtension<Capture>(null, out _capture) || _capture is null)
            {
                error = "OpenAL: ALC_EXT_CAPTURE extension is not available on this host.";
                return false;
            }
            var format = Channels >= 2 ? BufferFormat.Stereo16 : BufferFormat.Mono16;
            _device = _capture.CaptureOpenDevice(string.Empty, SampleRate, format, (int)(SampleRate * 2));
            if (_device == null)
            {
                error = "OpenAL: failed to open capture device.";
                return false;
            }
            _capture.CaptureStart(_device);
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            error = $"OpenAL capture failed: {ex.Message}";
            return false;
        }
    }

    /// <summary>
    /// Pulls up to <paramref name="maxSamples"/> int16 samples (interleaved) from the capture
    /// device. Returns the number of samples written into <paramref name="buffer"/>.
    /// </summary>
    public int Read(short[] buffer, int maxSamples)
    {
        if (_capture is null || _device == null) return 0;
        var available = _capture.GetAvailableSamples(_device);
        if (available <= 0) return 0;
        var take = Math.Min(available, maxSamples / Channels);
        if (take <= 0) return 0;
        fixed (short* p = buffer)
        {
            _capture.CaptureSamples(_device, p, take);
        }
        return take * Channels;
    }

    public void Stop()
    {
        if (_capture is null || _device == null) return;
        _capture.CaptureStop(_device);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        try
        {
            if (_capture is not null && _device != null)
            {
                _capture.CaptureStop(_device);
                _capture.CaptureCloseDevice(_device);
            }
            _alc?.Dispose();
        }
        catch { /* best-effort */ }
    }
}
