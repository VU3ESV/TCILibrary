using System;
using System.Timers;
using System.Linq;
using System.Threading;
using ExpertElectronics.Tci;
using ExpertElectronics.Tci.Events;
using Timer = System.Timers.Timer;

namespace TestApp
{
    class Program
    {
        static TciClient tciClient = null;
        private static void Main(string[] args)
        {
            var exitEvent = new ManualResetEvent(false);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                exitEvent.Set();
            };
            Timer timer = new Timer(60000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            try
            {
                tciClient = TciClient.Create("localhost", 40001, cancellationTokenSource.Token);
                tciClient.ConnectAsync().Wait(cancellationTokenSource.Token);
                //tciClient.TransceiverController.OnVfoChange += TransceiverController_OnVfoChange;
                tciClient.TransceiverController.OnAudioStartChanged += TransceiverController_OnAudioStartChanged;
                tciClient.TransceiverController.OnDrive += TransceiverController_OnDrive;
                tciClient.TransceiverController.OnTuneDrive += TransceiverController_OnTuneDrive; 
                //tciClient.TransceiverController.OnIfFreqChanged += TransceiverController_OnIfFreqChanged;
                tciClient.TransceiverController.OnIfLimitsChanged += TransceiverController_OnIfLimitsChanged;
                tciClient.TransceiverController.OnModulationChanged += TransceiverController_OnModulationChanged;
                tciClient.TransceiverController.OnSplitEnableChanged += TransceiverController_OnSplitEnableChanged;
                tciClient.TransceiverController.OnTrx += TransceiverController_OnTrx;
                tciClient.TransceiverController.OnTune += TransceiverController_OnTune;
                tciClient.TransceiverController.OnVolumeChanged += TransceiverController_OnVolumeChanged;
                tciClient.TransceiverController.OnXitEnableChanged += TransceiverController_OnXitEnableChanged;
                tciClient.TransceiverController.OnXitOffsetChanged += TransceiverController_OnXitOffsetChanged;
                tciClient.TransceiverController.OnRitEnableChanged += TransceiverController_OnRitEnableChanged;
                tciClient.TransceiverController.OnRitOffsetChanged += TransceiverController_OnRitOffsetChanged;
                tciClient.TransceiverController.OnMute += TransceiverController_OnMute;
                //tciClient.TransceiverController.OnChannelSMeterChanged += TransceiverController_OnChannelSMeterChanged;
                tciClient.TransceiverController.OnRxFilterChanged += TransceiverController_OnRxFilterChanged;
                tciClient.TransceiverController.OnRxMute += TransceiverController_OnRxMute;
                tciClient.TransceiverController.Start();
                tciClient.TransceiverController.Vfo(0, 0, 7154400);


            }
            catch (Exception exception)
            {
                Console.WriteLine($"Message: {exception.Message} /n StackTrace {exception.StackTrace}");
            }

            exitEvent.WaitOne();

            if (tciClient != null)
            {
                tciClient.TransceiverController.Stop();
               // tciClient.TransceiverController.OnVfoChange -= TransceiverController_OnVfoChange;
                tciClient.TransceiverController.OnAudioStartChanged -= TransceiverController_OnAudioStartChanged;
                tciClient.TransceiverController.OnDrive -= TransceiverController_OnDrive;
                tciClient.TransceiverController.OnTuneDrive -= TransceiverController_OnTuneDrive; 
                //tciClient.TransceiverController.OnIfFreqChanged -= TransceiverController_OnIfFreqChanged;
                tciClient.TransceiverController.OnIfLimitsChanged -= TransceiverController_OnIfLimitsChanged;
                tciClient.TransceiverController.OnModulationChanged -= TransceiverController_OnModulationChanged;
                tciClient.TransceiverController.OnSplitEnableChanged -= TransceiverController_OnSplitEnableChanged;
                tciClient.TransceiverController.OnTrx -= TransceiverController_OnTrx;
                tciClient.TransceiverController.OnTune -= TransceiverController_OnTune;
                tciClient.TransceiverController.OnVolumeChanged -= TransceiverController_OnVolumeChanged;
                tciClient.TransceiverController.OnXitEnableChanged -= TransceiverController_OnXitEnableChanged;
                tciClient.TransceiverController.OnXitOffsetChanged -= TransceiverController_OnXitOffsetChanged;
                tciClient.TransceiverController.OnRitEnableChanged -= TransceiverController_OnRitEnableChanged;
                tciClient.TransceiverController.OnRitOffsetChanged -= TransceiverController_OnRitOffsetChanged;
                tciClient.TransceiverController.OnMute -= TransceiverController_OnMute;
                //tciClient.TransceiverController.OnChannelSMeterChanged -= TransceiverController_OnChannelSMeterChanged;
                tciClient.TransceiverController.OnRxFilterChanged -= TransceiverController_OnRxFilterChanged;
                tciClient.TransceiverController.OnRxMute -= TransceiverController_OnRxMute;

                tciClient.DisConnectAsync().Wait(cancellationTokenSource.Token);
            }
            timer.Dispose();
            cancellationTokenSource.Cancel();
        }

