namespace ExpertElectronics.Tci;

/// <summary>
/// Event arguments wrapper for TCI command responses.
/// </summary>
public class TciCommandResponseEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TciCommandResponseEventArgs"/> class.
    /// </summary>
    /// <param name="tciCommandResponse">The command response payload.</param>
    public TciCommandResponseEventArgs(ITciCommandResponse tciCommandResponse) => TciCommandResponse = tciCommandResponse;

    /// <summary>
    /// Gets the wrapped TCI command response.
    /// </summary>
    public ITciCommandResponse TciCommandResponse { get; }
}