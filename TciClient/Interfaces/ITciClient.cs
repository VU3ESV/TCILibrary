using System;
using System.Threading.Tasks;

namespace ExpertElectronics.Tci.Interfaces
{
    public interface ITciClient : IDisposable
    {
        Task ConnectAsync();

        Task DisConnectAsync();

        ITransceiverController TransceiverController { get; }

        Task SendMessageAsync(string message);

        event EventHandler<TciConnectedEventArgs> OnConnect;

        event EventHandler<TciConnectedEventArgs> OnDisconnect;

        ConnectionStatus ConnectionStatus { get; }
    }

    public enum ConnectionStatus
    {
        None = 0,
        Connected = 1,
        Disconnected = 2,
    }
}