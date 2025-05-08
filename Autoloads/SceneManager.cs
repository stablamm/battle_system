using Godot;
using System.Collections.Generic;
using System.Linq;
namespace BattleSystem.Autoloads
{
    public partial class SceneManager : Node
    {
        public const string NODE_PATH = "/root/SceneManager";

        public enum SceneType
        {
            MainMenu,
            ServerClientMenu,
            MultiplayerLobby
        }

        // Dictionary to store scene paths
        private Dictionary<SceneType, string> _scenePaths = new()
        {
            { SceneType.MainMenu, "res://Scenes/Menus/MainMenu.tscn" },
            { SceneType.ServerClientMenu, "res://Scenes/Menus/ServerClientMenu.tscn" },
            { SceneType.MultiplayerLobby, "res://Scenes/Lobbies/MultiplayerLobby.tscn" }
        };

        // Current scene tracking
        private SceneType _currentSceneName = SceneType.MainMenu;
        public SceneType CurrentSceneName => _currentSceneName;

        public override void _Ready()
        {
            if (Multiplayer.IsServer())
            {
                Multiplayer.PeerConnected += OnPeerConnected;
            }
        }

        public override void _ExitTree()
        {
            if (Multiplayer.IsServer())
            {
                Multiplayer.PeerConnected -= OnPeerConnected;
            }
        }

        private void PerformSceneChange(string scenePath, SceneType sceneName)
        {
            // Clean up current scene
            Node currentScene = GetTree().CurrentScene;
            if (currentScene != null && currentScene != this)
            {
                currentScene.QueueFree();
            }

            // Load and instance new scene
            var packedScene = GD.Load<PackedScene>(scenePath);
            if (packedScene == null)
            {
                GD.PrintErr($"Failed to load scene at {scenePath}");
                return;
            }

            Node newScene = packedScene.Instantiate();
            GetTree().Root.AddChild(newScene);
            GetTree().CurrentScene = newScene;

            _currentSceneName = sceneName;
            GD.Print($"Changed to scene: {sceneName}");
        }

        // Handle new peer connections
        private void OnPeerConnected(long id)
        {
            if (Multiplayer.IsServer())
            {
                // Sync new peer with current scene
                RpcId(id, nameof(ChangeScene), (int)_currentSceneName);
            }
        }

        // Server-side: Request all clients to change scene
        [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        public void RequestSceneChange(SceneType sceneName)
        {
            if (!Multiplayer.IsServer())
            {
                GD.PrintErr("Only server can request scene changes");
                return;
            }

            if (!_scenePaths.ContainsKey(sceneName))
            {
                GD.PrintErr($"Scene {sceneName} not found in scene dictionary");
                return;
            }

            // Call the RPC on all clients
            Rpc(nameof(ChangeScene), (int)sceneName);
        }

        // Client-side: Actually perform the scene change
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
        private void ChangeScene(SceneType sceneName)
        {
            if (!_scenePaths.TryGetValue(sceneName, out string scenePath))
            {
                GD.PrintErr($"Scene {sceneName} not found in scene dictionary");
                return;
            }

            // Defer the scene change to avoid modifying the scene tree during processing
            CallDeferred(nameof(PerformSceneChange), scenePath, (int)sceneName);
        }
    }
}