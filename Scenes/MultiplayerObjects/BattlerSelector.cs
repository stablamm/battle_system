using BattleSystem.Autoloads;
using BattleSystem.BattleEngine.Battlers;
using Godot;
using System;

namespace BattleSystem.Scenes.MultiplayerObjects
{
    public partial class BattlerSelector : Node2D
    {
        public long NodeOwnerId { get; set; } = -1; // -1 means no owner, 0 means server, 1 means client

        private int BattlerIndex = 0;
        private BaseBattler SelectedBattler = null;

        private Label BattlerNameLabel;
        private RichTextLabel BattlerDescription;
        private Marker2D Marker;
        private Button LeftButton;
        private Button RightButton;
        private Button SelectButton;

        public override void _Ready()
        {
            BattlerNameLabel = GetNode<Label>("BattlerNameLabel");
            BattlerDescription = GetNode<RichTextLabel>("BattlerDescription");
            Marker = GetNode<Marker2D>("Marker");
            LeftButton = GetNode<Button>("%LeftButton");
            RightButton = GetNode<Button>("%RightButton");
            SelectButton = GetNode<Button>("%SelectButton");
            
            RequestNewBattler(BattlerIndex);

            if (!CanPerformAction())
            {
                LeftButton.Visible = false;
                RightButton.Visible = false;
                SelectButton.Visible = false;
            }
        }

        private void OnLeftButtonPressed()
        {
            if (!CanPerformAction()) return;

            BattlerIndex--;
            if (BattlerIndex < 0)
            {
                BattlerIndex = AutoloadManager.Instance.BattleM.MAX_BATTLERS - 1; // -1 because 0-based indexing
            }

            RequestNewBattler(BattlerIndex);
        }

        private void OnRightButtonPressed()
        {
            if (!CanPerformAction()) return;

            BattlerIndex++;
            if (BattlerIndex >= AutoloadManager.Instance.BattleM.MAX_BATTLERS)
            {
                BattlerIndex = 0;
            }

            RequestNewBattler(BattlerIndex);
        }

        private void OnSelectButtonPressed()
        {

        }

        public bool CanPerformAction() => NodeOwnerId == Multiplayer.GetUniqueId(); // Check if the current node is owned by the player

        public void RequestNewBattler(int new_index)
        {
            Rpc(nameof(UpdateBattler), new_index); // Remote update
            UpdateBattler(new_index); // Local update
        }

        public void RequestSync(long owner)
        {
            Rpc(nameof(SyncBattlerSelection), owner); // Remote update
            SyncBattlerSelection(owner); // Local update
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
        public void UpdateBattler(int new_index)
        {
            BattlerIndex = new_index;

            if (SelectedBattler != null)
            {
                SelectedBattler.QueueFree();
                SelectedBattler = null;
            }

            PackedScene packedBattler = AutoloadManager.Instance.BattleM.PackedBattlers[(BattleManager.Battlers)BattlerIndex];
            SelectedBattler = packedBattler.Instantiate() as BaseBattler;
            SelectedBattler.Position = Marker.Position;
            AddChild(SelectedBattler);
            
            BattlerNameLabel.Text = SelectedBattler.Resource.Name;
            BattlerDescription.Clear();
            BattlerDescription.AppendText($"{SelectedBattler.Resource.Description}");
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
        public void SyncBattlerSelection(long owner)
        {
            NodeOwnerId = owner;

            if (!CanPerformAction())
            {
                LeftButton.Visible   = false;
                RightButton.Visible  = false;
                SelectButton.Visible = false;
            }
            else
            {
                LeftButton.Visible   = true;
                RightButton.Visible  = true;
                SelectButton.Visible = true;
            }
        }
    }
}