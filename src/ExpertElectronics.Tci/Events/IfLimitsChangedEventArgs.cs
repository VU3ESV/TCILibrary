// Copyright (c) 2019 Vinod ES (VU3ESV)
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

namespace ExpertElectronics.Tci.Events;

public class IfLimitsChangedEventArgs(long min, long max) : EventArgs
{
    public long Min { get; } = min;

    public long Max { get; } = max;
}