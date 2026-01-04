using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci
{
    /// <summary>
    /// High-level client for connecting to a TCI server and exposing a transceiver controller.
    /// Use <see cref="Create(string,uint,CancellationToken)"/> to create an instance.
    /// </summary>
    public class TciClient : ITciClient
    {
        /// <summary>
        /// Creates a new <see cref="TciClient"/> instance and validates the server parameters.
        /// </summary>
        /// <param name="serverIpAddress">IP address or hostname of the TCI server (use "localhost" for local).</param>
        /// <param name="serverPort">TCP port of the TCI server.</param>
        /// <param name="cancellationToken">Cancellation token used when creating the underlying websocket client.</param>
        /// <returns>A new <see cref="TciClient"/> connected to the provided endpoint once <see cref="ConnectAsync"/> is called.</returns>
        public static TciClient Create(string serverIpAddress, uint serverPort, CancellationToken cancellationToken)
        {
            // Synchronous convenience wrapper for callers that expect a blocking create.
            return CreateAsync(serverIpAddress, serverPort, cancellationToken).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Creates a <see cref="TciClient"/> asynchronously.
        /// </summary>
        /// <param name="serverIpAddress">IP address or hostname of the TCI server (use "localhost" for local).</param>
        /// <param name="serverPort">TCP port of the TCI server.</param>
        /// <param name="cancellationToken">Cancellation token used when creating the underlying websocket client.</param>
        /// <returns>A task that returns a configured <see cref="TciClient"/> instance.</returns>
        public static async Task<TciClient> CreateAsync(string serverIpAddress, uint serverPort, CancellationToken cancellationToken)
        {
            Debug.Assert(!string.IsNullOrEmpty(serverIpAddress));
            Debug.Assert(serverPort != 0);
            if (serverIpAddress != "localhost" && !IPAddress.TryParse(serverIpAddress, out var _))
            {
                throw new ArgumentException($"Invalid Format. Parameter- '{serverIpAddress}'");
            }

            var messageHandler = new TciMessageHandler();
            var webSocketClient = await TciWebSocketClient.CreateAsync(serverIpAddress, serverPort, cancellationToken).ConfigureAwait(false);
            var client = new TciClient(messageHandler, webSocketClient);
            return client;
        }

        /// <summary>
        /// Private constructor used by the async factory. Initializes internal components.
        /// </summary>
        /// <param name="messageHandler">Pre-created message handler instance.</param>
        /// <param name="webSocketClient">Pre-created websocket client instance.</param>
        private TciClient(ITciMessageHandler messageHandler, TciWebSocketClient webSocketClient)
        {
            _messageHandler = messageHandler;
            _messageHandler.OnSocketConnectionChanged += MessageHandler_OnSocketConnectionChanged;
            TransceiverController = new TransceiverController(_messageHandler, this);
            _tciWebSocketClient = webSocketClient;
            Initialize();
        }

        private void MessageHandler_OnSocketConnectionChanged(object sender, TciConnectedEventArgs e)
        {
            if (e.TciConnection == true)
            {
                ConnectionStatus = ConnectionStatus.Connected;
                OnConnect?.Invoke(this, new TciConnectedEventArgs(true));
            }
            else if (e.TciConnection == false)
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
                OnDisconnect?.Invoke(this, new TciConnectedEventArgs(false));
            }
            else
            {
                ConnectionStatus = ConnectionStatus.None;
            }
        }

        /// <summary>
        /// Connects to the configured TCI server asynchronously.
        /// </summary>
        /// <returns>A task that completes when the underlying websocket has connected.</returns>
        public async Task ConnectAsync()
        {
            await _tciWebSocketClient.Connect();
        }

        /// <summary>
        /// Disconnects from the TCI server asynchronously.
        /// </summary>
        /// <returns>A task that completes when the underlying websocket has disconnected.</returns>
        public async Task DisConnectAsync()
        {
            await _tciWebSocketClient.Disconnect();
        }

        public ITransceiverController TransceiverController { get; }

        public ConnectionStatus ConnectionStatus
        {
            get; private set;
        }

        /// <summary>
        /// Sends a raw TCI message to the server asynchronously.
        /// </summary>
        /// <param name="message">The raw message to send (TCI command format).</param>
        /// <returns>A task that completes when the message has been transmitted.</returns>
        public async Task SendMessageAsync(string message)
        {
            await _tciWebSocketClient.SendMessage(message);
        }

        private void Initialize()
        {
            _tciWebSocketClient.OnConnect(_messageHandler.OnConnect);
            _tciWebSocketClient.OnDisconnect(_messageHandler.OnDisConnect);
            _tciWebSocketClient.OnMessage(_messageHandler.OnMessage);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (_messageHandler != null)
            {
                _messageHandler.OnSocketConnectionChanged -= MessageHandler_OnSocketConnectionChanged;
            }
        }

        private readonly TciWebSocketClient _tciWebSocketClient;
        private readonly ITciMessageHandler _messageHandler;

        public event EventHandler<TciConnectedEventArgs> OnConnect;
        public event EventHandler<TciConnectedEventArgs> OnDisconnect;
    }
}