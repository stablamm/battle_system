using BattleSystem.Autoloads.States;
using BattleSystem.BattleEngine.Battlers;
using Godot;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BattleSystem.Autoloads
{
    public partial class BattleManager : Node
    {
        public const string NODE_PATH = "/root/BattleManager";

        public readonly int MAX_BATTLERS = 2; // Increase/Decrease this value when battlers are added/removed. This will be referenced in other parts of the code.
        public enum Battlers
        {
            CHASMIRE,
            DREXAL
        }

        public Dictionary<Battlers, PackedScene> PackedBattlers { get; private set; } = new(); // Resources for spawning battlers.
        public Dictionary<long, Battlers> SelectedBattlers { get; private set; } = new(); // Battlers selected for battle.
        public Dictionary<long, BattlerStats> PlayerStats { get; private set; } = new();

        public override void _Ready()
        {
            PreloadBattlers();
        }

        public void PreloadBattlers()
        {
            PackedBattlers.Add(Battlers.CHASMIRE, GD.Load<PackedScene>("res://BattleEngine/Battlers/Inherited/chasmire_battler.tscn"));
            PackedBattlers.Add(Battlers.DREXAL, GD.Load<PackedScene>("res://BattleEngine/Battlers/Inherited/drexal_battler.tscn"));
        }

        public void SelectBattler(long id, Battlers battlerType)
        {
            SelectedBattlers[id] = battlerType;
        }

        public void StartGame()
        {
            AutoloadManager.Instance.LogM.WriteLog("Starting game...");
            AutoloadManager.Instance.SignalM.EmitRoundStateChanged((int)Round.RoundState.PREROUND);
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void SyncStats(long id, string statsJson)
        {
            if (!Multiplayer.IsServer()) { return; }

            var stats = JsonConvert.DeserializeObject<BattlerStats>(statsJson);
            PlayerStats[id] = stats;

            AutoloadManager.Instance.LogM.WriteLog($"Player {id} stats synced: {statsJson}");

            Rpc(nameof(RemoteSyncStats), id, statsJson);
        }

        /// <summary>
        /// Syncs the stats of the player with the given ID to the client.
        /// </summary>
        /// <param name="id">Player ID</param>
        /// <param name="statsJson">Jsonified Stats</param>
        [Rpc(MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void RemoteSyncStats(long id, string statsJson)
        {
            var stats = JsonConvert.DeserializeObject<BattlerStats>(statsJson);
            PlayerStats[id] = stats;

            AutoloadManager.Instance.LogM.WriteLog($"Player {id} stats synced: {statsJson}");
        }
    }
}
