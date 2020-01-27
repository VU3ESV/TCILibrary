// Copyright (c) 2019 Vinod ES (VU3ESV)
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;

namespace ExpertElectronics.Tci.Events
{
    public class StringValueChangedEventArgs : EventArgs
    {
        public StringValueChangedEventArgs(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}