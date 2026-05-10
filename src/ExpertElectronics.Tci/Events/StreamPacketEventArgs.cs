using ExpertElectronics.Tci.Streaming;

namespace ExpertElectronics.Tci.Events;

public class StreamPacketEventArgs(DataStreamPacket packet) : EventArgs
{
    public DataStreamPacket Packet { get; } = packet;
}
