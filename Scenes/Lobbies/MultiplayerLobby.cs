using BattleSystem.Autoloads;
using Godot;

namespace BattleSystem.Scenes.Lobbies
{
    public partial class MultiplayerLobby : Node2D
    {
        private Node2D GenerateSection;
        private LineEdit ConnectionString;

        public override void _Ready()
        {
            GenerateSection = GetNode<Node2D>("%GenerateSection");
            ConnectionString = GetNode<LineEdit>("%ConnectionString");

            if (!Multiplayer.IsServer())
            {
                GenerateSection.Hide();
            }
        }

        private void OnGenerateButtonPressed()
        {
            ConnectionString.Text = AutoloadManager.Instance.NetworkM.GetServerConnectionString();
        }

        private void OnCopyButtonPressed()
        {
            DisplayServer.ClipboardSet(ConnectionString.Text);
        }
    }
}