using BattleSystem.Autoloads;
using BattleSystem.BattleEngine.Battlers;

namespace BattleSystem.BattleEngine.Abilities.Inherited
{
    public partial class Fireball : BaseAbility
    {
        public override void ExecuteAbility(BaseBattler target)
        {
            base.ExecuteAbility(target);

            // Apply damage to the target
            target.TakeDamage(Resource.Power);

            // Log the ability execution
            AutoloadManager.Instance.LogM.WriteLog($"Fireball ability executed on {target.Name}");
        }
    }
}