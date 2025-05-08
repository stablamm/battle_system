using BattleSystem.Autoloads;
using Godot;
using System;

namespace BattleSystem.Scenes.MultiplayerObjects
{
    public partial class MultiplayerChat : Node2D
    {
        private RichTextLabel Chatbox;
        private LineEdit ChatMessage;

        public override void _Ready()
        {
            Chatbox = GetNode<RichTextLabel>("%Chatbox");
            ChatMessage = GetNode<LineEdit>("%ChatMessage");
        }

        public void OnTextSubmitted(String new_text)
        {
            RequestSendMessage(new_text);
        }

        public void RequestSendMessage(string message)
        {
            ChatMessage.Text = string.Empty;
            string sender = AutoloadManager.Instance.NetworkM.GetConnectedPeers()[Multiplayer.GetUniqueId()];
            UpdateChatbox(sender, message); // Local update
            Rpc(nameof(UpdateChatbox), sender, message); // Remote update
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
        public void UpdateChatbox(string sender, string message)
        {
            Chatbox.AppendText($"[color=white]{sender}: {message}[/color]\n");
        }
    }
}