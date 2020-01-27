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
            if (IPAddress.TryParse(serverIpAddress, out var _))
            {
                throw new ArgumentException($"Invalid Format. Parameter- '{serverIpAddress}'");
            }

            return new TciClient(serverIpAddress, serverPort, cancellationToken);
        }

        private TciClient(string serverIpAddress, uint serverPort, CancellationToken cancellationToken)
        {
            _messageHandler = new TciMessageHandler();
            TransceiverController = new TransceiverController(_messageHandler, this);
            _tciWebSocketClient = TciWebSocketClient.CreateAsync(serverIpAddress, serverPort, cancellationToken).Result;
            Initialize();
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

        private readonly TciWebSocketClient _tciWebSocketClient;
        private readonly ITciMessageHandler _messageHandler;
    }
}