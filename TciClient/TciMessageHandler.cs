using System;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci
{
    public class TciMessageHandler : ITciMessageHandler
    {
        public void OnConnect(TciWebSocketClient tciWebSocketClient)
        {
            _tciWebSocketClient = tciWebSocketClient;
             OnSocketConnectionChanged?.Invoke(this, new TciConnectedEventArgs(true));
        }

        public void OnDisConnect(TciWebSocketClient tciWebSocketClient)
        {
            if (_tciWebSocketClient != tciWebSocketClient)
            {
                return;
            }
            OnSocketConnectionChanged?.Invoke(this, new TciConnectedEventArgs(false));
        }

        public void OnMessage(string message, TciWebSocketClient tciWebSocketClient)
        {
            if (_tciWebSocketClient != tciWebSocketClient)
            {
                return;
            }           
            OnSocketMessageReceived?.Invoke(this, new TciMessageReceivedEventArgs(message: message));
        }

        public event EventHandler<TciMessageReceivedEventArgs> OnSocketMessageReceived;
        public event EventHandler<TciConnectedEventArgs> OnSocketConnectionChanged;

        private TciWebSocketClient _tciWebSocketClient;        
    }
}
