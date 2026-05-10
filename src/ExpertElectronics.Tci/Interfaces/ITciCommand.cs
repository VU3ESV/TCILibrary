namespace ExpertElectronics.Tci.Interfaces;

public interface ITciCommand
{
    bool ProcessCommandResponses(IEnumerable<string> messages);
}