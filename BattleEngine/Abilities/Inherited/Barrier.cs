using BattleSystem.Autoloads;
using BattleSystem.BattleEngine.Battlers;

namespace BattleSystem.BattleEngine.Abilities.Inherited
{
    public partial class Barrier : BaseAbility
    {
        public override void ExecuteAbility(BaseBattler target)
        {
            base.ExecuteAbility(target);

            target.ApplyDefenseBuff(Resource.Power);

            AutoloadManager.Instance.LogM.WriteLog($"Barrier ability executed on {target.Name}");
        }
    }
}