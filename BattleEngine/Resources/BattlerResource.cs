using Godot;
using System;

namespace BattleSystem.BattleEngine.Resources
{
    [GlobalClass]
    public partial class BattlerResource : Resource
    {
        [Export]
        public string Name { get; set; }

        [Export]
        public string Description { get; set; }

        [Export]
        public Texture2D SpriteTexture { get; set; }
    }
}
