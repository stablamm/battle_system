using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BattleSystem.Autoloads
{
    public partial class NetworkManager : Node
    {
        public const string NODE_PATH = "/root/NetworkManager";

        private ENetMultiplayerPeer _peer;
        private Dictionary<long, string> _connectedPeers = new();
        private string _serverPasswordHash;
        private string _clientPasswordHash;
        private const int PASSWORD_MIN_LENGTH = 8;

        public override void _Ready()
        {
            GetTree().GetMultiplayer().ServerDisconnected += OnServerDisconnected;
        }

        /// <summary>
        /// Starts a server session.
        /// </summary>
        /// <param name="username">User's Username</param>
        /// <param name="ip">Server IP</param>
        /// <param name="password">Server Password</param>
        /// <param name="port">Server Port ( Default 7589 )</param>
        /// <param name="maxClients">Maximum number of connected clients ( Default 2 )</param>
        /// <returns>Returns True if the server started, False if not.</returns>
        public bool StartServer(string username, string ip, string password, int port = 7589, int maxClients = 2)
        {
            if (!ValidatePassword(password)) return false;

            _peer = new ENetMultiplayerPeer();
            _peer.SetBindIP(ip);
            _serverPasswordHash = HashPassword(password);

            Error err = _peer.CreateServer(port, maxClients);
            if (err != Error.Ok)
            {
                GD.PrintErr($"Server creation failed: {err}");
                return false;
            }

            Multiplayer.MultiplayerPeer = _peer;
            Multiplayer.PeerConnected += OnPeerConnected;
            Multiplayer.PeerDisconnected += OnPeerDisconnected;

            _connectedPeers[1] = username;
            GD.Print($"Server started on port: {port}");
            UpdatePlayerList(ConvertToGodotDictionary(_connectedPeers)); // Initial update for server
            return true;
        }

        public bool ConnectToServer(string ip, string password, string username, int port = 7589)
        {
            if (!ValidatePassword(password) || string.IsNullOrEmpty(username)) return false;

            _peer = new ENetMultiplayerPeer();
            Error err = _peer.CreateClient(ip, port);
            if (err != Error.Ok)
            {
                GD.PrintErr($"Client connection failed: {err}");
                return false;
            }

            _clientPasswordHash = HashPassword(password);
            Multiplayer.MultiplayerPeer = _peer;
            Multiplayer.ConnectedToServer += OnConnectedToServer;
            Multiplayer.ConnectionFailed += OnConnectionFailed;
            Multiplayer.PeerConnected += OnPeerConnected;
            Multiplayer.PeerDisconnected += OnPeerDisconnected;

            _connectedPeers[Multiplayer.GetUniqueId()] = username;
            GD.Print($"Connecting to server at {ip}:{port}");
            return true;
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
        private void RequestConnection(string username, string passwordHash)
        {
            if (!Multiplayer.IsServer()) return;

            long peerId = GetTree().GetMultiplayer().GetRemoteSenderId();
            if (passwordHash != _serverPasswordHash)
            {
                RpcId(peerId, nameof(ConnectionRejected), "Invalid password");
                _peer.DisconnectPeer((int)peerId);
                return;
            }

            _connectedPeers[peerId] = username;
            var playerList = ConvertToGodotDictionary(_connectedPeers);
            Rpc(nameof(UpdatePlayerList), playerList); // Broadcast to all peers
            UpdatePlayerList(playerList); // Explicitly update server locally
            RpcId(peerId, nameof(ConnectionAccepted));
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
        private void UpdatePlayerList(Godot.Collections.Dictionary players)
        {
            _connectedPeers.Clear();
            foreach (var key in players.Keys)
            {
                _connectedPeers[(long)(Variant)key] = (string)players[key];
            }

            GD.Print($"Updating player list locally: {_connectedPeers.Count} players connected");
            AutoloadManager.Instance.SignalM.EmitPlayerListUpdated();
        }

        [Rpc(MultiplayerApi.RpcMode.Authority)]
        private void ConnectionAccepted()
        {
            GD.Print("Connected to server successfully!");
        }

        [Rpc(MultiplayerApi.RpcMode.Authority)]
        private void ConnectionRejected(string reason)
        {
            GD.PrintErr($"Connection rejected: {reason}");
            DisconnectFromServer();
        }

        private void OnPeerConnected(long id)
        {
            GD.Print($"Peer connected with ID: {id}");
            if (Multiplayer.IsServer())
            {
                Rpc(nameof(UpdatePlayerList), ConvertToGodotDictionary(_connectedPeers));
                UpdatePlayerList(ConvertToGodotDictionary(_connectedPeers)); // Update server locally
            }
        }

        private void OnPeerDisconnected(long id)
        {
            GD.Print($"Peer disconnected with ID: {id}");
            if (Multiplayer.IsServer())
            {
                _connectedPeers.Remove(id);
                var playerList = ConvertToGodotDictionary(_connectedPeers);
                Rpc(nameof(UpdatePlayerList), playerList);
                UpdatePlayerList(playerList); // Update server locally
            }
        }

        private void OnConnectedToServer()
        {
            GD.Print("Successfully connected to server, sending connection request...");
            RpcId(1, nameof(RequestConnection), _connectedPeers[Multiplayer.GetUniqueId()], _clientPasswordHash);
            _clientPasswordHash = null;
        }

        private void OnConnectionFailed()
        {
            GD.PrintErr("Failed to connect to server.");
            _clientPasswordHash = null;
            CleanupConnection();
        }

        private void OnServerDisconnected()
        {
            GD.Print("Disconnected from server.");
            CleanupConnection();
        }

        private bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < PASSWORD_MIN_LENGTH)
            {
                GD.PrintErr($"Password must be at least {PASSWORD_MIN_LENGTH} characters long");
                return false;
            }
            return true;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private void DisconnectFromServer()
        {
            if (_peer != null)
            {
                _peer.Close();
                CleanupConnection();
            }
        }

        private void CleanupConnection()
        {
            _peer = null;
            _connectedPeers.Clear();
            Multiplayer.MultiplayerPeer = null;
        }

        public Dictionary<long, string> GetConnectedPeers()
        {
            var copy = new Dictionary<long, string>();
            foreach (var kvp in _connectedPeers)
            {
                copy[kvp.Key] = kvp.Value;
            }
            return copy;
        }

        private Godot.Collections.Dictionary ConvertToGodotDictionary(Dictionary<long, string> dict)
        {
            var godotDict = new Godot.Collections.Dictionary();
            foreach (var kvp in dict)
            {
                godotDict[kvp.Key] = kvp.Value;
            }
            return godotDict;
        }
    }
}