using BattleSystem.BattleEngine.Battlers;
using Godot;
using Godot.Collections;

namespace BattleSystem.Autoloads
{
    public partial class BattleManager : Node
    {
        public const string NODE_PATH = "/root/BattleManager";

        public enum BattleType
        {
            ATTACK
        }

        public enum BattleState
        {
            IDLE,
            PLAYER_ONE_ATTACK,
            PLAYER_TWO_ATTACK,
            GAME_OVER
        }

        public readonly int MAX_BATTLERS = 2; // Increase/Decrease this value when battlers are added/removed. This will be referenced in other parts of the code.
        public enum Battlers
        {
            CHASMIRE,
            DREXAL
        }

        public Dictionary<Battlers, PackedScene> PackedBattlers { get; private set; } = new(); // Resources for spawning battlers.
        public Dictionary<long, Battlers> SelectedBattlers { get; private set; } = new(); // Battlers selected for battle.
        public BattleState CurrentState { get; private set; } = BattleState.IDLE; // Current state of the battle.
        public BattleType TypeOfBattle { get; private set; } = BattleType.ATTACK;

        public override void _Ready()
        {
            PreloadBattlers();
        }

        public void PreloadBattlers()
        {
            GD.Print("Preloading battlers...");

            PackedBattlers.Add(Battlers.CHASMIRE, GD.Load<PackedScene>("res://BattleEngine/Battlers/Inherited/chasmire_battler.tscn"));
            PackedBattlers.Add(Battlers.DREXAL, GD.Load<PackedScene>("res://BattleEngine/Battlers/Inherited/drexal_battler.tscn"));

            GD.Print("Battlers preloaded.");
        }

        public void SelectBattler(long id, Battlers battlerType)
        {
            SelectedBattlers[id] = battlerType;
        }

        public void StartGame()
        {
            if (!Multiplayer.IsServer()) { return; }

            GD.Print("Starting game...");

            Rpc(nameof(SyncState), (int)BattleState.PLAYER_ONE_ATTACK); // Start with player one attack state.
        }

        public void UpdateState(BattleState state)
        {
            if (!Multiplayer.IsServer()) { return; }

            GD.Print("Updating state...");
            Rpc(nameof(SyncState), (int)state);
        }

        [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
        private void SyncState(BattleState newState)
        {
            CurrentState = newState;
            AutoloadManager.Instance.SignalM.EmitBattleStateChanged((int)newState);
            GD.Print($"Turn changed to: { newState.ToString() }.");
        }
    }
}
