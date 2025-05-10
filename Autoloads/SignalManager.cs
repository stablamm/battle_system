using Godot;

namespace BattleSystem.Autoloads
{
    public partial class SignalManager : Node
    {
        public const string NODE_PATH = "/root/SignalManager";

        [Signal]
        public delegate void PlayerListUpdated_EventHandler();

        [Signal]
        public delegate void BattleMenu_DisplayStartButton_EventHandler();

        [Signal]
        public delegate void Battle_StateChanged_EventHandler(int newState);

        public override void _Ready()
        {
            AddUserSignal(nameof(PlayerListUpdated_EventHandler));
            AddUserSignal(nameof(BattleMenu_DisplayStartButton_EventHandler));
            AddUserSignal(nameof(Battle_StateChanged_EventHandler));
        }

        public void EmitPlayerListUpdated() => EmitSignal(nameof(PlayerListUpdated_EventHandler));
        public void EmitBattleMenuDisplayStartButton() => EmitSignal(nameof(BattleMenu_DisplayStartButton_EventHandler));
        public void EmitBattleStateChanged(int newState) => EmitSignal(nameof(Battle_StateChanged_EventHandler), newState);
    }
}