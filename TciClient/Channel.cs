namespace ExpertElectronics.Tci
{
    public class Channel
    {
        public Channel(uint periodicNumber)
        {
            PeriodicNumber = periodicNumber;
        }
        public bool Enable { get; set; }
        public uint PeriodicNumber { get; }
        public double IfFilter { get; set; }
        public int RxSMeter { get; set; }
        public long Vfo { get; set; }

        public bool ReceiveOnly { get; set; }
    }
}