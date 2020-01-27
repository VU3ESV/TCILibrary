using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Events;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciSqlLevelCommand : ITciCommand, IDisposable
    {
        public TciSqlLevelCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnSqlLevelChanged += TransceiverControllerOnSqlLevelChanged;
        }

        private void TransceiverControllerOnSqlLevelChanged(object sender, TrxIntValueChangedEventArgs e)
        {
            var transceiverPeriodicNumber = e.TransceiverPeriodicNumber;
            var sqlLevel = e.Value;
            if (sqlLevel >= -140 && sqlLevel <= 0)
            {
                _transceiverController.TciClient.SendMessageAsync($"{Name}:{transceiverPeriodicNumber},{sqlLevel};");
            }
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

            _transceiverController.SquelchLevel(transceiverPeriodicNumber, sqlLevel);
            return true;
        }


        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnSqlLevelChanged -= TransceiverControllerOnSqlLevelChanged;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int TransceiverIndex = 1;
        private const int SqlLevelIndex = 2;
        private const int CommandParameterCount = 4;
    }
}
