using Godot;

namespace BattleSystem.Autoloads
{
    public partial class SignalManager : Node
    {
        public const string NODE_PATH = "/root/SignalManager";

        [Signal] public delegate void PlayerListUpdated_EventHandler();
        [Signal] public delegate void BattleMenu_DisplayStartButton_EventHandler();
        [Signal] public delegate void Battle_StateChanged_EventHandler(int newState);
        [Signal] public delegate void Ability_Selected_EventHandler(int abilityId);

        [Signal] public delegate void LogMessage_Debug_EventHandler(string message);
        [Signal] public delegate void LogMessage_Info_EventHandler(string message);
        [Signal] public delegate void LogMessage_Error_EventHandler(string message);

        public override void _Ready()
        {
            AddUserSignal(nameof(PlayerListUpdated_EventHandler));
            AddUserSignal(nameof(BattleMenu_DisplayStartButton_EventHandler));
            AddUserSignal(nameof(Battle_StateChanged_EventHandler));
            AddUserSignal(nameof(Ability_Selected_EventHandler));

            AddUserSignal(nameof(LogMessage_Debug_EventHandler));
            AddUserSignal(nameof(LogMessage_Info_EventHandler));
            AddUserSignal(nameof(LogMessage_Error_EventHandler));
        }

        public void EmitPlayerListUpdated() => EmitSignal(nameof(PlayerListUpdated_EventHandler));
        public void EmitBattleMenuDisplayStartButton() => EmitSignal(nameof(BattleMenu_DisplayStartButton_EventHandler));
        public void EmitBattleStateChanged(int newState) => EmitSignal(nameof(Battle_StateChanged_EventHandler), newState);
        public void EmitAbilitySelected(int abilityId) => EmitSignal(nameof(Ability_Selected_EventHandler), abilityId);

        public void EmitDebugLogMessage(string message) => EmitSignal(nameof(LogMessage_Debug_EventHandler), message);
        public void EmitInfoLogMessage(string message) => EmitSignal(nameof(LogMessage_Info_EventHandler), message);
        public void EmitErrorLogMessage(string message) => EmitSignal(nameof(LogMessage_Error_EventHandler), message);
    }
}