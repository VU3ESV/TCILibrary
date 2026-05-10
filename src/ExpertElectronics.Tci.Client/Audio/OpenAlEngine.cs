using System;
using Silk.NET.OpenAL;

namespace ExpertElectronics.Tci.Client.Audio;

/// <summary>
/// Owns the global OpenAL device + context. One instance per application.
/// </summary>
public sealed unsafe class OpenAlEngine : IDisposable
{
    private ALContext _alc = null!;
    private AL _al = null!;
    private Device* _device;
    private Context* _context;
    private bool _disposed;

    public AL Al => _al;

    public bool Initialize(out string? error)
    {
        try
        {
            _alc = ALContext.GetApi(true);
            _al = AL.GetApi(true);
            _device = _alc.OpenDevice(string.Empty);
            if (_device == null)
            {
                error = "OpenAL: failed to open default audio device.";
                return false;
            }
            _context = _alc.CreateContext(_device, null);
            _alc.MakeContextCurrent(_context);
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            error = $"OpenAL initialization failed: {ex.Message}";
            return false;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        try
        {
            if (_context != null) _alc.DestroyContext(_context);
            if (_device != null) _alc.CloseDevice(_device);
            _al?.Dispose();
            _alc?.Dispose();
        }
        catch { /* best-effort */ }
    }
}
