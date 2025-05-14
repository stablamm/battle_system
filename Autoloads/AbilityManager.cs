using BattleSystem.BattleEngine.Abilities.Resources;
using Godot;
using System;
using System.Collections.Generic;

namespace BattleSystem.Autoloads
{
    public partial class AbilityManager : Node
    {
        public const string NODE_PATH = "/root/AbilityManager";

        public Dictionary<AbilityResource.AbilityId, AbilityResource> AbilityResources { get; set; } = new();
        public Dictionary<AbilityResource.AbilityId, PackedScene> PackedAbilities { get; set; } = new();

        public override void _Ready()
        {
            LoadAbilityResources();
            LoadPackedAbilities();
        }

        private void LoadAbilityResources()
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
                var ability = GD.Load<AbilityResource>(path);
                if (ability != null)
                {
                    AbilityResources[ability.Id] = ability;
                }
            }
        }

        private void LoadPackedAbilities()
        {
            var basePath = "res://BattleEngine/Abilities/Inherited/{0}.tscn";
            
            PackedAbilities[AbilityResource.AbilityId.Fireball] = GD.Load<PackedScene>(string.Format(basePath, "fireball"));
            PackedAbilities[AbilityResource.AbilityId.IceBolt] = GD.Load<PackedScene>(string.Format(basePath, "ice_bolt"));
            PackedAbilities[AbilityResource.AbilityId.Shield] = GD.Load<PackedScene>(string.Format(basePath, "shield"));
            PackedAbilities[AbilityResource.AbilityId.Barrier] = GD.Load<PackedScene>(string.Format(basePath, "barrier"));
            PackedAbilities[AbilityResource.AbilityId.Heal] = GD.Load<PackedScene>(string.Format(basePath, "heal"));
            PackedAbilities[AbilityResource.AbilityId.Buff] = GD.Load<PackedScene>(string.Format(basePath, "buff"));
        }
    }
}
