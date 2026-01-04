using System.Net;
using ExpertElectronics.Tci;
using ExpertElectronics.Tci.Interfaces;
using ExpertElectronics.Tci.Events;
using System.Runtime.Versioning;

namespace StationMonitor;

[SupportedOSPlatform("windows")]
public partial class StationMonitor : Form
{
    private ITciClient? tciClient;
    private ITransceiverController? tranceiverController;

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private Task InvokeOnUiThreadAsync(Action action)
    {
        var tcs = new TaskCompletionSource<bool>();
        if (this.IsHandleCreated)
        {
            this.BeginInvoke((Action)(() =>
            {
                try
                {
                    action();
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
        }
        else
        {
            try
            {
                action();
                tcs.SetResult(true);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        }

        return tcs.Task;
    }
    public StationMonitor()
    {
        InitializeComponent();
        ConnectionStatus.Text = "N";
        ConnectionStatus.BackColor = Color.Gray;
        VolumeControl.Enabled = false;
        DriveControl.Enabled = false;
        TuneControl.Enabled = false;
        StartButton.Enabled = false;
        StopButton.Enabled = false;
        tciServerIP.Enabled = true;
        tciServerPort.Enabled = true;
        R1AtoB.Enabled = false;
        R2AtoB.Enabled = false;
        R1BtoA.Enabled = false;
        R2BtoA.Enabled = false;
        TuneButton.Enabled = false;
        TuneButton.BackColor = Color.Green;
        MuteButton.Enabled = false;
        MuteButton.BackColor = Color.Green;
        R1Split.Enabled = false;
        R1Split.BackColor = Color.Green;
        R2Split.Enabled = false;
        R2Split.BackColor = Color.Green;
    }

    private async void ConnectButton_Click(object sender, EventArgs e)
    {
        var serverIp = tciServerIP.Text;
        if ((serverIp != "localhost") && (!IPAddress.TryParse(serverIp, out _)))
        {
            MessageBox.Show("Enter a Valid Ipaddress, Refer EESDR -> Options -> TCI Tab");
            return;
        }
        var serverPort = Convert.ToUInt32(tciServerPort.Text);

        if (serverPort == 0)
        {
            MessageBox.Show("Enter a Valid Port, Refer EESDR -> Options -> TCI Tab");
            return;
        }

        if (tciClient == null)
        {
            tciClient = TciClient.Create(serverIp, serverPort, _cancellationTokenSource.Token);
            tciClient.OnConnect += TciClient_OnConnect;
            tciClient.OnDisconnect += TciClient_OnDisconnect;
        }
        else
        {
            return;
        }

        await tciClient.ConnectAsync();
        tranceiverController = tciClient.TransceiverController;

        var ctrl = tranceiverController;
        if (ctrl != null)
        {
            await InvokeOnUiThreadAsync(() =>
            {
                Drive.Text = $"Drive: {ctrl.Drive.ToString()}";
                DriveControl.Value = (int)ctrl.Drive;
            });
        }

        if (ctrl != null)
        {
            await InvokeOnUiThreadAsync(() =>
            {
                Tune.Text = $"Tune: {ctrl.TuneDrive.ToString()}";
                TuneControl.Value = (int)ctrl.TuneDrive;
            });
        }

        if (ctrl != null)
        {
            await InvokeOnUiThreadAsync(() =>
            {
                Volume.Text = $"Volume: {ctrl.Volume.ToString()}";
                VolumeControl.Value = ctrl.Volume;
            });
        }

        // use the local controller reference to avoid nullable dereference warnings
        var ctrl2 = tranceiverController;
        if (ctrl2 != null)
        {
            ctrl2.OnVolumeChanged += TransceiverController_OnVolumeChanged;

            await InvokeOnUiThreadAsync(() =>
            {
                foreach (var transceiver in ctrl2.Transceivers)
                {
                    switch (transceiver.PeriodicNumber)
                    {
                        case 0:
                            Tr1ModulationValue.Text = transceiver.Modulation.ToUpperInvariant();
                            break;
                        case 1:
                            Tr2ModulationValue.Text = transceiver.Modulation.ToUpperInvariant();
                            break;
                        default:
                            break;
                    }
                }
            });
        }

        if (tciClient.TransceiverController.IsStarted())
        {
            await InvokeOnUiThreadAsync(() =>
            {
                StartButton.BackColor = Color.Green;
                StopButton.BackColor = Color.Red;
            });
        }
        else
        {
            await InvokeOnUiThreadAsync(() =>
            {
                StartButton.BackColor = Color.Red;
                StopButton.BackColor = Color.Green;
            });
        }

        tciClient.TransceiverController.OnDrive += TransceiverController_OnDrive;
        tciClient.TransceiverController.OnTuneDrive += TransceiverController_OnTuneDrive;
        tciClient.TransceiverController.OnStarted += TransceiverController_OnStarted;
        tciClient.TransceiverController.OnStopped += TransceiverController_OnStopped;
        var transceivers = tciClient?.TransceiverController.Transceivers;
        if (transceivers == null)
        {
            return;
        }

        foreach (var transceiver in transceivers)
        {
            transceiver.OnTrx += TransceiverController_OnTrx;
            transceiver.OnTune += TransceiverController_OnTune;
            transceiver.OnModulationChanged += TransceiverController_OnModulationChanged;
            var channels = transceiver.Channels;
            switch (transceiver.PeriodicNumber)
            {
                case 0:
                    foreach (var channel in channels)
                    {
                        channel.OnVfoChange += TransceiverController_OnVfoChange;
                        switch (channel.PeriodicNumber)
                        {
                            case 0:
                                await InvokeOnUiThreadAsync(() =>
                                {
                                    Receiver1VfoA.Text = channel.Vfo.ToString();
                                });
                                break;
                            case 1:
                                await InvokeOnUiThreadAsync(() =>
                                {
                                    Receiver1VfoB.Text = channel.Vfo.ToString();
                                });
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case 1:
                    foreach (var channel in channels)
                    {
                        channel.OnVfoChange += TransceiverController_OnVfoChange;
                        switch (channel.PeriodicNumber)
                        {
                            case 0:
                                await InvokeOnUiThreadAsync(() =>
                                {
                                    Receiver2VfoA.Text = channel.Vfo.ToString();
                                });
                                break;
                            case 1:
                                await InvokeOnUiThreadAsync(() =>
                                {
                                    Receiver2VfoB.Text = channel.Vfo.ToString();
                                });
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private async void TciClient_OnDisconnect(object? sender, TciConnectedEventArgs e)
    {
        await InvokeOnUiThreadAsync(() =>
        {
            ConnectionStatus.Text = "D";
            ConnectionStatus.BackColor = Color.Red;
            VolumeControl.Enabled = false;
            DriveControl.Enabled = false;
            TuneControl.Enabled = false;
            StartButton.Enabled = false;
            StopButton.Enabled = false;
            tciServerIP.Enabled = true;
            tciServerPort.Enabled = true;
            R1AtoB.Enabled = false;
            R2AtoB.Enabled = false;
            R1BtoA.Enabled = false;
            R2BtoA.Enabled = false;
            TuneButton.Enabled = false;
            MuteButton.Enabled = false;
            R1Split.Enabled = false;
            R2Split.Enabled = false;
        });
    }

    private async void TciClient_OnConnect(object? sender, TciConnectedEventArgs e)
    {
        var ctrl = tranceiverController;
        await InvokeOnUiThreadAsync(() =>
        {
            ConnectionStatus.Text = "C";
            ConnectionStatus.BackColor = Color.Green;
            VolumeControl.Enabled = true;
            DriveControl.Enabled = true;
            TuneControl.Enabled = true;
            StartButton.Enabled = true;
            StopButton.Enabled = true;
            tciServerIP.Enabled = false;
            tciServerPort.Enabled = false;
            R1AtoB.Enabled = true;
            R2AtoB.Enabled = true;
            R1BtoA.Enabled = true;
            R2BtoA.Enabled = true;
            TuneButton.Enabled = true;
            MuteButton.Enabled = true;
            R1Split.Enabled = true;
            R2Split.Enabled = true;
            if (ctrl != null)
            {
                Device.Text = ctrl.Device;
            }
        });
    }

    private async void TransceiverController_OnVolumeChanged(object? sender, IntValueChangedEventArgs e)
    {
        var ctrl = tranceiverController;
        if (ctrl == null)
        {
            return;
        }

        await InvokeOnUiThreadAsync(() =>
        {
            Volume.Text = $"Volume: {ctrl.Volume.ToString()}";
            VolumeControl.Value = ctrl.Volume;
        });
    }

    private async void TransceiverController_OnStopped(object? sender, EventArgs e)
    {
        await InvokeOnUiThreadAsync(() =>
        {
            StartButton.BackColor = Color.Green;
            StopButton.BackColor = Color.Red;
        });
    }

    private async void TransceiverController_OnTune(object? sender, TrxEventArgs e)
    {
        await InvokeOnUiThreadAsync(() =>
        {
            if (e.State == true)
            {
                Tune.BackColor = Color.Red;
            }
            else
            {
                Tune.BackColor = Color.Green;
            }
        });
    }

    private async void TransceiverController_OnTuneDrive(object? sender, UintValueChangedEventArgs e)
    {
        await InvokeOnUiThreadAsync(() =>
        {
            Tune.Text = $"Tune: {e.Value.ToString()}";
            TuneControl.Value = (int)e.Value;
        });
    }

    private async void TransceiverController_OnDrive(object? sender, UintValueChangedEventArgs e)
    {
        await InvokeOnUiThreadAsync(() =>
        {
            Drive.Text = $"Drive: {e.Value.ToString()}";
            DriveControl.Value = (int)e.Value;
        });
    }

    private async void TransceiverController_OnTrx(object? sender, TrxEventArgs e)
    {
        await InvokeOnUiThreadAsync(() =>
        {
            if (e.State == true)
            {
                Tx.Text = "Tx";
                Tx.BackColor = Color.Red;
            }
            else
            {
                Tx.Text = "Rx";
                Tx.BackColor = Color.Green;
            }
        });
    }

    private async void TransceiverController_OnModulationChanged(object? sender, TrxStringValueChangedEventArgs e)
    {
        await InvokeOnUiThreadAsync(() =>
        {
            switch (e.TransceiverPeriodicNumber)
            {
                case 0:
                    Tr1ModulationValue.Text = e.Value.ToString();
                    break;
                case 1:
                    Tr2ModulationValue.Text = e.Value.ToString();
                    break;
                default:
                    break;
            }
        });
    }

    private async void TransceiverController_OnVfoChange(object? sender, VfoChangeEventArgs e)
    {
        try
        {
            switch (e.TransceiverPeriodicNumber)
            {
                case 0:
                    {
                        switch (e.Channel)
                        {
                            case 0:
                                await InvokeOnUiThreadAsync(() =>
                                {
                                    Receiver1VfoA.Text = e.Vfo.ToString();
                                });
                                break;
                            case 1:
                                await InvokeOnUiThreadAsync(() =>
                                {
                                    Receiver1VfoB.Text = e.Vfo.ToString();
                                });
                                break;
                            default:
                                break;
                        }
                        break;
                    }
                case 1:
                    {
                        switch (e.Channel)
                        {
                            case 0:
                                await InvokeOnUiThreadAsync(() =>
                                {
                                    Receiver2VfoA.Text = e.Vfo.ToString();
                                });
                                break;
                            case 1:
                                await InvokeOnUiThreadAsync(() =>
                                {
                                    Receiver2VfoB.Text = e.Vfo.ToString();
                                });
                                break;
                            default:
                                break;
                        }
                        break;
                    }

                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            throw;
        }
    }

    private async void DisconnectButton_Click(object sender, EventArgs e)
    {
        if (tciClient != null)
        {
            await tciClient.DisConnectAsync();
        }
        else
        {
            return;
        }
        var ctrl = tranceiverController;
        if (ctrl != null)
        {
            foreach (var transceiver in ctrl.Transceivers)
            {
                transceiver.OnTrx -= TransceiverController_OnTrx;
                transceiver.OnTune -= TransceiverController_OnTune;
                transceiver.OnModulationChanged -= TransceiverController_OnModulationChanged;
            }

            ctrl.OnDrive -= TransceiverController_OnDrive;
            ctrl.OnTuneDrive -= TransceiverController_OnTuneDrive;
            ctrl.OnStarted -= TransceiverController_OnStarted;
            ctrl.OnStopped -= TransceiverController_OnStopped;
        }

        if (tciClient != null)
        {
            tciClient.OnConnect -= TciClient_OnConnect;
            tciClient.OnDisconnect -= TciClient_OnDisconnect;
            tciClient = null;
        }
    }

    private async void TransceiverController_OnStarted(object? sender, EventArgs e)
    {
        await InvokeOnUiThreadAsync(() =>
        {
            StartButton.BackColor = Color.Red;
            StopButton.BackColor = Color.Green;
        });
    }

    private async void StartButton_Click(object sender, EventArgs e)
    {
        var ctrl = tranceiverController;
        if (ctrl != null)
        {
            await ctrl.StartTransceiver();
        }
    }

    private void StopButton_Click(object sender, EventArgs e)
    {
        var ctrl = tranceiverController;
        if (ctrl != null)
        {
            ctrl.StopTransceiver();
        }
    }

    private async void VolumeControl_Scroll(object sender, EventArgs e)
    {
        var volumeLevel = VolumeControl.Value;
        if (volumeLevel < -60 || volumeLevel > 0)
        {
            return;
        }
        var ctrl = tranceiverController;
        if (ctrl != null)
        {
            await ctrl.SetVolume(volumeLevel);
        }
    }

    private async void DriveControl_Scroll(object sender, EventArgs e)
    {
        var driveLevel = (uint)DriveControl.Value;
        if (driveLevel < 0 || driveLevel > 100)
        {
            return;
        }
        var ctrl = tranceiverController;
        if (ctrl != null)
        {
            await ctrl.SetDrive(driveLevel);
        }

    }

    private async void TuneControl_Scroll(object sender, EventArgs e)
    {
        var driveLevel = (uint)TuneControl.Value;
        if (driveLevel < 0 || driveLevel > 100)
        {
            return;
        }
        var ctrl = tranceiverController;
        if (ctrl != null)
        {
            await ctrl.SetTuneDrive(driveLevel);
        }
    }

    private async void R1AtoB_Click(object sender, EventArgs e)
    {
        var ctrl = tranceiverController;
        if (ctrl != null)
        {
            await ctrl.VfoAToB(0);
        }
    }

    private async void R1BtoA_Click(object sender, EventArgs e)
    {
        var ctrl = tranceiverController;
        if (ctrl != null)
        {
            await ctrl.VfoBToA(0);
        }
    }

    private async void R2AtoB_Click(object sender, EventArgs e)
    {
        var ctrl = tranceiverController;
        if (ctrl != null)
        {
            await ctrl.VfoAToB(1);
        }
    }

    private async void R2BtoA_Click(object sender, EventArgs e)
    {
        var ctrl = tranceiverController;
        if (ctrl != null)
        {
            await ctrl.VfoBToA(1);
        }
    }

    private async void TuneButton_Click(object sender, EventArgs e)
    {
        var ctrl = tranceiverController;
        if (ctrl != null)
        {
            var tuneState = ctrl.Tune(transceiverPeriodicNumber: 0);
            TuneButton.BackColor = tuneState == true ? Color.Green : Color.Red;
            await ctrl.Tune(transceiverPeriodicNumber: 0, !tuneState);
        }
    }

    private async void MuteButton_Click(object sender, EventArgs e)
    {
        var ctrl2 = tranceiverController;
        if (ctrl2 != null)
        {
            var muteState = ctrl2.RxMute(receiverPeriodicNumber: 0);
            MuteButton.BackColor = muteState == true ? Color.Green : Color.Red;
            await ctrl2.RxMute(receiverPeriodicNumber: 0, !muteState);
        }
    }

    private async void R1Split_Click(object sender, EventArgs e)
    {
        var ctrl3 = tranceiverController;
        if (ctrl3 != null)
        {
            var splitState = ctrl3.SplitEnable(transceiverPeriodicNumber: 0);
            R1Split.BackColor = splitState == true ? Color.Green : Color.Red;
            await ctrl3.SplitEnable(transceiverPeriodicNumber: 0, !splitState);
        }
    }

    private async void R2Split_Click(object sender, EventArgs e)
    {
        var ctrl4 = tranceiverController;
        if (ctrl4 != null)
        {
            var splitState = ctrl4.SplitEnable(transceiverPeriodicNumber: 1);
            R2Split.BackColor = splitState == true ? Color.Green : Color.Red;
            await ctrl4.SplitEnable(transceiverPeriodicNumber: 1, !splitState);
        }
    }
}
