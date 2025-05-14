using BattleSystem.Autoloads;
using Godot;

namespace BattleSystem.Scenes.MultiplayerObjects;

public partial class LogContainer : Node2D
{
    private CanvasLayer UI;
    private RichTextLabel Logs;

    public override void _Ready()
    {
        UI = GetNode<CanvasLayer>("%UI");
        Logs = GetNode<RichTextLabel>("%Logs");

        Logs.Clear();

        AutoloadManager.Instance.SignalM.Connect(
            nameof(SignalManager.LogMessage_Debug_EventHandler)
            , new Callable(this, nameof(OnDebugLogMessage))
        );
        AutoloadManager.Instance.SignalM.Connect(
            nameof(SignalManager.LogMessage_Info_EventHandler)
            , new Callable(this, nameof(OnInfoLogMessage))
        );
        AutoloadManager.Instance.SignalM.Connect(
            nameof(SignalManager.LogMessage_Error_EventHandler)
            , new Callable(this, nameof(OnErrorLogMessage))
        );
    }

    private void OnDebugLogMessage(string message) => Logs.AppendText($"[color=blue]{message}[/color]\n");
    private void OnInfoLogMessage(string message) => Logs.AppendText($"[color=white]{message}[/color]\n");
    private void OnErrorLogMessage(string message) => Logs.AppendText($"[color=red]{message}[/color]\n");

    public void ToggleLogVisibility()
    {
        UI.Visible = !UI.Visible;
    }
}
