using System.Threading.Tasks;

namespace ExpertElectronics.Tci.Interfaces
{
    public interface ITciClient
    {
        Task ConnectAsync();

        Task DisConnectAsync();

        ITransceiverController TransceiverController { get; }

        Task SendMessageAsync(string message);
    }
}