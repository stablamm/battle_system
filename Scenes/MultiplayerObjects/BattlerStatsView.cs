using BattleSystem.Autoloads;
using Godot;

namespace BattleSystem.Scenes.MultiplayerObjects
{
    public partial class BattlerStatsView : Node2D
    {
        public long PlayerId { get; private set; } = -1;

        private Label HealthLabel;
        private Label ShieldLabel;
        private Label AttackLabel;
        private Label DefenseLabel;

        public override void _Ready()
        {
            HealthLabel = GetNode<Label>("%HealthLabel");
            ShieldLabel = GetNode<Label>("%ShieldLabel");
            AttackLabel = GetNode<Label>("%AttackLabel");
            DefenseLabel = GetNode<Label>("%DefenseLabel");
        }

        public override void _Process(double delta)
        {
            if (PlayerId == -1) return;
            if (!AutoloadManager.Instance.BattleM.PlayerStats.ContainsKey(PlayerId)) return;
            
            var currentStats = AutoloadManager.Instance.BattleM.PlayerStats[PlayerId];

            HealthLabel.Text = currentStats.Health.ToString();
            ShieldLabel.Text = currentStats.Shield.ToString();
            AttackLabel.Text = currentStats.Attack.ToString();
            DefenseLabel.Text = currentStats.Defense.ToString();
        }

        public void SetPlayerId(long id) => PlayerId = id;
    }
}
