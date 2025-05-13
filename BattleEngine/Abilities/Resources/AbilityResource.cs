using Godot;
using System;

namespace BattleSystem.BattleEngine.Abilities.Resources
{
    [GlobalClass]
    public partial class AbilityResource : Resource
    {
        public enum AbilityId
        {
            Fireball,
            IceBolt,
            Shield,
            Barrier,
            Heal,
            Buff
        }

        public enum AbilityType
        {
            Offense,
            Defense,
            Support
        }

        [Export] public AbilityId Id { get; set; }
        [Export] public string Name { get; set; }
        [Export] public AbilityType Type { get; set; }
        [Export] public int Power { get; set; } // Damage, heal amount, etc.
        [Export] public string Description { get; set; }
    }
}