        private static void TransceiverController_OnTuneDrive(object sender, UintValueChangedEventArgs e)
        {
            Console.WriteLine($"TuneDrive: {e.Value}");
        }

        private static void TransceiverController_OnRxMute(object sender, TrxEventArgs e)
        {
            Console.WriteLine($"Receiver :{e.TransceiverPeriodicNumber}, Rx Mute: {e.State}");
        }

        private static void TransceiverController_OnRxFilterChanged(object sender, RxFilterChangedEventArgs e)
        {
            Console.WriteLine($"Receiver :{e.TransceiverPeriodicNumber}, Filter Low: {e.Low}, High : {e.High}");
        }

        private static void TransceiverController_OnChannelSMeterChanged(object sender, ChannelSMeterChangeEventArgs e)
        {
            Console.WriteLine($"Transceiver :{e.TransceiverPeriodicNumber}, Channel: {e.Channel}, SMeter : {e.SMeter}");
        }

        private static void TransceiverController_OnMute(object sender, TrxEventArgs e)
        {
            Console.WriteLine($"Transceiver :{e.TransceiverPeriodicNumber}, Mute : {e.State}");
        }

        private static void TransceiverController_OnRitOffsetChanged(object sender, TrxIntValueChangedEventArgs e)
        {
            Console.WriteLine($"Transceiver :{e.TransceiverPeriodicNumber}, RitOffset : {e.Value}");
        }

        private static void TransceiverController_OnRitEnableChanged(object sender, TrxEventArgs e)
        {
            Console.WriteLine($"Transceiver :{e.TransceiverPeriodicNumber}, RitEnabe : {e.State}");
        }

        private static void TransceiverController_OnXitOffsetChanged(object sender, TrxIntValueChangedEventArgs e)
        {
            Console.WriteLine($"Transceiver :{e.TransceiverPeriodicNumber}, XitOffset : {e.Value}");
        }

        private static void TransceiverController_OnXitEnableChanged(object sender, TrxEventArgs e)
        {
            Console.WriteLine($"Transceiver :{e.TransceiverPeriodicNumber}, XitEnabe : {e.State}");
        }

        private static void TransceiverController_OnVolumeChanged(object sender, IntValueChangedEventArgs e)
        {
            Console.WriteLine($"Volume: {e.Value}");
        }

        private static void TransceiverController_OnTune(object sender, TrxEventArgs e)
        {
            Console.WriteLine($"Transceiver :{e.TransceiverPeriodicNumber}, Tune : {e.State}");
        }

        private static void TransceiverController_OnTrx(object sender, TrxEventArgs e)
        {
            Console.WriteLine($"Transceiver :{e.TransceiverPeriodicNumber}, Tx : {e.State}");
        }

        private static void TransceiverController_OnSplitEnableChanged(object sender, TrxEventArgs e)
        {
            Console.WriteLine($"Transceiver :{e.TransceiverPeriodicNumber}, Split : {e.State}");
        }

        private static void TransceiverController_OnModulationChanged(object sender, TrxStringValueChangedEventArgs e)
        {
            Console.WriteLine($"Transceiver :{e.TransceiverPeriodicNumber}, Modulation : {e.Value}");
        }

