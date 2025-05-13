using BattleSystem.Autoloads;
using BattleSystem.BattleEngine.Battlers;

namespace BattleSystem.BattleEngine.Abilities.Inherited
{
    public partial class Shield : BaseAbility
    {
        public override void ExecuteAbility(BaseBattler target)
        {
            base.ExecuteAbility(target);

            // Apply a shield to the target
            target.ApplyShield(Resource.Power);

            // Log the ability execution
            AutoloadManager.Instance.LogM.WriteLog($"Shield ability executed on {target.Name}");
        }
    }
}