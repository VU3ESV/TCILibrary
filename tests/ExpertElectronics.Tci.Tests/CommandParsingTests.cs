using System.Threading;
using System.Threading.Tasks;
using ExpertElectronics.Tci.Events;
using ExpertElectronics.Tci.Interfaces;
using ExpertElectronics.Tci.TciCommands;
using Xunit;

namespace ExpertElectronics.Tci.Tests;

/// <summary>
/// Smoke tests for the parse path. We instantiate <see cref="TransceiverController"/>
/// without a real websocket and feed protocol strings through individual command
/// parsers, then verify the controller / transceiver / channel state was updated.
/// </summary>
public class CommandParsingTests
{
    private static TransceiverController BuildController()
    {
        var handler = new TciMessageHandler();
        var controller = new TransceiverController(handler, new FakeTciClient());
        controller.CreateTransceivers(2);
        controller.CreateChannel(2);
        return controller;
    }

    [Fact]
    public void Lock_command_updates_transceiver_state()
    {
        var c = BuildController();
        var cmd = TciLockCommand.Create(c);
        Assert.True(cmd.ProcessCommandResponses(new[] { "lock:0,true;" }));
        Assert.True(c.GetTransceiver(0).Lock);
        Assert.True(cmd.ProcessCommandResponses(new[] { "lock:1,false;" }));
        Assert.False(c.GetTransceiver(1).Lock);
    }

    [Fact]
    public void VfoLock_command_updates_channel_state()
    {
        var c = BuildController();
        var cmd = TciVfoLockCommand.Create(c);
        Assert.True(cmd.ProcessCommandResponses(new[] { "vfo_lock:0,1,true;" }));
        var ch = c.GetTransceiver(0);
        Assert.True(System.Linq.Enumerable.First(ch.Channels, c1 => c1.PeriodicNumber == 1).VfoLock);
    }

    [Fact]
    public void RxChannelSensors_updates_signal_level()
    {
        var c = BuildController();
        var cmd = TciRxChannelSensorsCommand.Create(c);
        Assert.True(cmd.ProcessCommandResponses(new[] { "rx_channel_sensors:0,0,-71.5;" }));
        var ch = System.Linq.Enumerable.First(c.GetTransceiver(0).Channels, c1 => c1.PeriodicNumber == 0);
        Assert.Equal(-71.5, ch.RxSignalLevelDbm, 3);
    }

    [Fact]
    public void AgcMode_and_AgcGain_update_transceiver_state()
    {
        var c = BuildController();
        Assert.True(TciAgcModeCommand.Create(c).ProcessCommandResponses(new[] { "agc_mode:0,fast;" }));
        Assert.Equal("fast", c.GetTransceiver(0).AgcMode);
        Assert.True(TciAgcGainCommand.Create(c).ProcessCommandResponses(new[] { "agc_gain:0,87;" }));
        Assert.Equal(87, c.GetTransceiver(0).AgcGain);
    }

    [Fact]
    public void MonVolume_and_MonEnable_update_controller_state()
    {
        var c = BuildController();
        Assert.True(TciMonVolumeCommand.Create(c).ProcessCommandResponses(new[] { "mon_volume:-12;" }));
        Assert.Equal(-12, c.MonVolume);
        Assert.True(TciMonEnableCommand.Create(c).ProcessCommandResponses(new[] { "mon_enable:true;" }));
        Assert.True(c.MonEnable);
    }

    [Fact]
    public void DiglOffset_and_DiguOffset_update_controller_state()
    {
        var c = BuildController();
        Assert.True(TciDiglOffsetCommand.Create(c).ProcessCommandResponses(new[] { "digl_offset:1500;" }));
        Assert.Equal(1500, c.DiglOffset);
        Assert.True(TciDiguOffsetCommand.Create(c).ProcessCommandResponses(new[] { "digu_offset:2200;" }));
        Assert.Equal(2200, c.DiguOffset);
    }

    [Fact]
    public void TxFrequency_command_updates_controller_state()
    {
        var c = BuildController();
        Assert.True(TciTxFrequencyCommand.Create(c).ProcessCommandResponses(new[] { "tx_frequency:7140000;" }));
        Assert.Equal(7140000L, c.TxFrequency);
    }

    [Fact]
    public void AppFocus_command_updates_controller_state()
    {
        var c = BuildController();
        Assert.True(TciAppFocusCommand.Create(c).ProcessCommandResponses(new[] { "app_focus:true;" }));
        Assert.True(c.AppFocus);
    }

    [Fact]
    public void Keyer_command_raises_event()
    {
        var c = BuildController();
        KeyerEventArgs received = null;
        c.OnKeyer += (s, e) => received = e;
        var cmd = TciKeyerCommand.Create(c);
        Assert.True(cmd.ProcessCommandResponses(new[] { "keyer:0,true,142;" }));
        Assert.NotNull(received);
        Assert.Equal(0u, received.TransceiverPeriodicNumber);
        Assert.True(received.Pressed);
        Assert.Equal(142, received.PreviousCharacterDurationMs);
    }

