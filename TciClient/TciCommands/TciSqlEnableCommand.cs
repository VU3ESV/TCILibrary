using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExpertElectronics.Tci.Interfaces;

namespace ExpertElectronics.Tci.TciCommands
{
    public class TciSqlEnableCommand : ITciCommand, IDisposable
    {
        public TciSqlEnableCommand(ITransceiverController transceiverController)
        {
            _transceiverController = transceiverController;
            _transceiverController.OnSqlEnableChanged += TransceiverController_OnsqlEnableChanged;
        }

        private void TransceiverController_OnsqlEnableChanged(object sender, Events.TrxEventArgs e)
        {
            var transceiverPeriodicNumber = e.TransceiverPeriodicNumber;
            var sqlEnableState = e.State;

            if (transceiverPeriodicNumber < _transceiverController.TrxCount)
            {
                _transceiverController.TciClient.SendMessageAsync($"{Name}:{transceiverPeriodicNumber},{sqlEnableState};");
            }
        }

        public static TciSqlEnableCommand Create(ITransceiverController transceiverController)
        {
            Debug.Assert(transceiverController != null);
            return new TciSqlEnableCommand(transceiverController);
        }

        public static string Name => "sql_enable";

        public bool ProcessCommandResponses(IEnumerable<string> messages)
        {
            var enumerable = messages as string[] ?? messages.ToArray();
            if (!enumerable.Any(_ => _.Contains(Name)))
            {
                return false;
            }

            var sqlEnableMessage = enumerable.FirstOrDefault(_ => _.Contains(Name));
            if (string.IsNullOrEmpty(sqlEnableMessage))
            {
                return false;
            }

            var sqlEnableMessageElements = sqlEnableMessage.Split(':', ',', ';');
            if (sqlEnableMessageElements.Length != CommandParameterCount)
            {
                return false;
            }

            var transceiverPeriodicNumber = Convert.ToUInt32(sqlEnableMessageElements[TransceiverIndex]);
            var sqlEnable = Convert.ToBoolean(sqlEnableMessageElements[SqlEnableIndex]);
            _transceiverController.SquelchEnable(transceiverPeriodicNumber, sqlEnable);
            return true;
        }

        public void Dispose()
        {
            if (_transceiverController == null)
            {
                return;
            }

            _transceiverController.OnSqlEnableChanged -= TransceiverController_OnsqlEnableChanged;
            GC.SuppressFinalize(this);
        }

        private readonly ITransceiverController _transceiverController;
        private const int TransceiverIndex = 1;
        private const int SqlEnableIndex = 2;
        private const int CommandParameterCount = 4;
    }
}
