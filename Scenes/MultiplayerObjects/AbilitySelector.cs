using BattleSystem.Autoloads;
using Godot;
using System;

namespace BattleSystem.Scenes.MultiplayerObjects
{
    public partial class AbilitySelector : Node2D
    {
        private Tree AbilityTree;

        public override void _Ready()
        {
            AbilityTree = GetNode<Tree>("%AbilityTree");

            AbilityTree.Clear();

            AbilityTree.Columns = 2;
            AbilityTree.ColumnTitlesVisible = false;

            TreeItem root = AbilityTree.CreateItem();
            root.SetText(0, "Abilities");

            TreeItem offense = AbilityTree.CreateItem(root);
            offense.SetText(0, "Offense");

            TreeItem defense = AbilityTree.CreateItem(root);
            defense.SetText(0, "Defense");

            TreeItem support = AbilityTree.CreateItem(root);
            support.SetText(0, "Support");

            foreach (var ability in AutoloadManager.Instance.AbilityM.Abilities)
            {
                AutoloadManager.Instance.LogM.WriteLog($"Ability ID: {ability.Key}, Name: {ability.Value.Name}, Description: {ability.Value.Description}", LogManager.LOG_TYPE.DEBUG);

                TreeItem abilityItem = null;

                if (ability.Value.Type == BattleEngine.Abilities.Resources.AbilityResource.AbilityType.Offense)
                {
                    abilityItem = AbilityTree.CreateItem(offense);
                }
                else if (ability.Value.Type == BattleEngine.Abilities.Resources.AbilityResource.AbilityType.Defense)
                {
                    abilityItem = AbilityTree.CreateItem(defense);
                }
                else
                {
                    abilityItem = AbilityTree.CreateItem(support);
                }

                abilityItem.SetMetadata(0, (int)ability.Key);
                abilityItem.SetText(0, ability.Value.Name);
                abilityItem.SetText(1, ability.Value.Description);
            }
        }
    }
}
