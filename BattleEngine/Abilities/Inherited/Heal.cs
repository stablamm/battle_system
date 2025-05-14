using BattleSystem.Autoloads;
using BattleSystem.BattleEngine.Battlers;

namespace BattleSystem.BattleEngine.Abilities.Inherited
{
    public partial class Heal : BaseAbility
    {
        public override void ExecuteAbility(BaseBattler self, BaseBattler target)
        {
            base.ExecuteAbility(self, target);

            // Apply healing to the target
            self.Heal(Resource.Power);

            // Log the ability execution
            AutoloadManager.Instance.LogM.WriteLog($"Heal ability executed on {self.Name}");
        }
    }
}