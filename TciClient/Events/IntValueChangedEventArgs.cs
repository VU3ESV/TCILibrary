// Copyright (c) 2019 Vinod ES (VU3ESV)
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

namespace ExpertElectronics.Tci.Events;

public class IntValueChangedEventArgs : EventArgs
{
    public IntValueChangedEventArgs(int value) => Value = value;

    public int Value { get; }
}