using Godot;
using System;

namespace BattleSystem.Autoloads
{
    public partial class LogManager : Node
    {
        public const string NODE_PATH = "/root/LogManager";

        public enum LOG_TYPE
        {
            DEBUG,
            INFO,
            ERROR
        }

        public void WriteLog(string message, LOG_TYPE type = LOG_TYPE.INFO)
        {
            string logTimestamp = DateTime.Now.ToString("hh:mm:ss");
            string logMessage = $"[{logTimestamp}] [{type}] [{(Multiplayer.IsServer() ? "SERVER" : "CLIENT")}] {message}";
            string emitMessage = $"[{logTimestamp}] [{type}] {message}";

            switch (type)
            {
                case LOG_TYPE.DEBUG:
                    GD.Print(logMessage);
                    AutoloadManager.Instance.SignalM.EmitDebugLogMessage(emitMessage);
                    break;
                case LOG_TYPE.INFO:
                    GD.Print(logMessage);
                    AutoloadManager.Instance.SignalM.EmitInfoLogMessage(emitMessage);
                    break;
                case LOG_TYPE.ERROR:
                    GD.PrintErr(logMessage);
                    AutoloadManager.Instance.SignalM.EmitErrorLogMessage(emitMessage);
                    break;
            }
        }
    }
}