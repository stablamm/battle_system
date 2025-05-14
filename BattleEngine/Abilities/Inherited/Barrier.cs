using BattleSystem.Autoloads;
using BattleSystem.BattleEngine.Battlers;

namespace BattleSystem.BattleEngine.Abilities.Inherited
{
    public partial class Barrier : BaseAbility
    {
        public override void ExecuteAbility(BaseBattler self, BaseBattler target)
        {
            base.ExecuteAbility(self, target);

            self.ApplyDefenseBuff(Resource.Power);

            AutoloadManager.Instance.LogM.WriteLog($"Barrier ability executed on {self.Name}");
        }
    }
}