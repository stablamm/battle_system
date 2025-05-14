using BattleSystem.Autoloads;
using BattleSystem.BattleEngine.Battlers.Resources;
using Godot;
using Newtonsoft.Json;

namespace BattleSystem.BattleEngine.Battlers
{
    public partial class BaseBattler : Node2D
    {
        [Export]
        public BattlerResource Resource { get; set; }

        public long BattlerId { get; set; } = -1;

        public BattlerStats Stats { get; set; } = new BattlerStats();

        private Sprite2D Sprite;

        public override void _Ready()
        {
            Sprite = GetNode<Sprite2D>("Sprite");

            if (Resource == null)
            {
                AutoloadManager.Instance.LogM.WriteLog("Battler missing resource. Please assign a BattlerResource to the BaseBattler instance.", LogManager.LOG_TYPE.ERROR);
                return;
            }

            Sprite.Texture = Resource.SpriteTexture;
        }

        public virtual void LoadStats() { }

        public virtual void TakeDamage(int damage)
        {
            if (Stats.Shield > 0)
            {
                Stats.Shield -= damage;
                if (Stats.Shield < 0)
                {
                    Stats.Health += Stats.Shield;
                    Stats.Shield = 0;
                }
            }
            else
            {
                Stats.Health -= damage;
            }

            if (Stats.Health <= 0)
            {
                AutoloadManager.Instance.LogM.WriteLog($"{Resource.Name} has died.", LogManager.LOG_TYPE.INFO);
            }

            Rpc(nameof(SyncStats), BattlerId, JsonConvert.SerializeObject(Stats));
        }

        public virtual void Heal(int amount)
        {
            Stats.Health += amount;
            if (Stats.Health > 100)
            {
                Stats.Health = 100;
            }
            Rpc(nameof(SyncStats), BattlerId, JsonConvert.SerializeObject(Stats));
        }

        public virtual void ApplyShield(int amount)
        {
            Stats.Shield += amount;
            if (Stats.Shield > 100)
            {
                Stats.Shield = 100;
            }
            Rpc(nameof(SyncStats), BattlerId, JsonConvert.SerializeObject(Stats));
        }

        public virtual void ApplyAttackBuff(float amount)
        {
            Stats.Attack = amount;
            Rpc(nameof(SyncStats), BattlerId, JsonConvert.SerializeObject(Stats));
        }

        public virtual void ApplyDefenseBuff(float amount)
        {
            Stats.Defense = amount;
            Rpc(nameof(SyncStats), BattlerId, JsonConvert.SerializeObject(Stats));
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        private void SyncStats(long id, string statsJson)
        {
            var stats = JsonConvert.DeserializeObject<BattlerStats>(statsJson);

            AutoloadManager.Instance.BattleM.PlayerStats[id] = stats;
            AutoloadManager.Instance.LogM.WriteLog($"Player {id} stats synced: {statsJson}");
        }
    }

    public class BattlerStats
    {
        public int Health { get; set; } = 100;
        public int Shield { get; set; } = 0;
        public float Attack { get; set; } = 1;
        public float Defense { get; set; } = 1;
    }
}