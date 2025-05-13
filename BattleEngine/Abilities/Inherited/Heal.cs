using BattleSystem.Autoloads;
using BattleSystem.BattleEngine.Battlers;

namespace BattleSystem.BattleEngine.Abilities.Inherited
{
    public partial class Heal : BaseAbility
    {
        public override void ExecuteAbility(BaseBattler target)
        {
            base.ExecuteAbility(target);

            // Apply healing to the target
            target.Heal(Resource.Power);

            // Log the ability execution
            AutoloadManager.Instance.LogM.WriteLog($"Heal ability executed on {target.Name}");
        }
    }
}