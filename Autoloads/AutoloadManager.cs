using Godot;

namespace BattleSystem.Autoloads
{
    public partial class AutoloadManager : Node
    {
        public static AutoloadManager Instance;
        public BattleManager BattleM;
        public NetworkManager NetworkM;
        public SceneManager SceneM;
        public SignalManager SignalM;

        public override void _Ready()
        {
            Instance = this;

            BattleM = GetNode<BattleManager>(BattleManager.NODE_PATH);
            NetworkM = GetNode<NetworkManager>(NetworkManager.NODE_PATH);
            SceneM = GetNode<SceneManager>(SceneManager.NODE_PATH);
            SignalM = GetNode<SignalManager>(SignalManager.NODE_PATH);
        }
    }
}