namespace ExpertElectronics.Tci;

/// <summary>
/// Event arguments wrapper for TCI command responses.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TciCommandResponseEventArgs"/> class.
/// </remarks>
/// <param name="tciCommandResponse">The command response payload.</param>
public class TciCommandResponseEventArgs(ITciCommandResponse tciCommandResponse) : EventArgs
{

    /// <summary>
    /// Gets the wrapped TCI command response.
    /// </summary>
    public ITciCommandResponse TciCommandResponse { get; } = tciCommandResponse;
}