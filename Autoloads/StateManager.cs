using BattleSystem.BattleEngine.Abilities.Resources;
using Godot;
using System.Collections.Generic;

namespace BattleSystem.Autoloads;

public partial class StateManager : Node
{
    public const string NODE_PATH = "/root/StateManager";

    #region Ability
    public Dictionary<long, List<AbilityResource.AbilityId>> SelectedAbilities { get; private set; } = new(); // Abilities selected for each battler.

    public void RequestAddAbility(long battlerId, int abilityId)
    {
        Rpc(nameof(AddAbility), battlerId, abilityId);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void AddAbility(long battlerId, int abilityId)
    {
        if (!SelectedAbilities.ContainsKey(battlerId))
        {
            SelectedAbilities[battlerId] = new List<AbilityResource.AbilityId>();
        }

        SelectedAbilities[battlerId].Add((AbilityResource.AbilityId)abilityId);
    }

    public void RequestRemoveAbility(long battlerId, int abilityId)
    {
        Rpc(nameof(RemoveAbility), battlerId, abilityId);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    public void RemoveAbility(long battlerId, int abilityId)
    {
        if (SelectedAbilities.ContainsKey(battlerId))
        {
            SelectedAbilities[battlerId].Remove((AbilityResource.AbilityId)abilityId);
        }
    } 
    #endregion
}
