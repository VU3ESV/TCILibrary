using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci
{
    public class TciClient : ITciClient
    {
        public static TciClient Create(string serverIpAddress, uint serverPort, CancellationToken cancellationToken)
        {
            Debug.Assert(!string.IsNullOrEmpty(serverIpAddress));
            Debug.Assert(serverPort != 0);
            if (serverIpAddress != "localhost" && !IPAddress.TryParse(serverIpAddress, out var _))
            {
                throw new ArgumentException($"Invalid Format. Parameter- '{serverIpAddress}'");
            }

            return new TciClient(serverIpAddress, serverPort, cancellationToken);
        }

        private TciClient(string serverIpAddress, uint serverPort, CancellationToken cancellationToken)
        {
            _messageHandler = new TciMessageHandler();
            _messageHandler.OnSocketConnectionChanged += MessageHandler_OnSocketConnectionChanged;
            TransceiverController = new TransceiverController(_messageHandler, this);
            _tciWebSocketClient = TciWebSocketClient.CreateAsync(serverIpAddress, serverPort, cancellationToken).Result;
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

        public async Task ConnectAsync()
        {
            await _tciWebSocketClient.Connect();
        }

        public async Task DisConnectAsync()
        {
            await _tciWebSocketClient.Disconnect();
        }

        public ITransceiverController TransceiverController { get; }

        public ConnectionStatus ConnectionStatus
        {
            get; private set;
        }

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
            if (disposing)
            {
                return;
            }

            _messageHandler.OnSocketConnectionChanged -= MessageHandler_OnSocketConnectionChanged;
        }

        private readonly TciWebSocketClient _tciWebSocketClient;
        private readonly ITciMessageHandler _messageHandler;

        public event EventHandler<TciConnectedEventArgs> OnConnect;
        public event EventHandler<TciConnectedEventArgs> OnDisconnect;
    }
}