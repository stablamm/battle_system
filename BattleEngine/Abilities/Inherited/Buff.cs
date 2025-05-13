using BattleSystem.Autoloads;
using BattleSystem.BattleEngine.Battlers;

namespace BattleSystem.BattleEngine.Abilities.Inherited
{
    public partial class Buff : BaseAbility
    {
        public override void ExecuteAbility(BaseBattler target)
        {
            base.ExecuteAbility(target);

            // Apply a buff to the target
            target.ApplyAttackBuff(Resource.Power);

            // Log the ability execution
            AutoloadManager.Instance.LogM.WriteLog($"Buff ability executed on {target.Name}");
        }
    }
}