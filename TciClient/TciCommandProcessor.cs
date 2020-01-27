using System;
using System.Threading.Tasks;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci
{
    public class TciCommandProcessor
    {
        public static string CommandToMessage(ITciCommand tciCommand)
        {
            return string.Empty;
        }

        public static ITciCommandResponse MessageToCommandResponse(string message)
        {
            Console.WriteLine(message);
            return null;
        }

        public static async Task<bool> VerifyMessage(string message, ITciCommand tciCommand)
        {
            return false;
        }
    }
}