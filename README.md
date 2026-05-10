# TCILibrary

A C# implementation of the Expert Electronics **TCI (Transceiver Control Interface) Protocol v2.0** — the WebSocket-based control / streaming protocol used by ExpertSDR3 and the SunSDR family of transceivers. Targets **.NET 10**.

The upstream protocol specification lives at [ExpertSDR3/TCI](https://github.com/ExpertSDR3/TCI/blob/main/TCI%20Protocol.pdf).

## Layout

```
src/
  ExpertElectronics.Tci/         ← protocol library
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
```

## What it covers

The library implements the complete TCI v2.0 surface — every command and event in the upstream PDF — including the new v2.0 additions: `VFO_LOCK`, `RX_CHANNEL_SENSORS`, `KEYER`, `CW_KEYER_SPEED`, the full receiver DSP block (`RX_NB`/`RX_BIN`/`RX_NR`/`RX_ANC`/`RX_ANF`/`RX_APF`/`RX_DSE`/`RX_NF` enables and the `RX_NB_PARAM` tuning), AGC controls, monitor controls, line-out streaming + recording, audio-stream tuning, spot click notifications, app-focus, and TX/RX sensor reporting.

See [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) for the full command matrix and the class that implements each protocol message.

## Changelog

- 2026-05-10 — Restructured into `src/` + `tests/` layout. Implemented all v2.0 commands and notifications. Added xUnit test project. Wrote `CLAUDE.md` and `docs/ARCHITECTURE.md`.
- 2026-01-20 — Updated to .NET 10, minor refactoring for the latest language features.
- 2020-01-30 — Added a GUI test app (StationMonitor).
