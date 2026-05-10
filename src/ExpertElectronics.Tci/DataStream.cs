namespace ExpertElectronics.Tci;

public struct DataStream
{
    /// <summary>
    /// receiver's periodic number
    /// </summary>
    public uint Receiver;

    /// <summary>
    /// SampleRate
    /// </summary>
    public uint SampleRate;

    /// <summary>
    /// Always equal to 4 (float 32bit)
    /// </summary>
    public uint Format;

    /// <summary>
    /// Compression Algorithm, not implemented yet, 0 for now
    /// </summary>
    public uint Codec;

    /// <summary>
    /// CheckSum , not implemented Yet, 0 for now
    /// </summary>
    public uint Crc;

    /// <summary>
    /// length of data field
    /// </summary>
    public uint Length;

    /// <summary>
    /// type of data stream
    /// </summary>
    public uint Type;

    /// <summary>
    /// reserved
    /// </summary>
    public uint[] Reserved;

    /// <summary>
    /// data field
    /// </summary>
    public float[] Data;

    /// <summary>
    /// creates a new data stream
    /// </summary>
    /// <param name="receiverId"> identifier of the receiver</param>
    /// <param name="dataLength"> length of data</param>
    public DataStream(uint receiverId, uint dataLength = 4096)
    {
        Reserved = new uint[9];
        Data = new float[dataLength];
        Receiver = receiverId;
        SampleRate = 48000;
        Format = 4;
        Codec = 0;
        Crc = 0;
        Length = 0;
        Type = 0;
    }
}