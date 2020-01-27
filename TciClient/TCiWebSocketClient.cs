using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExpertElectronics.Tci
{
    public class TciWebSocketClient
    {
        protected TciWebSocketClient(string uri, CancellationToken cancellationToken)
        {
            _clientWebSocket = new ClientWebSocket();
            _clientWebSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(20);
            _uri = new Uri(uri);
            _cancellationToken = cancellationToken;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="uri">The URI of the WebSocket server.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<TciWebSocketClient> CreateAsync(string uri, CancellationToken cancellationToken)
        {
            return Task.FromResult(new TciWebSocketClient(uri, cancellationToken));
        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="ipAddress">IP Address of the TCI Socket Server</param>
        /// <param name="port"> TCI Port</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<TciWebSocketClient> CreateAsync(string ipAddress, uint port, CancellationToken cancellationToken)
        {
            var uri = $"ws://{ipAddress}:{port}";
            return await CreateAsync(uri, cancellationToken);
        }


        /// <summary>
        /// Connects to the WebSocket server.
        /// </summary>
        /// <returns></returns>
        public async Task<TciWebSocketClient> Connect()
        {
            await _clientWebSocket.ConnectAsync(_uri, _cancellationToken);
            CallOnConnected();
            await Task.Factory.StartNew(async () => await StartListen(), cancellationToken: _cancellationToken);
            return this;
        }

        public async Task Disconnect()
        {
            if (_clientWebSocket.State == WebSocketState.Open)
            {
                await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                                                  string.Empty,
                                                  CancellationToken.None);
            }
        }

        /// <summary>
        /// Set the Action to call when the connection has been established.
        /// </summary>
        /// <param name="onConnect">The Action to call.</param>
        /// <returns></returns>
        public TciWebSocketClient OnConnect(Action<TciWebSocketClient> onConnect)
        {
            _onConnected = onConnect;
            return this;
        }

        /// <summary>
        /// Set the Action to call when the connection has been terminated.
        /// </summary>
        /// <param name="onDisconnect">The Action to call</param>
        /// <returns></returns>
        public TciWebSocketClient OnDisconnect(Action<TciWebSocketClient> onDisconnect)
        {
            _onDisconnected = onDisconnect;
            return this;
        }

        /// <summary>
        /// Set the Action to call when a messages has been received.
        /// </summary>
        /// <param name="onMessage">The Action to call.</param>
        /// <returns></returns>
        public TciWebSocketClient OnMessage(Action<string, TciWebSocketClient> onMessage)
        {
            _onMessage = onMessage;
            return this;
        }

        /// <summary>
        /// Send a message to the WebSocket server.
        /// </summary>
        /// <param name="message">The message to send</param>
        public async Task SendMessage(string message)
        {
            if (_clientWebSocket.State != WebSocketState.Open)
            {
                throw new Exception("Connection is not open.");
            }

            var messageBuffer = Encoding.UTF8.GetBytes(message);
            var messagesCount = (int)Math.Ceiling((double)messageBuffer.Length / SendChunkSize);

            for (var i = 0; i < messagesCount; i++)
            {
                var offset = (SendChunkSize * i);
                var count = SendChunkSize;
                var lastMessage = ((i + 1) == messagesCount);

                if ((count * (i + 1)) > messageBuffer.Length)
                {
                    count = messageBuffer.Length - offset;
                }

                await _clientWebSocket.SendAsync(new ArraySegment<byte>(messageBuffer, offset, count),
                                                 WebSocketMessageType.Text, lastMessage, _cancellationToken);
            }
        }

        private async Task StartListen()
        {
            var buffer = new byte[ReceiveChunkSize];
            try
            {
                while (_clientWebSocket.State == WebSocketState.Open)
                {
                    var stringResult = new StringBuilder();
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationToken);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await
                                _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                                                            string.Empty, CancellationToken.None);
                            CallOnDisconnected();
                        }
                        else
                        {
                            var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            stringResult.Append(str);
                        }
                    } while (!result.EndOfMessage);

                    CallOnMessage(stringResult);
                }
            }
            catch (Exception)
            {
                CallOnDisconnected();
            }
            finally
            {
                _clientWebSocket.Dispose();
            }
        }

        private void CallOnMessage(StringBuilder stringResult)
        {
            //if (_onMessage != null)
            //    ExecuteInTask(() => _onMessage(stringResult.ToString(), this));

            if (_onMessage != null)
                _onMessage(stringResult.ToString(), this);
        }

        private void CallOnDisconnected()
        {
            if (_onDisconnected != null)
                ExecuteInTask(() => _onDisconnected(this));
        }

        private void CallOnConnected()
        {
            if (_onConnected != null)
                ExecuteInTask(() => _onConnected(this));
        }

        private static void ExecuteInTask(Action action)
        {
            Task.Factory.StartNew(action);
        }

        private const int ReceiveChunkSize = 1024;
        private const int SendChunkSize = 1024;

        private readonly ClientWebSocket _clientWebSocket;
        private readonly Uri _uri;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly CancellationToken _cancellationToken;

        private Action<TciWebSocketClient> _onConnected;
        private Action<string, TciWebSocketClient> _onMessage;
        private Action<TciWebSocketClient> _onDisconnected;
    }
}
