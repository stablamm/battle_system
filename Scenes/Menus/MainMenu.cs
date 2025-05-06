using BattleSystem.Autoloads;
using Godot;

namespace BattleSystem.Scenes.Menus
{
    public partial class MainMenu : Control
    {
        private Button ConnectButton;
        private Button ExitButton;

        public override void _Ready()
        {
            ConnectButton = GetNode<Button>("%ConnectButton");
            ExitButton = GetNode<Button>("%ExitButton");
        }

        private void OnConnectButtonPressed()
        {
            AutoloadManager.Instance.SceneM.RequestSceneChange(SceneManager.SceneType.ServerClientMenu);
        }

        private void OnExitButtonPressed()
        {
            GetTree().Quit();
        }
    }
}
