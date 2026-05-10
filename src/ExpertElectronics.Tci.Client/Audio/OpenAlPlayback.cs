using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using Silk.NET.OpenAL;

namespace ExpertElectronics.Tci.Client.Audio;

/// <summary>
/// Per-receiver OpenAL playback queue. Owns its own AL source and a ring of buffers; samples
/// are submitted as int16 PCM (the broadest OpenAL-supported format).
/// </summary>
public sealed unsafe class OpenAlPlayback : IDisposable
{
    private readonly AL _al;
    private readonly uint _source;
    private readonly Queue<uint> _freeBuffers = new();
    private readonly object _gate = new();
    private bool _disposed;
    private bool _started;
    public uint SampleRate { get; private set; } = 48000;
    public int Channels { get; private set; } = 1;

    public OpenAlPlayback(AL al, int bufferCount = 8)
    {
        _al = al;
        _source = _al.GenSource();
        _al.SetSourceProperty(_source, SourceFloat.Gain, 1.0f);
        _al.SetSourceProperty(_source, SourceVector3.Position, 0, 0, 0);

        for (var i = 0; i < bufferCount; i++)
        {
            _freeBuffers.Enqueue(_al.GenBuffer());
        }
    }

    /// <summary>
    /// Push float samples (interleaved if stereo, range ~[-1, 1]) into the playback queue.
    /// </summary>
    public void Submit(float[] samples, uint sampleRate, int channels)
    {
        if (_disposed || samples.Length == 0) return;

        // Convert float to int16. OpenAL doesn't support float natively without an extension.
        var pcm = new short[samples.Length];
        for (var i = 0; i < samples.Length; i++)
        {
            var s = samples[i];
            if (s > 1f) s = 1f; else if (s < -1f) s = -1f;
            pcm[i] = (short)(s * short.MaxValue);
        }

        SubmitPcm16(pcm, sampleRate, channels);
    }

    /// <summary>
    /// Push int16 PCM samples (interleaved) directly. Avoids the float→short conversion path.
    /// </summary>
    public void SubmitPcm16(short[] samples, uint sampleRate, int channels)
    {
        if (_disposed || samples.Length == 0) return;

        SampleRate = sampleRate;
        Channels = channels;

        // Recycle any buffers that have finished playing.
        ReclaimProcessedBuffers();

        uint buffer;
        lock (_gate)
        {
            if (_freeBuffers.Count == 0)
            {
                // Drop this packet rather than blocking the receive loop.
                return;
            }
            buffer = _freeBuffers.Dequeue();
        }

        var format = channels >= 2 ? BufferFormat.Stereo16 : BufferFormat.Mono16;
        fixed (short* p = samples)
        {
            _al.BufferData(buffer, format, p, samples.Length * sizeof(short), (int)sampleRate);
        }
        _al.SourceQueueBuffers(_source, new[] { buffer });

        // Start playback once we've queued our first buffer (or restart if it underran).
        _al.GetSourceProperty(_source, GetSourceInteger.SourceState, out int state);
        if (!_started || state == (int)SourceState.Stopped || state == (int)SourceState.Initial)
        {
            _al.SourcePlay(_source);
            _started = true;
        }
    }

    private void ReclaimProcessedBuffers()
    {
        _al.GetSourceProperty(_source, GetSourceInteger.BuffersProcessed, out int processed);
        if (processed <= 0) return;
        var slot = new uint[processed];
        _al.SourceUnqueueBuffers(_source, slot);
        lock (_gate)
        {
            foreach (var b in slot) _freeBuffers.Enqueue(b);
        }
    }

    public void Stop()
    {
        if (_disposed) return;
        _al.SourceStop(_source);
        ReclaimProcessedBuffers();
        _started = false;
    }

    public void SetGain(float gain)
    {
        if (_disposed) return;
        _al.SetSourceProperty(_source, SourceFloat.Gain, gain);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        try
        {
            _al.SourceStop(_source);
            // Detach all buffers
            _al.SetSourceProperty(_source, SourceInteger.Buffer, 0);
            _al.DeleteSource(_source);
            lock (_gate)
            {
                while (_freeBuffers.Count > 0)
                {
                    var b = _freeBuffers.Dequeue();
                    _al.DeleteBuffer(b);
                }
            }
        }
        catch
        {
            // best-effort cleanup
        }
    }
}
