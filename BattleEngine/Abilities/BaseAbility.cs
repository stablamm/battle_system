using BattleSystem.Autoloads;
using BattleSystem.BattleEngine.Abilities.Resources;
using BattleSystem.BattleEngine.Battlers;
using Godot;

namespace BattleSystem.BattleEngine.Abilities
{
    public partial class BaseAbility : Node2D
    {
        [Export]
        public AbilityResource Resource { get; set; }

        public virtual void ExecuteAbility(BaseBattler self, BaseBattler target)
        {
            if (Resource == null)
            {
                AutoloadManager.Instance.LogM.WriteLog("Ability missing resource. Please assign an AbilityResource to the BaseAbility instance.", LogManager.LOG_TYPE.ERROR);
                return;
            }
        }
    }
}
