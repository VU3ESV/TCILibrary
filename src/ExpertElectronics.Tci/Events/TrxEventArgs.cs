// Copyright (c) 2019 Vinod ES (VU3ESV)
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

namespace ExpertElectronics.Tci.Events;

public class TrxEventArgs(uint transceiverPeriodicNumber, bool state) : EventArgs
{
    public uint TransceiverPeriodicNumber { get; } = transceiverPeriodicNumber;

    public bool State { get; } = state;
}