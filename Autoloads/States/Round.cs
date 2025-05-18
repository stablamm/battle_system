using Godot;
using System.Collections.Generic;
using System.Linq;

namespace BattleSystem.Autoloads.States
{
    public partial class Round : Node
    {
        public enum RoundState
        {
            PREROUND,
            PREPARE,
            ATTACK,
            POSTROUND
        }

        public RoundState CurrentRoundState { get; private set; } = RoundState.PREPARE;
        public int CurrentRound { get; private set; } = 0;
        public long ActivePlayerId => TurnOrder[CurrentTurnIndex];

        private List<long> TurnOrder = new();
        private int CurrentTurnIndex = 0;

        public void RequestSetupTurnOrder()
        {
            if (!Multiplayer.IsServer()) { return; }

            TurnOrder.Clear();

            foreach (var p in AutoloadManager.Instance.NetworkM.ConnectedPeers)
            {
                TurnOrder.Add(p.Key);
            }

            Rpc(nameof(SyncTurnOrder), new Godot.Collections.Array<long>(TurnOrder));
            AutoloadManager.Instance.LogM.WriteLog("Turn order setup complete.");
        }

        public void RequestRoundChange()
        {
            if (!Multiplayer.IsServer()) { return; }

            CurrentRound++;
            UpdateRound(CurrentRound);
        }

        public void RequestRoundStateChange(RoundState state)
        {
            if (!Multiplayer.IsServer()) { return; }

            CurrentRoundState = state;
            Rpc(nameof(SyncRoundState), (int)state);
        }

        public void UpdateRound(int round)
        {
            AutoloadManager.Instance.LogM.WriteLog("Updating round...");
            Rpc(nameof(SyncRound), round);
        }

        public void ProgressRound()
        {
            if (CurrentRoundState == RoundState.PREROUND)
            {
                RequestRoundStateChange(RoundState.PREPARE);
            }
            else if (CurrentRoundState == RoundState.PREPARE)
            {
                RequestRoundStateChange(RoundState.ATTACK);
            }
            else if (CurrentRoundState == RoundState.ATTACK)
            {
                if (CurrentTurnIndex >= TurnOrder.Count)
                {
                    RequestRoundStateChange(RoundState.POSTROUND);
                }
            }
            else if (CurrentRoundState == RoundState.POSTROUND)
            {

            }
        }

        public void RequestRoundSync()
        {
            if (!Multiplayer.IsServer()) { return; }
            
            AutoloadManager.Instance.LogM.WriteLog("Syncing round...");
            
            Rpc(nameof(SyncRoundState), (int)CurrentRoundState);
            Rpc(nameof(SyncRound), CurrentRound);
            Rpc(nameof(SyncTurnOrder), new Godot.Collections.Array<long>(TurnOrder));
            
            AutoloadManager.Instance.LogM.WriteLog($"Round synced: {CurrentRound}.");
        }

        [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
        private void SyncRoundState(RoundState newState)
        {
            CurrentRoundState = newState;
            AutoloadManager.Instance.LogM.WriteLog($"Round state changed to: {newState.ToString()}.");
            AutoloadManager.Instance.SignalM.EmitRoundStateChanged((int)newState);
        }

        [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
        private void SyncRound(int round)
        {
            CurrentRound = round;
            AutoloadManager.Instance.LogM.WriteLog($"Round synced to: {CurrentRound}.");
        }

        [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
        private void SyncTurnOrder(Godot.Collections.Array<long> turnOrder)
        {
            CurrentTurnIndex = 0;
            TurnOrder = turnOrder.ToList();
            AutoloadManager.Instance.LogM.WriteLog($"Turn order synced: {string.Join(", ", TurnOrder)}.");
        }
    }
}
