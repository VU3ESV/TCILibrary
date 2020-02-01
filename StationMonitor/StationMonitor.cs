using System;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Windows.Forms;

using ExpertElectronics.Tci;
using ExpertElectronics.Tci.Interfaces;
using ExpertElectronics.Tci.Events;
using System.Windows.Threading;

namespace StationMonitor
{
    public partial class StationMonitor : Form
    {
        private ITciClient tciClient;
        private ITransceiverController tranaceiverController;

        private Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public StationMonitor()
        {
            InitializeComponent();
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
            }
            else
            {
                return;
            }

            await tciClient.ConnectAsync();
            tranaceiverController = tciClient.TransceiverController;

            await dispatcher.InvokeAsync(() =>
            {
                DriveLevel.Text = tciClient.TransceiverController.Drive.ToString();
            });

            await dispatcher.InvokeAsync(() =>
            {
                TuneLevel.Text = tciClient.TransceiverController.TuneDrive.ToString();
            });

            await dispatcher.InvokeAsync(() =>
            {
                foreach (var transceiver in tciClient.TransceiverController.Transceivers)
                {
                    switch (transceiver.PeriodicNumber)
                    {
                        case 0:
                            Tr1ModulationValue.Text = transceiver.Modulation;
                            break;
                        case 1:
                            Tr2ModulationValue.Text = transceiver.Modulation;
                            break;
                        default:
                            break;
                    }
                }
            });

            if(tciClient.TransceiverController.IsStarted())
            {
                await dispatcher.InvokeAsync(() =>
                {
                    StartButton.BackColor = Color.Green;
                    StopButton.BackColor = Color.Red;
                });
            }
            else
            {
                await dispatcher.InvokeAsync(() =>
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
                                    await dispatcher.InvokeAsync(() =>
                                    {
                                        Receiver1VfoA.Text = channel.Vfo.ToString();
                                    });
                                    break;
                                case 1:
                                    await dispatcher.InvokeAsync(() =>
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
                                    dispatcher.Invoke(() =>
                                    {
                                        Receiver2VfoA.Text = channel.Vfo.ToString();
                                    });
                                    break;
                                case 1:
                                    dispatcher.Invoke(() =>
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

        private async void TransceiverController_OnStopped(object sender, EventArgs e)
        {
            await dispatcher.InvokeAsync(() =>
            {
                StartButton.BackColor = Color.Green;
                StopButton.BackColor = Color.Red;
            });
        }

        private async void TransceiverController_OnTune(object sender, TrxEventArgs e)
        {
            await dispatcher.InvokeAsync(() =>
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

        private async void TransceiverController_OnTuneDrive(object sender, UintValueChangedEventArgs e)
        {
            await dispatcher.InvokeAsync(() =>
            {
                TuneLevel.Text = e.Value.ToString();
            });
        }

        private async void TransceiverController_OnDrive(object sender, UintValueChangedEventArgs e)
        {
            await dispatcher.InvokeAsync(() =>
            {
                DriveLevel.Text = e.Value.ToString();
            });
        }

        private async void TransceiverController_OnTrx(object sender, TrxEventArgs e)
        {
            await dispatcher.InvokeAsync(() =>
            {
                if (e.State == true)
                {
                    Tx.BackColor = Color.Red;
                }
                else
                {
                    Tx.BackColor = Color.Green;
                }
            });
        }

        private async void TransceiverController_OnModulationChanged(object sender, TrxStringValueChangedEventArgs e)
        {
            await dispatcher.InvokeAsync(() =>
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

        private async void TransceiverController_OnVfoChange(object sender, VfoChangeEventArgs e)
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
                                    await dispatcher.InvokeAsync(() =>
                                    {
                                        Receiver1VfoA.Text = e.Vfo.ToString();
                                    });
                                    break;
                                case 1:
                                    await dispatcher.InvokeAsync(() =>
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
                                    await dispatcher.InvokeAsync(() =>
                                    {
                                        Receiver2VfoA.Text = e.Vfo.ToString();
                                    });
                                    break;
                                case 1:
                                    await dispatcher.InvokeAsync(() =>
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

            foreach (var transceiver in tranaceiverController.Transceivers)
            {
                transceiver.OnTrx -= TransceiverController_OnTrx;
                transceiver.OnTune -= TransceiverController_OnTune;
                transceiver.OnModulationChanged -= TransceiverController_OnModulationChanged;
            }

            tciClient.TransceiverController.OnDrive -= TransceiverController_OnDrive;
            tciClient.TransceiverController.OnTuneDrive -= TransceiverController_OnTuneDrive;
            tciClient.TransceiverController.OnStarted -= TransceiverController_OnStarted;
            tciClient.TransceiverController.OnStopped += TransceiverController_OnStopped;
            tciClient = null;
        }

        private async void TransceiverController_OnStarted(object sender, EventArgs e)
        {
            await dispatcher.InvokeAsync(() =>
            {
                StartButton.BackColor = Color.Red;
                StopButton.BackColor = Color.Green;
            });
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            tranaceiverController.StartTransceiver();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            tranaceiverController.StopTransceiver();
        }
    }
}
