using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciSqlLevelCommand : ITciCommand, IDisposable
    {
        public TciSqlLevelCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
        }

        public static TciSqlLevelCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciSqlLevelCommand(transceiverController);
        }

        public static string Name => "sql_level";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var sqlLeverMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(sqlLeverMessage))
            {
                return false;
            }

            var sqlLeverMessageElements = sqlLeverMessage.Split(':', ',', ';');
            if (sqlLeverMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(sqlLeverMessageElements[TransceiverIndex]);
            var sqlLevel = Convert.ToInt32(sqlLeverMessageElements[SqlLevelIndex]);
            if (sqlLevel < -140 && sqlLevel > 0)
            {
                return false;
            }

            var transceiver = _transceiverController.GeTransceiver(transceiverPeriodicNumber);
            if (transceiver != null)
            {
                transceiver.SquelchThreshold = sqlLevel;
            }
            return true;
        }


        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int TransceiverIndex = 1;
        private const int SqlLevelIndex = 2;
        private const int CommandParameterCount = 4;
    }
}
