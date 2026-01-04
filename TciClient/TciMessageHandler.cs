using System;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci
{
    /// <summary>
    /// Handles socket-level events and exposes them via typed events to the rest of the library.
    /// </summary>
    public class TciMessageHandler : ITciMessageHandler
    {
        /// <summary>
        /// Called when the websocket client has established a connection.
        /// </summary>
        /// <param name="tciWebSocketClient">The websocket client instance that raised the event.</param>
        public void OnConnect(TciWebSocketClient tciWebSocketClient)
        {
            _tciWebSocketClient = tciWebSocketClient;
            OnSocketConnectionChanged?.Invoke(this, new TciConnectedEventArgs(true));
        }

        /// <summary>
        /// Called when the websocket client has disconnected.
        /// </summary>
        /// <param name="tciWebSocketClient">The websocket client instance that raised the event.</param>
        public void OnDisConnect(TciWebSocketClient tciWebSocketClient)
        {
            if (_tciWebSocketClient != tciWebSocketClient)
            {
                return;
            }
            OnSocketConnectionChanged?.Invoke(this, new TciConnectedEventArgs(false));
        }

        /// <summary>
        /// Called when a message is received from the websocket.
        /// </summary>
        /// <param name="message">The raw message text received.</param>
        /// <param name="tciWebSocketClient">The websocket client instance that received the message.</param>
        public void OnMessage(string message, TciWebSocketClient tciWebSocketClient)
        {
            if (_tciWebSocketClient != tciWebSocketClient)
            {
                return;
            }
            OnSocketMessageReceived?.Invoke(this, new TciMessageReceivedEventArgs(message: message));
        }

        /// <summary>
        /// Raised when a message has been received and validated for this client instance.
        /// </summary>
        public event EventHandler<TciMessageReceivedEventArgs> OnSocketMessageReceived;

        /// <summary>
        /// Raised when the underlying socket connection state changes.
        /// </summary>
        public event EventHandler<TciConnectedEventArgs> OnSocketConnectionChanged;

        private TciWebSocketClient _tciWebSocketClient;
    }
}
