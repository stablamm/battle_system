using BattleSystem.Autoloads;
using BattleSystem.BattleEngine.Battlers;
using Godot;

namespace BattleSystem.Scenes.BattleScenes
{
    public partial class DemoScene : Node2D
    {
        private Marker2D LeftSpawn;
        private Marker2D RightSpawn;
        private Label WaitingLabel;
        private Button AttackButton;

        public override void _Ready()
        {
            LeftSpawn = GetNode<Marker2D>("%LeftSpawn");
            RightSpawn = GetNode<Marker2D>("%RightSpawn");
            WaitingLabel = GetNode<Label>("%WaitingLabel");
            AttackButton = GetNode<Button>("%AttackButton");
            
            AutoloadManager.Instance.SignalM.Connect(
                nameof(SignalManager.Battle_StateChanged_EventHandler)
                , new Callable(this, nameof(OnBattleStateChanged))
            );

            if (!Multiplayer.IsServer()) { return; }
                
            SpawnBattlers();
            AutoloadManager.Instance.BattleM.StartGame();
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
                AttackButton.Visible = true;
            }
            else
            {
                WaitingLabel.Visible = true;
                AttackButton.Visible = false;
            }
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
                    Rpc(nameof(SyncLeftSpawn), (int)v.Value);
                }
                else
                {
                    Rpc(nameof(SyncRightSpawn), (int)v.Value);
                }
            }
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        public void SyncLeftSpawn(BattleManager.Battlers battler) // Server
        {
            BaseBattler b = AutoloadManager.Instance.BattleM.PackedBattlers[battler].Instantiate() as BaseBattler;
            b.Position = LeftSpawn.Position;
            AddChild(b);
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        public void SyncRightSpawn(BattleManager.Battlers battler) // Client/AI
        {
            BaseBattler b = AutoloadManager.Instance.BattleM.PackedBattlers[battler].Instantiate() as BaseBattler;
            b.Position = RightSpawn.Position;
            AddChild(b);
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
    }
}