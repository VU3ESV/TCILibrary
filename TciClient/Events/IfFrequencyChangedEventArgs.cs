// Copyright (c) 2019 Vinod ES (VU3ESV)
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

namespace ExpertElectronics.Tci.Events;

public class IfFrequencyChangedEventArgs : EventArgs
{
    public IfFrequencyChangedEventArgs(uint transceiverPeriodicNumber, uint channel, double value)
    {
        TransceiverPeriodicNumber = transceiverPeriodicNumber;
        Channel = channel;
        Value = value;
    }

    public uint TransceiverPeriodicNumber { get; }

    public uint Channel { get; }

    public double Value { get; }
}