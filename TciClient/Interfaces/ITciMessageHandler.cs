using System;

namespace ExpertElectronics.Tci.Interfaces
{
    public interface ITciMessageHandler
    {
        void OnConnect(TciWebSocketClient tciWebSocketClient);
        void OnDisConnect(TciWebSocketClient tciWebSocketClient);
        void OnMessage(string message, TciWebSocketClient tciWebSocketClient);

        event EventHandler<TciMessageReceivedEventArgs> OnSocketMessageReceived;

        event EventHandler<TciConnectedEventArgs> OnSocketConnected;
    }

    public class TciMessageReceivedEventArgs : EventArgs
    {
        public TciMessageReceivedEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    public class TciConnectedEventArgs : EventArgs
    {
        public TciConnectedEventArgs(bool connection)
        {
            TciConnection = connection;
        }

        public bool TciConnection { get; }
    }
}