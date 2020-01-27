using System;
using System.Timers;
using System.Linq;
using System.Threading;
using ExpertElectronics.Tci;
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
            Timer timer = new Timer(5000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            try
            {
                tciClient = TciClient.Create("localhost", 40001, cancellationTokenSource.Token);
                tciClient.ConnectAsync().Wait(cancellationTokenSource.Token);
                tciClient.TransceiverController.Start();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Message: {exception.Message} /n StackTrace {exception.StackTrace}");
            }

            exitEvent.WaitOne();

            if (tciClient != null)
            {
                tciClient.TransceiverController.Stop();
                tciClient.DisConnectAsync().Wait(cancellationTokenSource.Token);
            }
            timer.Dispose();
            cancellationTokenSource.Cancel();
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var transceivers = tciClient?.TransceiverController.Transceivers;
            if(transceivers == null)
            {
                return;
            }
           
            foreach (var  transceiver in  transceivers)
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
