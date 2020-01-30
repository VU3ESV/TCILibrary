using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            tranaceiverController.Start();
            dispatcher.Invoke(() =>
            {
                DriveLevel.Text = tciClient.TransceiverController.Drive.ToString();
            });

            dispatcher.Invoke(() =>
            {
                TuneLevel.Text = tciClient.TransceiverController.TuneDrive.ToString();
            });

            dispatcher.Invoke(() =>
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

            tciClient.TransceiverController.OnVfoChange += TransceiverController_OnVfoChange;
            tciClient.TransceiverController.OnTrx += TransceiverController_OnTrx;
            tciClient.TransceiverController.OnDrive += TransceiverController_OnDrive;
            tciClient.TransceiverController.OnTuneDrive += TransceiverController_OnTuneDrive;
            tciClient.TransceiverController.OnTune += TransceiverController_OnTune;
            tciClient.TransceiverController.OnModulationChanged += TransceiverController_OnModulationChanged;

            var transceivers = tciClient?.TransceiverController.Transceivers;
            if (transceivers == null)
            {
                return;
            }

            foreach (var transceiver in transceivers)
            {
                var channels = transceiver.Channels;
                switch (transceiver.PeriodicNumber)
                {
                    case 0:
                        foreach (var channel in channels)
                        {
                            switch (channel.PeriodicNumber)
                            {
                                case 0:
                                    dispatcher.Invoke(() =>
                                    {
                                        Receiver1VfoA.Text = channel.Vfo.ToString();
                                    });
                                    break;
                                case 1:
                                    dispatcher.Invoke(() =>
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

        private void TransceiverController_OnTune(object sender, TrxEventArgs e)
        {
            dispatcher.Invoke(() =>
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

        private void TransceiverController_OnTuneDrive(object sender, UintValueChangedEventArgs e)
        {
            dispatcher.Invoke(() =>
            {
                TuneLevel.Text = e.Value.ToString();
            });
        }

        private void TransceiverController_OnDrive(object sender, UintValueChangedEventArgs e)
        {
            dispatcher.Invoke(() =>
            {
                DriveLevel.Text = e.Value.ToString();
            });
        }

        private void TransceiverController_OnTrx(object sender, TrxEventArgs e)
        {
            dispatcher.Invoke(() =>
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

        private void TransceiverController_OnModulationChanged(object sender, TrxStringValueChangedEventArgs e)
        {
            dispatcher.Invoke(() =>
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

        private void TransceiverController_OnVfoChange(object sender, VfoChangeEventArgs e)
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
                                    dispatcher.Invoke(() =>
                                    {
                                        Receiver1VfoA.Text = e.Vfo.ToString();
                                    });
                                    break;
                                case 1:
                                    dispatcher.Invoke(() =>
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
                                    dispatcher.Invoke(() =>
                                    {
                                        Receiver2VfoA.Text = e.Vfo.ToString();
                                    });
                                    break;
                                case 1:
                                    dispatcher.Invoke(() =>
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

            tciClient.TransceiverController.OnVfoChange -= TransceiverController_OnVfoChange;
            tciClient.TransceiverController.OnTrx -= TransceiverController_OnTrx;
            tciClient.TransceiverController.OnDrive -= TransceiverController_OnDrive;
            tciClient.TransceiverController.OnTuneDrive -= TransceiverController_OnTuneDrive;
            tciClient.TransceiverController.OnTune -= TransceiverController_OnTune;
            tciClient.TransceiverController.OnModulationChanged -= TransceiverController_OnModulationChanged;

            tciClient = null;
        }
    }
}
