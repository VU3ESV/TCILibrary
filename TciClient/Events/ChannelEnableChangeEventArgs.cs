// Copyright (c) 2019 Vinod ES (VU3ESV)
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;

namespace ExpertElectronics.Tci.Events
{
    public class ChannelEnableChangeEventArgs : EventArgs
    {
        public ChannelEnableChangeEventArgs(uint transceiverPeriodicNumber, uint channel, bool state)
        {
            TransceiverPeriodicNumber = transceiverPeriodicNumber;
            Channel = channel;
            State = state;
        }

        public uint TransceiverPeriodicNumber { get; }

        public uint Channel { get; }

        public bool State { get; }
    }
}