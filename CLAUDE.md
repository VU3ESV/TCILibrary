# CLAUDE.md

Guidance for Claude Code working in this repository.

## What this repo is

C# implementation of the Expert Electronics **TCI** (Transceiver Control Interface) protocol — the WebSocket-based control / streaming protocol used by ExpertSDR3 and the SunSDR family of transceivers. Reference protocol: [ExpertSDR3/TCI](https://github.com/ExpertSDR3/TCI), version 2.0 (12 Jan 2024).

The repo ships a library plus reference apps, all targeting **.NET 10**.

## Layout

```
src/
  ExpertElectronics.Tci/         # The protocol library (NuGet-shippable). namespace: ExpertElectronics.Tci
  ExpertElectronics.Tci.Client/  # Cross-platform Avalonia desktop client (Windows/macOS/Linux). Uses Silk.NET.OpenAL.
tests/
  ExpertElectronics.Tci.Tests/   # xUnit unit tests for the library.
docs/
  ARCHITECTURE.md                # Architecture & TCI protocol mapping.
TciClient.sln                    # Solution file (kept name for backwards compat).
```

The Client project is `net10.0` (no platform suffix) — it works cross-platform because Avalonia abstracts the windowing system and Silk.NET.OpenAL.Soft.Native ships per-rid OpenAL binaries.

## Common commands

```bash
# Build the library
dotnet build src/ExpertElectronics.Tci/ExpertElectronics.Tci.csproj

# Build the whole solution
dotnet build TciClient.sln

# Run unit tests
dotnet test tests/ExpertElectronics.Tci.Tests/ExpertElectronics.Tci.Tests.csproj

# Run the desktop client (RX-only)
dotnet run --project src/ExpertElectronics.Tci.Client

# Run the desktop client with TX (mic capture + keying) enabled
dotnet run --project src/ExpertElectronics.Tci.Client -- --enable-tx
```

## Library architecture (what to know before editing)

- **Transport** — `TciWebSocketClient` wraps `ClientWebSocket`. WebSocket connects to `ws://host:port`. Default port for ExpertSDR3 is `40001`.
- **Message format** — ASCII strings. Structure: `name:arg1,arg2,...,argN;`. Reserved separators: `:` `,` `;`. Case-insensitive command names. The library lower-cases names (e.g. the `START` command is sent/parsed as `start`).
- **Routing** — `TciMessageHandler` raises socket events; `TransceiverController` parses incoming messages by splitting on `:`, `,`, `;` and dispatching the first token to a registered `ITciCommand`. Commands implement `ProcessCommandResponses` and mutate state on the controller / `Transceiver` / `Channel`.
- **Command discovery** — `TransceiverController.Initialize()` reflects across the AppDomain looking for non-interface types implementing `ITciCommand`, calls each type's static `Create(this)` factory, and indexes the resulting command instance by its static `Name` property. **Adding a new command class is enough — you do not need to wire it up anywhere.**
- **State exposure** — `TransceiverController` is the public-facing state machine. Per-radio state lives on `Transceiver`; per-receiver-channel state lives on `Channel`. Both raise typed `EventHandler<T>` events when mutated, named `On{Property}Changed` (or similar). Clients subscribe to these events; they are the public API for state changes.

## Adding a new TCI command — checklist

1. Add the class under `src/ExpertElectronics.Tci/TciCommands/` (or `TciCommands/Audio/` if it's an audio-stream control command). Mirror the existing pattern:
   - `public static string Name => "command_name_lowercase";`
   - `private` constructor + `public static Create(ITransceiverController)` factory.
   - `ProcessCommandResponses(IEnumerable<string>)` parses the message and mutates state.
2. If the command introduces new observable state, add a property (with a backing event) to `Transceiver`, `Channel`, or `TransceiverController` and surface it on the matching interface.
3. If the command is client→server (Set), add an awaitable method to `ITransceiverController` and `TransceiverController` that builds and sends the message via `TciClient.SendMessageAsync`.
4. **Do not** manually register the command anywhere — reflection in `Initialize()` picks it up.

## Conventions

- `LangVersion=14`. Library uses C# 14 features freely (primary constructors, collection expressions, etc.). Don't downgrade.
- The library deliberately keeps `Nullable` and `ImplicitUsings` disabled — `GlobalUsings.cs` curates the imports. If you need a new system namespace inside the library, add it there.
- The library has `<GenerateDocumentationFile>true</GenerateDocumentationFile>`. Public API surfaces should carry XML doc comments; missing-comment warnings are silenced via `NoWarn=CS1591` but well-named commands should still document their intent in 1–2 lines.
- Do **not** add multi-paragraph banner comments. Keep XML docs tight.
- `Color` values are passed through `ColorConverterExtensions.ToRgbString()` for the `SPOT` command — keep that as the wire-format converter.

## Testing notes

- Unit tests focus on parsing logic. Each `ITciCommand` is straightforward to test by feeding raw protocol strings into `ProcessCommandResponses` against a minimally-stubbed `ITransceiverController`.
- Integration testing against a live ExpertSDR3 server is out-of-scope here; use the Console client for manual smoke testing.

## TCI protocol reference

See [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) for the full command/event matrix and which classes implement which protocol messages. The authoritative source is the upstream PDF at [`ExpertSDR3/TCI/TCI Protocol.pdf`](https://github.com/ExpertSDR3/TCI/blob/main/TCI%20Protocol.pdf).
