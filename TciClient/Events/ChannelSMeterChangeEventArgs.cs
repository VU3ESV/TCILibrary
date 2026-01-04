// Copyright (c) 2019 Vinod ES (VU3ESV)
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

namespace ExpertElectronics.Tci.Events;

public class ChannelSMeterChangeEventArgs : EventArgs
{
    public ChannelSMeterChangeEventArgs(uint transceiverPeriodicNumber, uint channel, int sMeter)
    {
        TransceiverPeriodicNumber = transceiverPeriodicNumber;
        Channel = channel;
        SMeter = sMeter;
    }

    public uint TransceiverPeriodicNumber { get; }

    public uint Channel { get; }

    public int SMeter { get; }
}