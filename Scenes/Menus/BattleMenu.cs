using BattleSystem.Autoloads;
using BattleSystem.Scenes.MultiplayerObjects;
using Godot;
using System.Collections.Generic;

namespace BattleSystem.Scenes.Menus
{
    public partial class BattleMenu : Node2D
    {
        private BattlerSelector BattlerSelectorLeft; // Server
        private BattlerSelector BattlerSelectorRight; // Client/AI
        private Button StartBattleButton;

        public override void _Ready()
        {
            BattlerSelectorLeft = GetNode<BattlerSelector>("BattlerSelectorLeft");
            BattlerSelectorRight = GetNode<BattlerSelector>("BattlerSelectorRight");
            StartBattleButton = GetNode<Button>("StartBattleButton");

            InitializeBattlerSelectors();
            StartBattleButton.Visible = false;

            AutoloadManager.Instance.SignalM.Connect(
                nameof(SignalManager.BattleMenu_DisplayStartButton_EventHandler)
                , new Callable(this, nameof(RequestDisplayStartBattleButton)));
        }

        public void OnStartBattleButtonPressed()
        {
            if (!Multiplayer.IsServer()) { return; }

            AutoloadManager.Instance.SceneM.RequestSceneChange(SceneManager.SceneType.DemoBattleScene);
        }

        public void InitializeBattlerSelectors()
        {
            if (!Multiplayer.IsServer()) return;

            long leftIndex = -1; 
            long rightIndex = -1;

            foreach (KeyValuePair<long, string> kvp in AutoloadManager.Instance.NetworkM.GetConnectedPeers())
            {
                if (kvp.Key == Multiplayer.GetUniqueId())
                {
                    leftIndex = kvp.Key;
                }
                else
                {
                    rightIndex = kvp.Key;
                }
            }

            Rpc(nameof(SyncBattleSelectors), leftIndex, rightIndex);
        }

        public void RequestDisplayStartBattleButton()
        {
            Rpc(nameof(DisplayStartBattleButton));
        }

        [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
        public void SyncBattleSelectors(long leftIndex, long rightIndex)
        {
            BattlerSelectorLeft.RequestSync(leftIndex);
            BattlerSelectorRight.RequestSync(rightIndex);
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        public void DisplayStartBattleButton()
        {
            if (!Multiplayer.IsServer()) { return; }
        
            StartBattleButton.Visible = true;
        }
    }
}