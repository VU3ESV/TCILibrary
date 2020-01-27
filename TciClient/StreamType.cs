namespace ExpertElectronics.Tci
{
    internal enum StreamType
    {
        /// <summary>
        /// Receiver's IQ Signal Stream
        /// </summary>
        IqStream = 0,

        /// <summary>
        /// Receiver's Audio Signal Stream
        /// </summary>
        RxAudioStream,

        /// <summary>
        /// Transceiver's Audio Signal Stream
        /// </summary>
        TxAudioStream,

        /// <summary>
        /// Stream of temporary Markers to transmit audio signal
        /// </summary>
        TxChrono,
    }
}