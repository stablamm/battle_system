using BattleSystem.Autoloads;
using Godot;
using System;
using System.Linq;

namespace BattleSystem.Scenes.Menus
{
    public partial class ServerClientMenu : Control
    {
        const int MINIMUM_PASSWORD_LENGTH = 8;

        // Main Container
        private VBoxContainer MainContainer;

        // Server Container
        private VBoxContainer ServerContainer;
        private LineEdit Input_Server_ID;
        private LineEdit Input_Server_IP;
        private LineEdit Input_Server_Port;
        private LineEdit Input_Server_Pass;

        // Client Container
        private VBoxContainer ClientContainer;
        private LineEdit Input_Client_ID;
        private LineEdit Input_Client_IP;
        private LineEdit Input_Client_Port;
        private LineEdit Input_Client_Pass;
        private LineEdit Input_ConnectionString;

        public override void _Ready()
        {
            // Main Container
            MainContainer = GetNode<VBoxContainer>("%MainContainer");

            // Server Container
            ServerContainer = GetNode<VBoxContainer>("%ServerContainer");
            Input_Server_ID = GetNode<LineEdit>("%Input_Server_ID");
            Input_Server_IP = GetNode<LineEdit>("%Input_Server_IP");
            Input_Server_Port = GetNode<LineEdit>("%Input_Server_Port");
            Input_Server_Pass = GetNode<LineEdit>("%Input_Server_Pass");

            // Client Container
            ClientContainer = GetNode<VBoxContainer>("%ClientContainer");
            Input_Client_ID = GetNode<LineEdit>("%Input_Client_ID");
            Input_Client_IP = GetNode<LineEdit>("%Input_Client_IP");
            Input_Client_Port = GetNode<LineEdit>("%Input_Client_Port");
            Input_Client_Pass = GetNode<LineEdit>("%Input_Client_Pass");
            Input_ConnectionString = GetNode<LineEdit>("%Input_ConnectionString");

            MainContainer.Show();
            ServerContainer.Hide();
            ClientContainer.Hide();
        }

        #region Main Container
        private void OnServerButtonPressed()
        {
            MainContainer.Hide();
            ServerContainer.Show();
            ClientContainer.Hide();
        }

        private void OnClientButtonPressed()
        {
            MainContainer.Hide();
            ServerContainer.Hide();
            ClientContainer.Show();
        }

        private void OnBackButtonPressed()
        {
            AutoloadManager.Instance.SceneM.RequestSceneChange(SceneManager.SceneType.MainMenu);
        }
        #endregion

        #region Server Container
        private void OnGetIPButtonPressed()
        {
            string localIp = IP.GetLocalAddresses()
                .Cast<string>()
                .FirstOrDefault(ip => ip.StartsWith("192.") || ip.StartsWith("10.") || ip.StartsWith("172."));

            Input_Server_IP.Text = localIp;
        }

        private void OnOverrideDefaultPortButtonPressed()
        {
            Input_Server_Port.Editable = !Input_Server_Port.Editable;
        }

        private void OnServerStartButtonPressed()
        {
            bool hasUsername = !string.IsNullOrEmpty(Input_Server_ID.Text);
            bool hasIp       = !string.IsNullOrEmpty(Input_Server_IP.Text);
            bool hasPort     = !string.IsNullOrEmpty(Input_Server_Port.Text);
            bool hasPassword = !string.IsNullOrEmpty(Input_Server_Pass.Text);

            if (hasUsername
                && hasIp
                && hasPort
                && hasPassword)
            {
                if (Input_Server_Pass.Text.Length < MINIMUM_PASSWORD_LENGTH)
                {
                    GD.PrintErr($"Password must be at least {MINIMUM_PASSWORD_LENGTH} characters long.");
                    return;
                }

                var res = AutoloadManager.Instance.NetworkM.StartServer(
                    Input_Server_ID.Text,
                    Input_Server_IP.Text,
                    Input_Server_Pass.Text,
                    Convert.ToInt32(Input_Server_Port.Text)
                );

                if (res)
                {
                    GD.Print("Server started successfully.");
                    AutoloadManager.Instance.SceneM.RequestSceneChange(SceneManager.SceneType.MultiplayerLobby);
                }
                else
                {
                    GD.PrintErr("Failed to start server.");
                }
            }
        }

        private void OnServerBackButtonPressed()
        {
            MainContainer.Show();
            ServerContainer.Hide();
        }
        #endregion

        #region Client Container
        private void OnJoinServerButtonPressed()
        {
            bool hasUsername = !string.IsNullOrEmpty(Input_Client_ID.Text);
            bool hasIp = !string.IsNullOrEmpty(Input_Client_IP.Text);
            bool hasPort = !string.IsNullOrEmpty(Input_Client_Port.Text);
            bool hasPassword = !string.IsNullOrEmpty(Input_Client_Pass.Text);
            bool hasConnStr = !string.IsNullOrEmpty(Input_ConnectionString.Text);
            bool connectedToServer = false;

            if (hasConnStr)
            {
                connectedToServer = AutoloadManager.Instance.NetworkM.ConnectToServer(Input_Client_ID.Text, Input_ConnectionString.Text);
            }
            else if (hasUsername
                && hasIp
                && hasPort
                && hasPassword)
            {
                if (Input_Client_Pass.Text.Length < MINIMUM_PASSWORD_LENGTH)
                {
                    GD.PrintErr($"Password must be at least {MINIMUM_PASSWORD_LENGTH} characters long.");
                    return;
                }

                connectedToServer = AutoloadManager.Instance.NetworkM.ConnectToServer(
                    Input_Client_ID.Text,
                    Input_Client_IP.Text,
                    Input_Client_Pass.Text,
                    Convert.ToInt32(Input_Client_Port.Text)
                );
            }

            if (connectedToServer)
            {
                GD.Print("Connected to server successfully.");
                AutoloadManager.Instance.SceneM.RequestSceneChange(SceneManager.SceneType.MultiplayerLobby);
            }
            else
            {
                GD.PrintErr("Failed to connect to server.");
            }
        }

        private void OnClientBackButtonPressed()
        {
            MainContainer.Show();
            ClientContainer.Hide();
        }
        #endregion
    }
}
