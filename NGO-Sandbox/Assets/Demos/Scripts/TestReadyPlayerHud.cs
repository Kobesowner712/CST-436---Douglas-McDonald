using Unity.Netcode;
using UnityEngine;

/*
 * TestReadyPlayerHud shows each player's replicated ready state in Demos 03
 * and 08. Only the local owner's panel gets a button, and that button calls
 * RequestToggleReady instead of changing the NetworkVariable itself.
 */

public class TestReadyPlayerHud : MonoBehaviour
{
    [SerializeField] TestReadyPlayer player;

    const float PanelX = 16f;
    const float PanelY = 86f;
    const float PanelWidth = 276f;
    const float PanelGap = 8f;
    const float Padding = 12f;
    const float RowHeight = 24f;
    const int RowCount = 5;
    const float ButtonHeight = 30f;
    const float PanelHeight = (Padding * 2f) + (RowHeight * RowCount) + ButtonHeight;
    const float LabelWidth = 104f;

    static readonly Color LabelColor = new(0.78f, 0.82f, 0.88f);
    static readonly Color ReadyColor = new(0.35f, 0.9f, 0.55f);
    static readonly Color NotReadyColor = new(0.95f, 0.58f, 0.5f);
    static readonly Color OwnerColor = new(0.45f, 0.78f, 1f);
    static readonly Color RemoteColor = new(0.95f, 0.82f, 0.35f);

    void Awake()
    {
        if (player == null)
            player = GetComponent<TestReadyPlayer>();
    }

    int GetDisplayRow()
    {
        int row = 0;
        NetworkManager networkManager = NetworkManager.Singleton;

        if (networkManager == null || networkManager.SpawnManager == null)
            return row;

        foreach (var spawnedObject in networkManager.SpawnManager.SpawnedObjectsList)
        {
            if (!spawnedObject.TryGetComponent(out TestReadyPlayer other)) continue;
            if (!other.IsSpawned) continue;
            if (other.NetworkObject.OwnerClientId < player.NetworkObject.OwnerClientId)
                row++;
        }

        return row;
    }

    void OnGUI()
    {
        if (player == null || !player.IsSpawned) return;

        float y = PanelY + (GetDisplayRow() * (PanelHeight + PanelGap));
        Rect panelRect = new(PanelX, y, PanelWidth, PanelHeight);
        Rect rowRect = new(
            panelRect.x + Padding,
            panelRect.y + Padding,
            panelRect.width - (Padding * 2f),
            RowHeight);

        GUI.Box(panelRect, GUIContent.none);
        DrawColoredLabel(rowRect, player.name, OwnerColor);
        rowRect.y += RowHeight;
        DrawGridRow(rowRect, "Local copy", player.IsOwner ? "owner" : "remote",
            player.IsOwner ? OwnerColor : RemoteColor);
        rowRect.y += RowHeight;
        DrawGridRow(rowRect, "OwnerClientId", player.NetworkObject.OwnerClientId.ToString(), OwnerColor);
        rowRect.y += RowHeight;
        DrawGridRow(rowRect, "Ready state", player.IsReady ? "Ready" : "Not Ready",
            player.IsReady ? ReadyColor : NotReadyColor);
        rowRect.y += RowHeight;
        DrawGridRow(rowRect, "Toggle access", player.IsOwner ? "local owner" : "remote only",
            player.IsOwner ? ReadyColor : RemoteColor);
        rowRect.y += RowHeight;

        if (!player.IsOwner) return;

        Rect buttonRect = new(rowRect.x, rowRect.y + 2f, rowRect.width, ButtonHeight);
        string buttonText = player.IsReady ? "Not Ready" : "Ready";
        if (GUI.Button(buttonRect, buttonText))
            player.RequestToggleReady();
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
