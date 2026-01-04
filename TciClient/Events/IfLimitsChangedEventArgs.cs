// Copyright (c) 2019 Vinod ES (VU3ESV)
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

namespace ExpertElectronics.Tci.Events;

public class IfLimitsChangedEventArgs : EventArgs
{
    public IfLimitsChangedEventArgs(long min, long max)
    {
        Min = min;
        Max = max;
    }

    public long Min { get; }

    public long Max { get; }
}