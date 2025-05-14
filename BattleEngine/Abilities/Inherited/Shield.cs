using BattleSystem.Autoloads;
using BattleSystem.BattleEngine.Battlers;

namespace BattleSystem.BattleEngine.Abilities.Inherited
{
    public partial class Shield : BaseAbility
    {
        public override void ExecuteAbility(BaseBattler self, BaseBattler target)
        {
            base.ExecuteAbility(self, target);

            // Apply a shield to the target
            self.ApplyShield(Resource.Power);

            // Log the ability execution
            AutoloadManager.Instance.LogM.WriteLog($"Shield ability executed on {self.Name}");
        }
    }
}