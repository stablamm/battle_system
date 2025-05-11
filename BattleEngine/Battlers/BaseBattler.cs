using BattleSystem.Autoloads;
using BattleSystem.BattleEngine.Resources;
using Godot;

namespace BattleSystem.BattleEngine.Battlers
{
    public partial class BaseBattler : Node2D
    {
        [Export]
        public BattlerResource Resource { get; set; }

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
    }
}