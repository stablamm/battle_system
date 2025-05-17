using Godot;

namespace BattleSystem.Autoloads
{
    public partial class AutoloadManager : Node
    {
        public static AutoloadManager Instance;
        public LogManager LogM;
        public BattleManager BattleM;
        public NetworkManager NetworkM;
        public SceneManager SceneM;
        public SignalManager SignalM;
        public AbilityManager AbilityM;
        public StateManager StateM;

        public override void _Ready()
        {
            Instance = this;

            LogM = GetNode<LogManager>(LogManager.NODE_PATH);
            BattleM = GetNode<BattleManager>(BattleManager.NODE_PATH);
            NetworkM = GetNode<NetworkManager>(NetworkManager.NODE_PATH);
            SceneM = GetNode<SceneManager>(SceneManager.NODE_PATH);
            SignalM = GetNode<SignalManager>(SignalManager.NODE_PATH);
            AbilityM = GetNode<AbilityManager>(AbilityManager.NODE_PATH);
            StateM = GetNode<StateManager>(StateManager.NODE_PATH);
        }
    }
}