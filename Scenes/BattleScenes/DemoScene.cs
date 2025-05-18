using BattleSystem.Autoloads;
using BattleSystem.BattleEngine.Abilities.Resources;
using BattleSystem.BattleEngine.Battlers;
using BattleSystem.Scenes.MultiplayerObjects;
using Godot;
using Newtonsoft.Json;

namespace BattleSystem.Scenes.BattleScenes
{
    public partial class DemoScene : Node2D
    {
        private long _playerOneBattlerId = -1;
        private BaseBattler _playerOneBattler;

        private long _playerTwoBattlerId = -1;
        private BaseBattler _playerTwoBattler;

        private LogContainer LogContainer;
        private BattlerStatsView LeftBattleStats;
        private BattlerStatsView RightBattleStats;
        private AbilitySelector MainAbilitySelector;
        private Marker2D LeftSpawn;
        private Marker2D RightSpawn;
        private Label WaitingLabel;

        public override void _Ready()
        {
            LogContainer = GetNode<LogContainer>("%LogContainer");
            LeftBattleStats = GetNode<BattlerStatsView>("%LeftBattleStats");
            RightBattleStats = GetNode<BattlerStatsView>("%RightBattleStats");
            MainAbilitySelector = GetNode<AbilitySelector>("%MainAbilitySelector");
            LeftSpawn = GetNode<Marker2D>("%LeftSpawn");
            RightSpawn = GetNode<Marker2D>("%RightSpawn");
            WaitingLabel = GetNode<Label>("%WaitingLabel");

            AutoloadManager.Instance.SignalM.Connect(
                nameof(SignalManager.Battle_StateChanged_EventHandler)
                , new Callable(this, nameof(OnBattleStateChanged))
            );

            AutoloadManager.Instance.SignalM.Connect(
                nameof(SignalManager.Ability_Selected_EventHandler)
                , new Callable(this, nameof(OnAbilitySelected))
            );

            MainAbilitySelector.SetBattlerId(Multiplayer.GetUniqueId());

            if (!Multiplayer.IsServer()) { return; }

            SpawnBattlers();
            AutoloadManager.Instance.BattleM.StartGame();
        }

        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("toggle_logs"))
            {
                LogContainer.ToggleLogVisibility();
            }
        }

        private void OnBattleStateChanged(int newState)
        {
            BattleManager.BattleState currentState = (BattleManager.BattleState)newState;
            bool isMyTurn = (currentState == BattleManager.BattleState.PLAYER_ONE_ATTACK && Multiplayer.GetUniqueId() == 1)
                         || (currentState == BattleManager.BattleState.PLAYER_TWO_ATTACK && Multiplayer.GetUniqueId() != 1);

            // Enable/disable ui components based on whose turn it is
            if (isMyTurn)
            {
                WaitingLabel.Visible = false;
                MainAbilitySelector.IsActive = true;
            }
            else
            {
                WaitingLabel.Visible = true;
                MainAbilitySelector.IsActive = false;
            }
        }

        private void OnAbilitySelected(int abilityId)
        {
            Rpc(nameof(RequestUseAbility), abilityId);
        }

        private void OnAttackButtonPressed()
        {
            Rpc(nameof(RequestStateChange));
        }

        public void SpawnBattlers()
        {
            foreach (var v in AutoloadManager.Instance.BattleM.SelectedBattlers)
            {
                if (v.Key == 1)
                {
                    Rpc(nameof(SyncLeftSpawn), v.Key, (int)v.Value);
                }
                else
                {
                    Rpc(nameof(SyncRightSpawn), v.Key, (int)v.Value);
                }
            }
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void SyncLeftSpawn(long id, BattleManager.Battlers battler) // Server
        {
            try
            {
                BaseBattler b = AutoloadManager.Instance.BattleM.PackedBattlers[battler].Instantiate() as BaseBattler;
                b.Position = LeftSpawn.Position;
                b.BattlerId = id;
                AddChild(b);

                string statsJson = JsonConvert.SerializeObject(b.Stats);
                AutoloadManager.Instance.BattleM.Rpc(nameof(BattleManager.SyncStats), id, statsJson);

                LeftBattleStats.SetPlayerId(id);
                _playerOneBattlerId = id;
                _playerOneBattler = b;
            }
            catch (System.Exception ex)
            {
                AutoloadManager.Instance.LogM.WriteLog(ex.Message, LogManager.LOG_TYPE.ERROR);
            }
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        public void SyncRightSpawn(long id, BattleManager.Battlers battler) // Client/AI
        {
            try
            {
                BaseBattler b = AutoloadManager.Instance.BattleM.PackedBattlers[battler].Instantiate() as BaseBattler;
                b.Position = RightSpawn.Position;
                b.BattlerId = id;
                AddChild(b);

                string statsJson = JsonConvert.SerializeObject(b.Stats);
                AutoloadManager.Instance.BattleM.Rpc(nameof(BattleManager.SyncStats), id, statsJson);

                RightBattleStats.SetPlayerId(id);
                _playerTwoBattlerId = id;
                _playerTwoBattler = b;
            }
            catch (System.Exception ex)
            {
                AutoloadManager.Instance.LogM.WriteLog(ex.Message, LogManager.LOG_TYPE.ERROR);
            }
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        public void RequestStateChange()
        {
            if (!Multiplayer.IsServer()) { return; }

            BattleManager.BattleState nextState = AutoloadManager.Instance.BattleM.CurrentState == BattleManager.BattleState.PLAYER_ONE_ATTACK
                ? BattleManager.BattleState.PLAYER_TWO_ATTACK
                : BattleManager.BattleState.PLAYER_ONE_ATTACK;

            AutoloadManager.Instance.BattleM.UpdateState(nextState);
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void RequestUseAbility(int abilityId)
        {
            if (!Multiplayer.IsServer()) { return; }

            var packedAbility = AutoloadManager.Instance.AbilityM.PackedAbilities[(AbilityResource.AbilityId)abilityId];
            var unpackedAbility = (BattleEngine.Abilities.BaseAbility)packedAbility.Instantiate();

            if (AutoloadManager.Instance.BattleM.CurrentState == BattleManager.BattleState.PLAYER_ONE_ATTACK)
            {
                unpackedAbility.ExecuteAbility(_playerOneBattler, _playerTwoBattler);
            }
            else
            {
                unpackedAbility.ExecuteAbility(_playerTwoBattler, _playerOneBattler);
            }

            Rpc(nameof(RequestStateChange));
        }
    }
}