# TCILibrary

A C# implementation of the Expert Electronics **TCI (Transceiver Control Interface) Protocol v2.0** — the WebSocket-based control / streaming protocol used by ExpertSDR3 and the SunSDR family of transceivers. Targets **.NET 10**.

The upstream protocol specification lives at [ExpertSDR3/TCI](https://github.com/ExpertSDR3/TCI/blob/main/TCI%20Protocol.pdf).

## Layout

```
src/
  ExpertElectronics.Tci/         ← protocol library
  ExpertElectronics.Tci.Client/  ← cross-platform Avalonia desktop client
tests/
  ExpertElectronics.Tci.Tests/   ← xUnit tests
docs/
  ARCHITECTURE.md                ← architecture & TCI command/event matrix
CLAUDE.md                        ← guidance for the Claude Code assistant
```

## Build & test

```bash
# Build the whole solution
dotnet build TciClient.sln

# Run unit tests
dotnet test tests/ExpertElectronics.Tci.Tests/ExpertElectronics.Tci.Tests.csproj

# Run the desktop client (defaults to ws://localhost:40001; receive-only by default)
dotnet run --project src/ExpertElectronics.Tci.Client

# To allow microphone capture and TX audio to the radio, opt in explicitly:
dotnet run --project src/ExpertElectronics.Tci.Client -- --enable-tx
```

## Desktop client

The Avalonia client targets `net10.0` and runs on **macOS (x64/arm64), Windows (x64), and Linux (x64)** out of the box. Audio playback / capture uses [Silk.NET.OpenAL.Soft](https://github.com/dotnet/Silk.NET) — the native OpenAL-Soft binaries ship with the NuGet package, so no system install is required.

Features:
- Connection panel (host + port).
- Auto-discovery of every transceiver reported by the TCI server.
- Per-transceiver card with **Start RX / Stop RX / Start TX / Stop TX** buttons, live VFO/modulation readouts, and a sample-rate / byte-counter meter.
- TX audio is **opt-in** via the `--enable-tx` CLI flag. Without it the Start TX button reports the safety guard. With it, the client opens a microphone via OpenAL ALC capture, sends `TRX:n,true,tci`, and streams float32 TX_AUDIO_STREAM packets in response to TX_CHRONO timing markers from the server.
- macOS: the first run with `--enable-tx` will prompt for microphone permission. Linux: the OpenAL-Soft native will use PulseAudio / PipeWire. Windows: WASAPI via OpenAL-Soft.

## What it covers

The library implements the complete TCI v2.0 surface — every command and event in the upstream PDF — including the new v2.0 additions: `VFO_LOCK`, `RX_CHANNEL_SENSORS`, `KEYER`, `CW_KEYER_SPEED`, the full receiver DSP block (`RX_NB`/`RX_BIN`/`RX_NR`/`RX_ANC`/`RX_ANF`/`RX_APF`/`RX_DSE`/`RX_NF` enables and the `RX_NB_PARAM` tuning), AGC controls, monitor controls, line-out streaming + recording, audio-stream tuning, spot click notifications, app-focus, and TX/RX sensor reporting.

See [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) for the full command matrix and the class that implements each protocol message.

## Changelog

- 2026-05-10 — Restructured into `src/` + `tests/` layout. Implemented all v2.0 commands and notifications. Added xUnit test project. Wrote `CLAUDE.md` and `docs/ARCHITECTURE.md`.
- 2026-01-20 — Updated to .NET 10, minor refactoring for the latest language features.
- 2020-01-30 — Added a GUI test app (StationMonitor).
