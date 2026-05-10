# Architecture

This document describes the architecture of the **ExpertElectronics.Tci** library and maps every TCI protocol command/event to its C# implementation. The library covers the full cumulative protocol surface from v1.4 through v2.0:

- **v2.0** (12 Jan 2024) — [ExpertSDR3/TCI Protocol.pdf](https://github.com/ExpertSDR3/TCI/blob/main/TCI%20Protocol.pdf)
- **v1.6** (2021) — [maksimus1210/TCI/doc/TCI_interface_1.6.pdf](https://github.com/maksimus1210/TCI/blob/master/doc/TCI_interface_1.6.pdf) — adds the RX DSP block (NB/NB_PARAM/BIN/NR/ANC/ANF/APF/DSE/NF) and `TX_FREQUENCY`
- **v1.5** — `TRX`, `DRIVE`, `TUNE_DRIVE`, `RX_SENSORS_ENABLE`, `TX_SENSORS_ENABLE`, `RX_SENSORS`, `TX_SENSORS`, `cw_terminal`, `cw_macros_empty`
- **v1.4** — CTCSS controls (`CTCSS_ENABLE`/`MODE`/`RX_TONE`/`TX_TONE`/`LEVEL`), E-Coder switch (`ECODER_SWITCH_RX`/`ECODER_SWITCH_CHANNEL`), `RX_VOLUME`, `RX_BALANCE`

## 1. Solution layout

```
ExpertElectronics.Tci.sln (TciClient.sln)
├── src/
│   ├── ExpertElectronics.Tci/                 ← protocol library
│   ├── ExpertElectronics.Tci.StationMonitor/  ← WinForms reference UI
│   └── ExpertElectronics.Tci.Console/         ← cross-platform console client
└── tests/
    └── ExpertElectronics.Tci.Tests/           ← xUnit tests
```

The library has zero external dependencies beyond `System.Net.WebSockets` and `System.Drawing`. It targets `net10.0`. The StationMonitor targets `net10.0-windows` (WinForms).

## 2. Layered design

```
┌──────────────────────────────────────────────────────────────┐
│  Client application (StationMonitor / Console / 3rd-party)   │
└──────────────┬───────────────────────────────────────────────┘
               │ subscribes to events, calls async methods
┌──────────────▼───────────────────────────────────────────────┐
│  TciClient                                                   │
│   └── TransceiverController  ← state aggregate, command bus  │
│         ├── Transceiver[]    ← per-radio state               │
│         │     └── Channel[]  ← per-VFO state                 │
│         └── Dictionary<string,ITciCommand>  ← parser table   │
└──────────────┬───────────────────────────────────────────────┘
               │ raw ASCII frames over WebSocket
┌──────────────▼───────────────────────────────────────────────┐
│  TciMessageHandler  →  TciWebSocketClient                    │
└──────────────┬───────────────────────────────────────────────┘
               │ ws://host:port (default 40001)
┌──────────────▼───────────────────────────────────────────────┐
│  ExpertSDR3 server                                           │
└──────────────────────────────────────────────────────────────┘
```

### 2.1 Components

| Component                   | Responsibility                                                                                            |
|-----------------------------|-----------------------------------------------------------------------------------------------------------|
| `TciWebSocketClient`        | Owns the `ClientWebSocket`. Connects, listens, chunks send/receive, surfaces three callbacks.             |
| `TciMessageHandler`         | Adapts raw socket callbacks into typed `EventHandler<T>` events that the controller subscribes to.         |
| `TciClient`                 | Public entry point. Factory creates a wired-up handler/socket/controller graph. Owns the connection lifecycle. |
| `TransceiverController`     | Aggregates state. Discovers `ITciCommand` implementations via reflection. Dispatches incoming messages.   |
| `Transceiver` / `Channel`   | Mutable state holders. Property setters fire the corresponding event.                                     |
| `ITciCommand` implementations | One class per TCI command. Stateless parsers/builders. `Name` is the wire token.                        |

### 2.2 Message flow (server → client)

1. `TciWebSocketClient.StartListen` reads a frame, joins fragments, calls `_onMessage(text, this)`.
2. `TciMessageHandler.OnMessage` raises `OnSocketMessageReceived`.
3. `TransceiverController.MessageHandler_OnSocketMessageReceived` splits on `:` `,` `;`, takes token `[0]`, looks up the command in the parser table, and calls `command.ProcessCommandResponses([message])`.
4. The command parses arguments and writes them to controller / transceiver / channel properties — those setters fire the corresponding events that consumers have subscribed to.

### 2.3 Message flow (client → server)

1. Client calls e.g. `controller.SetDdsFrequency(0, 7100000)`.
2. The method validates and short-circuits if state is unchanged, then calls `TciClient.SendMessageAsync($"{TciDdsCommand.Name}:0,7100000;")`.
3. The websocket client sends the framed text. The server replies with the same command echoed back, which flows through the receive path above and updates local state.

This server-echo pattern is why most setters compare-then-send: the server is authoritative, and the local state is only updated when the echo is parsed.

### 2.4 Command discovery

`TransceiverController.Initialize()` scans `AppDomain.CurrentDomain` for non-interface types implementing `ITciCommand`, invokes each type's static `Create(ITransceiverController)` factory, and indexes the resulting instance by its static `Name`. **There is no manual registration.** Adding a new command class with a unique `Name` is sufficient.

## 3. TCI protocol map

Below: every command and event in TCI v2.0, paired with the implementing class. ✅ = implemented in this repo; ⚠️ = partial; ⛔ = stub / TODO.

### 3.1 Initialization commands (server → client, sent on connect)

| TCI name           | Class                              | Status |
|--------------------|------------------------------------|--------|
| `VFO_LIMITS`       | `TciVfoLimitsCommand`              | ✅     |
| `IF_LIMITS`        | `TciIfLimitsCommand`               | ✅     |
| `TRX_COUNT`        | `TciTrxCountCommand`               | ✅     |
| `CHANNEL_COUNT`    | `TciChannelCountCommand`           | ✅     |
| `DEVICE`           | `TciDeviceCommand`                 | ✅     |
| `RECEIVE_ONLY`     | `TciReceiveOnlyCommand`            | ✅     |
| `MODULATIONS_LIST` | `TciModulationListCommand`         | ✅     |
| `PROTOCOL`         | `TciProtocolCommand`               | ✅     |
| `READY`            | `TciReadyCommand`                  | ✅     |

### 3.2 Bidirectional control commands

| TCI name             | Class                                     | Controller method               | Status |
|----------------------|-------------------------------------------|----------------------------------|--------|
| `START`              | `TciStartCommand`                          | `StartTransceiver()`             | ✅     |
| `STOP`               | `TciStopCommand`                           | `StopTransceiver()`              | ✅     |
| `DDS`                | `TciDdsCommand`                            | `SetDdsFrequency(...)`           | ✅     |
| `IF`                 | `TciIfCommand`                             | `IfFilter(...)`                  | ✅     |
| `VFO`                | `TciVfoCommand`                            | `Vfo(...)`                       | ✅     |
| `MODULATION`         | `TciModulationCommand`                     | `Modulation(...)`                | ✅     |
| `TRX`                | `TciTrxCommand`                            | `Trx(...)`                       | ✅     |
| `TUNE`               | `TciTuneCommand`                           | `Tune(...)`                      | ✅     |
| `DRIVE`              | `TciDriveCommand`                          | `SetDrive(...)`                  | ✅     |
| `TUNE_DRIVE`         | `TciTuneDriveCommand`                      | `SetTuneDrive(...)`              | ✅     |
| `RIT_ENABLE`         | `TciRitEnableCommand`                      | `RitEnable(...)`                 | ✅     |
| `XIT_ENABLE`         | `TciXitEnableCommand`                      | `XitEnable(...)`                 | ✅     |
| `SPLIT_ENABLE`       | `TciSplitEnableCommand`                    | `SplitEnable(...)`               | ✅     |
| `RIT_OFFSET`         | `TciRitOffsetCommand`                      | `RitOffset(...)`                 | ✅     |
| `XIT_OFFSET`         | `TciXitOffsetCommand`                      | `XitOffset(...)`                 | ✅     |
| `RX_CHANNEL_ENABLE`  | `TciRxChannelEnableCommand`                | `ChannelEnable(...)`             | ✅     |
| `RX_FILTER_BAND`     | `TciRxFilterBandsCommand`                  | `RxFilter(...)`                  | ✅     |
| `CW_MACROS_SPEED`    | `TciCWMacrosSpeedCommand`                  | `SetCwMacroSpeed(...)`           | ✅     |
| `CW_MACROS_DELAY`    | `TciCWMacrosDelayCommand`                  | `SetCwMacrosDelay(...)`          | ✅     |
| `CW_KEYER_SPEED`     | `TciCwKeyerSpeedCommand`                   | `SetCwKeyerSpeed(...)`           | ✅ NEW |
| `VOLUME`             | `TciVolumeCommand`                         | `SetVolume(...)`                 | ✅     |
| `MUTE`               | `TciMuteCommand`                           | `SetMute(...)`                   | ✅     |
| `RX_MUTE`            | `TciRxMuteCommand`                         | `RxMute(...)`                    | ✅     |
| `RX_VOLUME`          | `TciRxVolumeCommand`                       | `RxVolume(...)`                  | ✅ NEW |
| `RX_BALANCE`         | `TciRxBalanceCommand`                      | `RxBalance(...)`                 | ✅ NEW |
| `MON_VOLUME`         | `TciMonVolumeCommand`                      | `SetMonVolume(...)`              | ✅ NEW |
| `MON_ENABLE`         | `TciMonEnableCommand`                      | `SetMonEnable(...)`              | ✅ NEW |
| `AGC_MODE`           | `TciAgcModeCommand`                        | `SetAgcMode(...)`                | ✅ NEW |
| `AGC_GAIN`           | `TciAgcGainCommand`                        | `SetAgcGain(...)`                | ✅ NEW |
| `RX_NB_ENABLE`       | `TciRxNbEnableCommand`                     | `RxNbEnable(...)`                | ✅ NEW |
| `RX_NB_PARAM`        | `TciRxNbParamCommand`                      | `RxNbParam(...)`                 | ✅ NEW |
| `RX_BIN_ENABLE`      | `TciRxBinEnableCommand`                    | `RxBinEnable(...)`               | ✅ NEW |
| `RX_NR_ENABLE`       | `TciRxNrEnableCommand`                     | `RxNrEnable(...)`                | ✅ NEW |
| `RX_ANC_ENABLE`      | `TciRxAncEnableCommand`                    | `RxAncEnable(...)`               | ✅ NEW |
| `RX_ANF_ENABLE`      | `TciRxAnfEnableCommand`                    | `RxAnfEnable(...)`               | ✅ NEW |
| `RX_APF_ENABLE`      | `TciRxApfEnableCommand`                    | `RxApfEnable(...)`               | ✅ NEW |
| `RX_DSE_ENABLE`      | `TciRxDseEnableCommand`                    | `RxDseEnable(...)`               | ✅ NEW |
| `RX_NF_ENABLE`       | `TciRxNfEnableCommand`                     | `RxNfEnable(...)`                | ✅ NEW |
| `LOCK`               | `TciLockCommand`                           | `Lock(...)`                      | ✅ NEW |
| `SQL_ENABLE`         | `TciSqlEnableCommand`                      | `SquelchEnable(...)`             | ✅     |
| `SQL_LEVEL`          | `TciSqlLevelCommand`                       | `SquelchLevel(...)`              | ✅     |
| `DIGL_OFFSET`        | `TciDiglOffsetCommand`                     | `SetDiglOffset(...)`             | ✅ NEW |
| `DIGU_OFFSET`        | `TciDiguOffsetCommand`                     | `SetDiguOffset(...)`             | ✅ NEW |
| `CTCSS_ENABLE`       | `TciCtcssEnableCommand`                    | `SetCtcssEnable(...)`            | ✅ v1.4 |
| `CTCSS_MODE`         | `TciCtcssModeCommand`                      | `SetCtcssMode(...)`              | ✅ v1.4 |
| `CTCSS_RX_TONE`      | `TciCtcssRxToneCommand`                    | `SetCtcssRxTone(...)`            | ✅ v1.4 |
| `CTCSS_TX_TONE`      | `TciCtcssTxToneCommand`                    | `SetCtcssTxTone(...)`            | ✅ v1.4 |
| `CTCSS_LEVEL`        | `TciCtcssLevelCommand`                     | `SetCtcssLevel(...)`             | ✅ v1.4 |
| `ECODER_SWITCH_RX`   | `TciECoderSwitchRxCommand`                 | `ECoderSwitchRx(...)`            | ✅ v1.4 |
| `ECODER_SWITCH_CHANNEL` | `TciECoderSwitchChannelCommand`         | `ECoderSwitchChannel(...)`       | ✅ v1.4 |
| `TX_POWER`           | `TciTxPowerCommand`                        | (read-only)                      | ✅ v1.0 |
| `TX_SWR`             | `TciTxSwrCommand`                          | (read-only)                      | ✅ v1.0 |
| `RX_SMETER`          | `TciRxSMeterCommand`                       | `ReadRxSMeter(...)`              | ✅ v1.0 |
| `RX_SENSORS`         | `TciRxSensorsCommand`                      | (notification, deprecated in v2.0)| ✅ v1.5 |

### 3.3 Unidirectional control commands

| TCI name                  | Class                                          | Controller method            | Status |
|---------------------------|------------------------------------------------|-------------------------------|--------|
| `TX_ENABLE`               | `TciTxEnableCommand`                            | (read-only)                   | ✅     |
| `CW_MACROS_SPEED_UP`      | `TciCwMacroSpeedUpCommand`                      | `CwMacroSpeedUp(...)`         | ✅     |
| `CW_MACROS_SPEED_DOWN`    | `TciCwMacroSpeedDownCommand`                    | `CwMacroSpeedDown(...)`       | ✅     |
| `SPOT`                    | `TciSpotCommand`                                | `Spot(...)`                   | ✅     |
| `SPOT_DELETE`             | `TciSpotDeleteCommand`                          | `SpotDelete(...)`             | ✅     |
| `SPOT_CLEAR`              | `TciSpotClearCommand`                           | `SpotClear()`                 | ✅     |
| `IQ_SAMPLERATE`           | `TciIqSampleRateCommand`                        | `SetIqSampleRate(...)`        | ✅     |
| `AUDIO_SAMPLERATE`        | `TciAudioSampleRateCommand`                     | `SetAudioSampleRate(...)`     | ✅     |
| `IQ_START`                | `TciIqStartCommand`                             | `IqStart(...)`                | ✅     |
| `IQ_STOP`                 | `TciIqStopCommand`                              | `IqStop(...)`                 | ✅     |
| `AUDIO_START`             | `TciAudioStartCommand`                          | `AudioStart(...)`             | ✅     |
| `AUDIO_STOP`              | `TciAudioStopCommand`                           | `AudioStop(...)`              | ✅     |
| `LINE_OUT_START`          | `TciLineOutStartCommand`                        | `LineOutStart(...)`           | ✅ NEW |
| `LINE_OUT_STOP`           | `TciLineOutStopCommand`                         | `LineOutStop(...)`            | ✅ NEW |
| `LINE_OUT_RECORDER_START` | `TciLineOutRecorderStartCommand`                | `LineOutRecorderStart(...)`   | ✅ NEW |
| `LINE_OUT_RECORDER_SAVE`  | `TciLineOutRecorderSaveCommand`                 | `LineOutRecorderSave(...)`    | ✅ NEW |
| `LINE_OUT_RECORDER_BREAK` | `TciLineOutRecorderBreakCommand`                | `LineOutRecorderBreak(...)`   | ✅ NEW |
| `AUDIO_STREAM_SAMPLE_TYPE`| `TciAudioStreamSampleTypeCommand`               | `SetAudioStreamSampleType(...)` | ✅ NEW |
| `AUDIO_STREAM_CHANNELS`   | `TciAudioStreamChannelsCommand`                 | `SetAudioStreamChannels(...)` | ✅ NEW |
| `AUDIO_STREAM_SAMPLES`    | `TciAudioStreamSamplesCommand`                  | `SetAudioStreamSamples(...)`  | ✅ NEW |
| `TX_STREAM_AUDIO_BUFFERING`| `TciTxStreamAudioBufferingCommand`             | `SetTxStreamAudioBuffering(...)`| ✅ NEW |
| `SET_IN_FOCUS`            | `TciSetInFocusCommand`                          | `SetInFocus()`                | ✅ NEW |

### 3.4 Notification commands (server → client)

| TCI name             | Class                              | Surfaced as                              | Status |
|----------------------|------------------------------------|------------------------------------------|--------|
| `CLICKED_ON_SPOT`    | `TciClickedOnSpotCommand`          | `OnSpotClicked` event                    | ✅ NEW |
| `RX_CLICKED_ON_SPOT` | `TciRxClickedOnSpotCommand`        | `OnRxSpotClicked` event                  | ✅ NEW |
| `TX_FOOTSWITCH`      | `TciFootSwitchCommand`             | `Transceiver.OnTxFootSwitch`             | ✅     |
| `TX_FREQUENCY`       | `TciTxFrequencyCommand`            | `OnTxFrequencyChanged` event             | ✅ NEW |
| `APP_FOCUS`          | `TciAppFocusCommand`               | `OnAppFocusChanged` event                | ✅ NEW |
| `KEYER`              | `TciKeyerCommand`                  | `OnKeyer` event                          | ✅ NEW |
| `RX_SENSORS_ENABLE`  | `TciRxSensorsEnableCommand`        | `RxSensorsEnable` property               | ✅ NEW |
| `TX_SENSORS_ENABLE`  | `TciTxSensorsEnableCommand`        | `TxSensorsEnable` property               | ✅ NEW |
| `TX_SENSORS`         | `TciTxSensorsCommand`              | `OnTxSensorsChanged` event               | ✅ v1.5 |

### 3.5 New in v2.0

| TCI name              | Class                          | Status |
|-----------------------|--------------------------------|--------|
| `VFO_LOCK`            | `TciVfoLockCommand`            | ✅ NEW |
| `RX_CHANNEL_SENSORS`  | `TciRxChannelSensorsCommand`   | ✅ NEW |

### 3.6 CW macros / messages

| TCI name        | Sent via                                        | Status |
|-----------------|-------------------------------------------------|--------|
| `cw_macros`     | `TransceiverController.SetMacros(...)`          | ✅     |
| `cw_macros_stop`| `TransceiverController.SetCwMacrosStop()`       | ✅     |
| `cw_msg`        | `TransceiverController.CwMessage(...)`          | ✅     |
| `callsign_send` | `TransceiverController.AddCwMessageCallSign(...)` | ✅   |
| `cw_terminal`   | `TransceiverController.SetCwTerminalMode(...)`  | ✅ NEW |
| `cw_macros_empty` | `OnCwMacrosEmpty` event                       | ✅ NEW |

## 4. Data streams (binary)

The TCI protocol multiplexes audio/IQ binary frames over the same WebSocket as the text commands. The library's `DataStream` struct mirrors the on-wire layout (header + samples). Stream types from the spec:

```
IQ_STREAM        = 0   // receiver IQ
RX_AUDIO_STREAM  = 1   // receiver audio
TX_AUDIO_STREAM  = 2   // tx audio (client → server)
TX_CHRONO        = 3   // tx pacing markers
LINEOUT_STREAM   = 4   // line-out duplicate
```

Sample formats (per `AUDIO_STREAM_SAMPLE_TYPE`): `int16`, `int24`, `int32`, `float32`. The library currently exposes the binary frames at the WebSocket level; high-level decode helpers are scoped for a future revision.

## 5. Threading model

- `ClientWebSocket.ReceiveAsync` runs in a dedicated `Task` from `StartListen`.
- Incoming-message handlers (`MessageHandler_OnSocketMessageReceived` → command dispatch → property setter → event invocation) run on that receive task. Subscribers must marshal back to the UI thread themselves (StationMonitor uses `Control.Invoke`).
- Outgoing sends are awaited; there is no internal send queue.

## 6. Extensibility

The library is intentionally reflection-driven for command discovery (see §2.4). New protocol commands added by future TCI revisions only require a new `ITciCommand` class — no manual registration. Properties on `Transceiver` / `Channel` follow the "set + raise event" pattern; conform to it and the consumer experience stays uniform.
