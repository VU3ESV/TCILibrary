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
            var transceiver = _transceiverController.GeTransceiver(transceiverPeriodicNumber);
            if (transceiver != null)
            {
                transceiver.Squelch = sqlEnable;
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
        private const int SqlEnableIndex = 2;
        private const int CommandParameterCount = 4;
    }
}
