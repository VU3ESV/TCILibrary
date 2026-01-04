// Copyright (c) 2019 Vinod ES (VU3ESV)
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

namespace ExpertElectronics.Tci.Events;

public class TrxStringValueChangedEventArgs : EventArgs
{
    public TrxStringValueChangedEventArgs(uint transceiverPeriodicNumber, string value)
    {
        TransceiverPeriodicNumber = transceiverPeriodicNumber;
        Value = value;
    }

    public uint TransceiverPeriodicNumber { get; }

    public string Value { get; }
}