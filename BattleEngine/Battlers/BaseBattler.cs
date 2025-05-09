using BattleSystem.BattleEngine.Resources;
using Godot;
using System;

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
                GD.PrintErr("Battler missing resource. Please assign a BattlerResource to the BaseBattler instance.");
                return;
            }

            Sprite.Texture = Resource.SpriteTexture;
        }
    }
}