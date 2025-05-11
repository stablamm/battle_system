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
            string logMessage = $"[{DateTime.Now.ToString("hh:mm:ss")}] [{type}] [{(Multiplayer.IsServer() ? "SERVER" : "CLIENT")}] {message}";

            switch (type)
            {
                case LOG_TYPE.DEBUG:
                    GD.Print(logMessage);
                    break;
                case LOG_TYPE.INFO:
                    GD.Print(logMessage);
                    break;
                case LOG_TYPE.ERROR:
                    GD.PrintErr(logMessage);
                    break;
            }
        }
    }
}