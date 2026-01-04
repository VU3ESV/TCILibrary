// Copyright (c) 2019 Vinod ES (VU3ESV)
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

namespace ExpertElectronics.Tci.Events;

public class VfoChangeEventArgs : EventArgs
{
    public VfoChangeEventArgs(uint transceiverPeriodicNumber, uint channel, long vfo)
    {
        TransceiverPeriodicNumber = transceiverPeriodicNumber;
        Channel = channel;
        Vfo = vfo;
    }

    public uint TransceiverPeriodicNumber { get; }

    public uint Channel { get; }

    public long Vfo { get; }
}