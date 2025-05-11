using Godot;
using System.Collections.Generic;

namespace BattleSystem.Autoloads
{
    public partial class SceneManager : Node
    {
        public const string NODE_PATH = "/root/SceneManager";

        public enum SceneType
        {
            MainMenu,
            ServerClientMenu,
            MultiplayerLobby,
            BattleMenu,
            DemoBattleScene
        }

        // Dictionary to store scene paths
        private Dictionary<SceneType, string> _scenePaths = new()
        {
            { SceneType.MainMenu, "res://Scenes/Menus/MainMenu.tscn" },
            { SceneType.ServerClientMenu, "res://Scenes/Menus/ServerClientMenu.tscn" },
            { SceneType.MultiplayerLobby, "res://Scenes/Lobbies/MultiplayerLobby.tscn" },
            { SceneType.BattleMenu, "res://Scenes/Menus/BattleMenu.tscn" },
            { SceneType.DemoBattleScene, "res://Scenes/BattleScenes/DemoScene.tscn" }
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
                AutoloadManager.Instance.LogM.WriteLog($"Failed to load scene at {scenePath}", LogManager.LOG_TYPE.ERROR);
                return;
            }

            Node newScene = packedScene.Instantiate();
            GetTree().Root.AddChild(newScene);
            GetTree().CurrentScene = newScene;

            _currentSceneName = sceneName;
            AutoloadManager.Instance.LogM.WriteLog($"Changed to scene: {sceneName}", LogManager.LOG_TYPE.INFO);
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
                AutoloadManager.Instance.LogM.WriteLog("Only server can request scene changes", LogManager.LOG_TYPE.ERROR);
                return;
            }

            if (!_scenePaths.ContainsKey(sceneName))
            {
                AutoloadManager.Instance.LogM.WriteLog($"Scene {sceneName} not found in scene dictionary", LogManager.LOG_TYPE.ERROR);
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
                AutoloadManager.Instance.LogM.WriteLog($"Scene {sceneName} not found in scene dictionary", LogManager.LOG_TYPE.ERROR);
                return;
            }

            // Defer the scene change to avoid modifying the scene tree during processing
            CallDeferred(nameof(PerformSceneChange), scenePath, (int)sceneName);
        }
    }
}