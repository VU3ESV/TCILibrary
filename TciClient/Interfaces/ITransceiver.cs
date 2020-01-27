using System.Collections.Generic;

namespace ExpertElectronics.Tci.Interfaces
{
    public interface ITransceiver
    {
        uint PeriodicNumber { get; set; }

        bool TxEnable { get; set; }

        bool TxFootSwitch { get; set; }

        double DdsFrequency { get; set; }

        IEnumerable<Channel> Channels { get; }

        bool Rit { get; set; }

        int RitOffset { get; set; }

        string Modulation { get; set; }

        bool RxEnable { get; set; }

        bool Xit { get; set; }

        bool Split { get; set; }

        int XitOffset { get; set; }

        int RxFilterLowLimit { get; set; }

        int RxFilterHighLimit { get; set; }

        bool TrxEnable { get; set; }

        bool Tune { get; set; }

        bool IqEnable { get; set; }

        bool AudioEnable { get; set; }

        bool Squelch { get; set; }

        int SquelchThreshold { get; set; }

        bool RxMute { get; set; }

        void AddChannel(uint channelNumbers);
    }
}