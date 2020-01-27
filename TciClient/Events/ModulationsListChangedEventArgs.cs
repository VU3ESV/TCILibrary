// Copyright (c) 2019 Vinod ES (VU3ESV)
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;

namespace ExpertElectronics.Tci.Events
{
    public class ModulationsListChangedEventArgs : EventArgs
    {
        public ModulationsListChangedEventArgs(IEnumerable<string> list)
        {
            ModulationsList = list;
        }

        public IEnumerable<string> ModulationsList;
    }
}