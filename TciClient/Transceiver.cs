using System.Collections.Generic;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci
{
    public class Transceiver : ITransceiver
    {
        public Transceiver(uint periodicNumber)
        {
            _channels = new List<Channel>();
            PeriodicNumber = periodicNumber;
        }

        public uint PeriodicNumber { get; set; }

        public bool TxEnable { get; set; }

        public bool TxFootSwitch { get; set; }

        public double DdsFrequency { get; set; }

        public IEnumerable<Channel> Channels => _channels;

        public bool Rit { get; set; }

        public int RitOffset { get; set; }

        public string Modulation { get; set; }

        public bool RxEnable { get; set; }

        public bool Xit { get; set; }

        public bool Split { get; set; }

        public int XitOffset { get; set; }

        public int RxFilterLowLimit { get; set; }

        public int RxFilterHighLimit { get; set; }

        public bool TrxEnable { get; set; }

        public bool Tune { get; set; }

        public bool IqEnable { get; set; }

        public bool AudioEnable { get; set; }

        public bool Squelch { get; set; }

        public int SquelchThreshold { get; set; }

        public bool RxMute { get; set; }

        public void AddChannel(uint channelNumbers)
        {
            for (uint i = 0; i < channelNumbers; i++)
            {
                _channels.Add(new Channel(i));
            }
        }

        private readonly List<Channel> _channels;
    }
}