    [Fact]
    public void TxSensors_command_raises_event_with_parsed_values()
    {
        var c = BuildController();
        TxSensorsEventArgs received = null;
        c.OnTxSensorsChanged += (s, e) => received = e;
        var cmd = TciTxSensorsCommand.Create(c);
        Assert.True(cmd.ProcessCommandResponses(new[] { "tx_sensors:0,-27.2,47.4,67.5,1.7;" }));
        Assert.NotNull(received);
        Assert.Equal(0u, received.TransceiverPeriodicNumber);
        Assert.Equal(-27.2, received.MicLevelDbm, 3);
        Assert.Equal(47.4, received.SignalPowerWatts, 3);
        Assert.Equal(67.5, received.PeakPowerWatts, 3);
        Assert.Equal(1.7, received.Swr, 3);
    }

    [Fact]
    public void RxNbParam_updates_threshold_and_pulse_duration()
    {
        var c = BuildController();
        Assert.True(TciRxNbParamCommand.Create(c).ProcessCommandResponses(new[] { "rx_nb_param:0,70,25;" }));
        var trx = c.GetTransceiver(0);
        Assert.Equal(70, trx.RxNbThreshold);
        Assert.Equal(25, trx.RxNbPulseDuration);
    }

    [Fact]
    public void ChannelCount_command_uses_singular_wire_name()
    {
        Assert.Equal("channel_count", TciChannelCountCommand.Name);
    }

    [Fact]
    public void Ctcss_commands_update_transceiver_state()
    {
        var c = BuildController();
        Assert.True(TciCtcssEnableCommand.Create(c).ProcessCommandResponses(new[] { "ctcss_enable:0,true;" }));
        Assert.True(c.GetTransceiver(0).CtcssEnable);
        Assert.True(TciCtcssModeCommand.Create(c).ProcessCommandResponses(new[] { "ctcss_mode:0,1;" }));
        Assert.Equal(1, c.GetTransceiver(0).CtcssMode);
        Assert.True(TciCtcssRxToneCommand.Create(c).ProcessCommandResponses(new[] { "ctcss_rx_tone:0,18;" }));
        Assert.Equal(18, c.GetTransceiver(0).CtcssRxTone);
        Assert.True(TciCtcssTxToneCommand.Create(c).ProcessCommandResponses(new[] { "ctcss_tx_tone:0,15;" }));
        Assert.Equal(15, c.GetTransceiver(0).CtcssTxTone);
        Assert.True(TciCtcssLevelCommand.Create(c).ProcessCommandResponses(new[] { "ctcss_level:0,50;" }));
        Assert.Equal(50, c.GetTransceiver(0).CtcssLevel);
    }

    [Fact]
    public void ECoderSwitchRx_raises_event_with_indices()
    {
        var c = BuildController();
        ECoderSwitchEventArgs received = null;
        c.OnECoderRxSwitched += (s, e) => received = e;
        Assert.True(TciECoderSwitchRxCommand.Create(c).ProcessCommandResponses(new[] { "ecoder_switch_rx:0,1;" }));
        Assert.NotNull(received);
        Assert.Equal(0u, received.ECoderPeriodicNumber);
        Assert.Equal(1u, received.TargetPeriodicNumber);
    }

    [Fact]
    public void ECoderSwitchChannel_raises_event_with_indices()
    {
        var c = BuildController();
        ECoderSwitchEventArgs received = null;
        c.OnECoderChannelSwitched += (s, e) => received = e;
        Assert.True(TciECoderSwitchChannelCommand.Create(c).ProcessCommandResponses(new[] { "ecoder_switch_channel:1,0;" }));
        Assert.NotNull(received);
        Assert.Equal(1u, received.ECoderPeriodicNumber);
        Assert.Equal(0u, received.TargetPeriodicNumber);
    }

    [Fact]
    public void RxSensors_writes_signal_level_to_channel_zero()
    {
        var c = BuildController();
        Assert.True(TciRxSensorsCommand.Create(c).ProcessCommandResponses(new[] { "rx_sensors:0,-78.5;" }));
        var ch0 = System.Linq.Enumerable.First(c.GetTransceiver(0).Channels, ch => ch.PeriodicNumber == 0);
        Assert.Equal(-78.5, ch0.RxSignalLevelDbm, 3);
    }

    [Fact]
    public void TxPower_command_updates_controller_state()
    {
        var c = BuildController();
        Assert.True(TciTxPowerCommand.Create(c).ProcessCommandResponses(new[] { "tx_power:13.5;" }));
        Assert.Equal(13.5f, c.TxPower, 3);
    }

    [Fact]
    public void TxSwr_command_updates_controller_state()
    {
        var c = BuildController();
        Assert.True(TciTxSwrCommand.Create(c).ProcessCommandResponses(new[] { "tx_swr:2.4;" }));
        Assert.Equal(2.4f, c.TxSwr, 3);
    }

    private sealed class FakeTciClient : ITciClient
    {
        public ITransceiverController TransceiverController => null;
        public ConnectionStatus ConnectionStatus => ConnectionStatus.Disconnected;
        public Task ConnectAsync() => Task.CompletedTask;
        public Task DisConnectAsync() => Task.CompletedTask;
        public Task SendMessageAsync(string message) => Task.CompletedTask;
        public void Dispose() { }
        public event System.EventHandler<TciConnectedEventArgs> OnConnect { add { } remove { } }
        public event System.EventHandler<TciConnectedEventArgs> OnDisconnect { add { } remove { } }
    }
}
