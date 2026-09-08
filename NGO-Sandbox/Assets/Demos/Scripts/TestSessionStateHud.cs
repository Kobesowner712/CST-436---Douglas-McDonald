using UnityEngine;

/*
 * TestSessionStateHud is Demo 08's phase and ready-count display. Its buttons
 * send start, game-over, and return-to-lobby requests to the state machine;
 * they never assign the replicated phase directly.
 */

public class TestSessionStateHud : MonoBehaviour
{
    [SerializeField] TestSessionStateMachine stateMachine;

    const float PanelX = 308f;
    const float PanelY = 86f;
    const float PanelWidth = 300f;
    const float Padding = 12f;
    const float RowHeight = 24f;
    const float ButtonHeight = 30f;
    const float LabelWidth = 120f;
    const int RowCount = 5;
    const float PanelHeight = (Padding * 2f) + (RowHeight * RowCount) + (ButtonHeight * 2f) + 10f;

    static readonly Color LabelColor = new(0.78f, 0.82f, 0.88f);
    static readonly Color PhaseColor = new(0.45f, 0.78f, 1f);
    static readonly Color ReadyColor = new(0.35f, 0.9f, 0.55f);
    static readonly Color BlockedColor = new(0.95f, 0.58f, 0.5f);

    void Awake()
    {
        if (stateMachine == null)
            stateMachine = GetComponent<TestSessionStateMachine>();
    }

    void OnGUI()
    {
        if (stateMachine == null || !stateMachine.IsSpawned) return;

        Rect panelRect = new(PanelX, PanelY, PanelWidth, PanelHeight);
        Rect rowRect = new(
            panelRect.x + Padding,
            panelRect.y + Padding,
            panelRect.width - (Padding * 2f),
            RowHeight);

        GUI.Box(panelRect, GUIContent.none);
        DrawGridRow(rowRect, "Session phase", stateMachine.CurrentPhase.ToString(), PhaseColor);
        rowRect.y += RowHeight;
        DrawGridRow(rowRect, "Authority", stateMachine.IsServer ? "server" : "client observer",
            stateMachine.IsServer ? ReadyColor : LabelColor);
        rowRect.y += RowHeight;
        DrawGridRow(rowRect, "Ready players",
            $"{stateMachine.ReadyPlayerCount}/{stateMachine.RequiredReadyPlayers}",
            stateMachine.CanStart ? ReadyColor : BlockedColor);
        rowRect.y += RowHeight;
        DrawGridRow(rowRect, "Can start", stateMachine.CanStart ? "yes" : "no",
            stateMachine.CanStart ? ReadyColor : BlockedColor);
        rowRect.y += RowHeight;
        DrawGridRow(rowRect, "Countdown",
            stateMachine.CurrentPhase == TestSessionPhase.Countdown
                ? $"{Mathf.CeilToInt(stateMachine.CountdownRemaining)}"
                : "-",
            PhaseColor);
        rowRect.y += RowHeight + 6f;

        if (GUI.Button(new Rect(rowRect.x, rowRect.y, rowRect.width, ButtonHeight), "Start / Countdown"))
            stateMachine.RequestStart();
        rowRect.y += ButtonHeight + 4f;

        string secondaryText = stateMachine.CurrentPhase == TestSessionPhase.GameOver
            ? "Return To Lobby"
            : "Game Over";
        if (GUI.Button(new Rect(rowRect.x, rowRect.y, rowRect.width, ButtonHeight), secondaryText))
        {
            if (stateMachine.CurrentPhase == TestSessionPhase.GameOver)
                stateMachine.RequestReturnToLobby();
            else
                stateMachine.RequestGameOver();
        }
    }

    static void DrawGridRow(Rect rowRect, string label, string value, Color valueColor)
    {
        Rect labelRect = new(rowRect.x, rowRect.y, LabelWidth, rowRect.height);
        Rect valueRect = new(rowRect.x + LabelWidth, rowRect.y, rowRect.width - LabelWidth, rowRect.height);

        DrawColoredLabel(labelRect, label, LabelColor);
        DrawColoredLabel(valueRect, value, valueColor);
    }

    static void DrawColoredLabel(Rect rect, string text, Color color)
    {
        Color previousColor = GUI.color;
        GUI.color = color;
        GUI.Label(rect, text);
        GUI.color = previousColor;
    }
}
