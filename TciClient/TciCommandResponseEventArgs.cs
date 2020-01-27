using System;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci
{
    public class TciCommandResponseEventArgs : EventArgs
    {
        public TciCommandResponseEventArgs(ITciCommandResponse tciCommandResponse)
        {
            TciCommandResponse = tciCommandResponse;
        }

        public ITciCommandResponse TciCommandResponse { get; }
    }
}