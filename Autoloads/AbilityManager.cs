using BattleSystem.BattleEngine.Abilities.Resources;
using Godot;
using System;
using System.Collections.Generic;

namespace BattleSystem.Autoloads
{
    public partial class AbilityManager : Node
    {
        public const string NODE_PATH = "/root/AbilityManager";

        public Dictionary<AbilityResource.AbilityId, AbilityResource> Abilities = new();

        public override void _Ready()
        {
            var basePath = "res://BattleEngine/Abilities/Resources/Inherited/";
            var pathEnd = "Resrc.tres";
            var abilityPaths = new[]
            {
                $"{basePath}Barrier{pathEnd}",
                $"{basePath}Buff{pathEnd}",
                $"{basePath}Fireball{pathEnd}",
                $"{basePath}Heal{pathEnd}",
                $"{basePath}IceBolt{pathEnd}",
                $"{basePath}Shield{pathEnd}",
            };

            foreach (var path in abilityPaths)
            {
                LoadAbility(path);
            }
        }

        private void LoadAbility(string path)
        {
            var ability = GD.Load<AbilityResource>(path);
            if (ability != null)
            {
                Abilities[ability.Id] = ability;
            }
        }
    }
}
