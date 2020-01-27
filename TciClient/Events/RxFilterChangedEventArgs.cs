// Copyright (c) 2019 Vinod ES (VU3ESV)
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;

namespace ExpertElectronics.Tci.Events
{
    public class RxFilterChangedEventArgs : EventArgs
    {
        public RxFilterChangedEventArgs(uint transceiverPeriodicNumber, int low, int high)
        {
            TransceiverPeriodicNumber = transceiverPeriodicNumber;
            Low = low;
            High = high;
        }

        public uint TransceiverPeriodicNumber { get; }

        public int Low { get; }

        public int High { get; }
    }
}