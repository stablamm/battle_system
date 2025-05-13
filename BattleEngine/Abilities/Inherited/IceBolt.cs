using BattleSystem.Autoloads;
using BattleSystem.BattleEngine.Battlers;

namespace BattleSystem.BattleEngine.Abilities.Inherited
{
    public partial class IceBolt : BaseAbility
    {
        public override void ExecuteAbility(BaseBattler target)
        {
            base.ExecuteAbility(target);

            // Apply damage to the target
            target.TakeDamage(Resource.Power);

            // Log the ability execution
            AutoloadManager.Instance.LogM.WriteLog($"IceBolt ability executed on {target.Name}");
        }
    }
}