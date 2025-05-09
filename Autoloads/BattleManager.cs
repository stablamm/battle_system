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
            ATTACKING,
            DEFENDING,
            WIN,
            LOSE
        }

        public readonly int MAX_BATTLERS = 2; // Increase/Decrease this value when battlers are added/removed. This will be referenced in other parts of the code.
        public enum Battlers
        {
            CHASMIRE,
            DREXAL
        }

        public Dictionary<Battlers, PackedScene> PackedBattlers { get; private set; } = new();

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
    }
}
