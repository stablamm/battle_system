using BattleSystem.Autoloads;
using BattleSystem.BattleEngine.Battlers;

namespace BattleSystem.BattleEngine.Abilities.Inherited
{
    public partial class Buff : BaseAbility
    {
        public override void ExecuteAbility(BaseBattler self, BaseBattler target)
        {
            base.ExecuteAbility(self, target);

            // Apply a buff to the target
            self.ApplyAttackBuff(Resource.Power);

            // Log the ability execution
            AutoloadManager.Instance.LogM.WriteLog($"Buff ability executed on {self.Name}");
        }
    }
}