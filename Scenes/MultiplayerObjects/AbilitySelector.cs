using BattleSystem.Autoloads;
using BattleSystem.BattleEngine.Abilities.Resources;
using Godot;

namespace BattleSystem.Scenes.MultiplayerObjects
{
    public partial class AbilitySelector : Node2D
    {
        public bool IsActive { get; set; } = false;
        public long BattlerId { get; set; } = -1;

        private Tree AbilityTree;

        public override void _Ready()
        {
            AbilityTree = GetNode<Tree>("%AbilityTree");

            AbilityTree.Clear();

            AbilityTree.Columns = 2;
            AbilityTree.ColumnTitlesVisible = false;

            TreeItem root = AbilityTree.CreateItem();
            root.SetText(0, "Abilities");
            root.SetMetadata(0, -1);

            TreeItem offense = AbilityTree.CreateItem(root);
            offense.SetText(0, "Offense");
            offense.SetMetadata(0, -1);

            TreeItem defense = AbilityTree.CreateItem(root);
            defense.SetText(0, "Defense");
            defense.SetMetadata(0, -1);

            TreeItem support = AbilityTree.CreateItem(root);
            support.SetText(0, "Support");
            support.SetMetadata(0, -1);

            foreach (var ability in AutoloadManager.Instance.AbilityM.AbilityResources)
            {
                TreeItem abilityItem = null;

                if (ability.Value.Type == AbilityResource.AbilityType.Offense)
                {
                    abilityItem = AbilityTree.CreateItem(offense);
                }
                else if (ability.Value.Type == AbilityResource.AbilityType.Defense)
                {
                    abilityItem = AbilityTree.CreateItem(defense);
                }
                else
                {
                    abilityItem = AbilityTree.CreateItem(support);
                }

                abilityItem.SetMetadata(0, (int)ability.Key);
                abilityItem.SetMetadata(1, (int)ability.Key);
                abilityItem.SetText(0, ability.Value.Name);
                abilityItem.SetText(1, ability.Value.Description);
            }
        }

        private void OnItemSelected()
        {
            if (!IsActive || Multiplayer.GetUniqueId() != BattlerId) 
            { 
                AbilityTree.DeselectAll(); 
                return; 
            }

            TreeItem selectedItem = AbilityTree.GetSelected();
            if (selectedItem != null)
            {
                int abilityId = (int)selectedItem.GetMetadata(0);
                if (abilityId == -1) { return; }
                AutoloadManager.Instance.SignalM.EmitAbilitySelected(abilityId);
                AbilityTree.DeselectAll();
            }
        }
    }
}