        private static void TransceiverController_OnIfLimitsChanged(object sender, IfLimitsChangedEventArgs e)
        {
            Console.WriteLine($"If Limits Max: {e.Max}, Min: {e.Min}");
        }

        private static void TransceiverController_OnIfFreqChanged(object sender, IfFrequencyChangedEventArgs e)
        {
            Console.WriteLine($"Transceiver :{e.TransceiverPeriodicNumber}, IF Freq : {e.Value}");
        }

        private static void TransceiverController_OnDrive(object sender, UintValueChangedEventArgs e)
        {
            Console.WriteLine($"Drive: {e.Value}");
        }

        private static void TransceiverController_OnAudioStartChanged(object sender, UintValueChangedEventArgs e)
        {
            Console.WriteLine($"AudioStart = {e.Value}");
        }

        private static void TransceiverController_OnVfoChange(object sender, VfoChangeEventArgs e)
        {
            Console.WriteLine($"Transceiver {e.TransceiverPeriodicNumber} : Channel {e.Channel} : Vfo {e.Vfo}");
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            var transceivers = tciClient?.TransceiverController.Transceivers;
            if (transceivers == null)
            {
                return;
            }

            foreach (var transceiver in transceivers)
            {
                Console.WriteLine($"{nameof(transceiver.PeriodicNumber)}: {transceiver.PeriodicNumber}");
                Console.WriteLine($"{nameof(transceiver.Modulation)}: {transceiver.Modulation}");
                Console.WriteLine($"{nameof(transceiver.IqEnable)}: {transceiver.IqEnable}");
                Console.WriteLine($"{nameof(transceiver.Rit)}: {transceiver.Rit}");
                Console.WriteLine($"{nameof(transceiver.RitOffset)}: {transceiver.RitOffset}");
                Console.WriteLine($"{nameof(transceiver.RxEnable)}: {transceiver.RxEnable}");
                Console.WriteLine($"{nameof(transceiver.RxFilterHighLimit)}: {transceiver.RxFilterHighLimit}");
                Console.WriteLine($"{nameof(transceiver.RxFilterLowLimit)}: {transceiver.RxFilterLowLimit}");
                Console.WriteLine($"{nameof(transceiver.RxMute)}: {transceiver.RxMute}");
                Console.WriteLine($"{nameof(transceiver.Split)}: {transceiver.Split}");
                Console.WriteLine($"{nameof(transceiver.Squelch)}: {transceiver.Squelch}");
                Console.WriteLine($"{nameof(transceiver.SquelchThreshold)}: {transceiver.SquelchThreshold}");
                Console.WriteLine($"{nameof(transceiver.TrxEnable)}: {transceiver.TxEnable}");
                Console.WriteLine($"{nameof(transceiver.TxFootSwitch)}: {transceiver.TxFootSwitch}");
                Console.WriteLine($"{nameof(transceiver.Xit)}: {transceiver.Xit}");
                Console.WriteLine($"{nameof(transceiver.XitOffset)}: {transceiver.XitOffset}");
                Console.WriteLine($"{nameof(transceiver.AudioEnable)}: {transceiver.AudioEnable}");
                Console.WriteLine($"{nameof(transceiver.DdsFrequency)}: {transceiver.DdsFrequency}");
                var channels = transceiver.Channels;
                foreach (var channel in channels)
                {
                    Console.WriteLine($"Channel {nameof(channel.PeriodicNumber)}: {channel.PeriodicNumber}");
                    Console.WriteLine($"{nameof(channel.IfFilter)}: {channel.IfFilter}");
                    Console.WriteLine($"{nameof(channel.Enable)}: {channel.Enable}");
                    Console.WriteLine($"{nameof(channel.ReceiveOnly)}: {channel.ReceiveOnly}");

                    Console.WriteLine($"{nameof(channel.RxSMeter)}: {channel.RxSMeter}");
                    Console.WriteLine($"{nameof(channel.Vfo)}: {channel.Vfo}");
                }

                Console.WriteLine("***********************************************************************");
            }

            Console.WriteLine("################################################################################");

        }
    }
}
