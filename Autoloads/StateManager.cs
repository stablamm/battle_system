using BattleSystem.Autoloads.States;
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

    #region Battle
    public enum BattleState
    {
        PREPARE,
        PLAYER_ONE_ATTACK,
        PLAYER_TWO_ATTACK,
        GAME_OVER
    }

    public BattleState CurrentState { get; private set; } = BattleState.PREPARE; // Current state of the battle.

    public void UpdateState(BattleState state)
    {
        if (!Multiplayer.IsServer()) { return; }

        AutoloadManager.Instance.LogM.WriteLog("Updating state...");
        Rpc(nameof(SyncState), (int)state);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    private void SyncState(BattleState newState)
    {
        CurrentState = newState;
        AutoloadManager.Instance.LogM.WriteLog($"Turn changed to: {newState.ToString()}.");
        AutoloadManager.Instance.SignalM.EmitBattleStateChanged((int)newState);
    }
    #endregion

    #region Round
    public Round CurrentRound { get; private set; }
    public bool IsMyTurn() => Multiplayer.GetUniqueId() == CurrentRound.ActivePlayerId;

    public void RequestInitializeRound()
    {
        if (CurrentRound == null)
        {
            CurrentRound = new Round();
            AddChild(CurrentRound);
        }

        CurrentRound.RequestSetupTurnOrder();
        CurrentRound.RequestRoundChange();
        CurrentRound.RequestRoundStateChange(Round.RoundState.PREPARE);
    }

    public void RequestRoundStateChange(Round.RoundState state)
    {
        if (CurrentRound == null)
        {
            AutoloadManager.Instance.LogM.WriteLog("CurrentRound is null. Unable to change round state.", LogManager.LOG_TYPE.ERROR);
            return;
        }

        CurrentRound.RequestRoundStateChange(state);
    }

    public void RequestProgressRound()
    {
        if (CurrentRound == null)
        {
            AutoloadManager.Instance.LogM.WriteLog("CurrentRound is null. Unable to progress round.", LogManager.LOG_TYPE.ERROR);
            return;
        }

        CurrentRound.ProgressRound();
    }

    public void RequestSyncRound()
    {
        if (CurrentRound == null)
        {
            CurrentRound = new Round();
        }

        CurrentRound.RequestRoundSync();
    }

    #endregion
}
