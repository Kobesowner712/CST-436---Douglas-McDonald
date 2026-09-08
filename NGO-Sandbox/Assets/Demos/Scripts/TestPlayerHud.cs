using Unity.Netcode;
using UnityEngine;

/*
 * TestPlayerHud is the Demo 02 identity readout. One panel appears for each
 * spawned player copy and exposes the ownership and process-role flags that
 * TestPlayer logs, without changing any network state.
 */

public class TestPlayerHud : MonoBehaviour
{
    [SerializeField] TestPlayer player;

    const float PanelX = 16f;
    const float PanelY = 86f;
    const float PanelWidth = 276f;
    const float PanelGap = 8f;
    const float Padding = 12f;
    const float RowHeight = 22f;
    const int RowCount = 7;
    const float PanelHeight = (Padding * 2f) + (RowHeight * RowCount);
    const float LabelWidth = 104f;

    static readonly Color LabelColor = new(0.78f, 0.82f, 0.88f);
    static readonly Color TrueColor = new(0.35f, 0.9f, 0.55f);
    static readonly Color FalseColor = new(0.95f, 0.58f, 0.5f);
    static readonly Color OwnerColor = new(0.45f, 0.78f, 1f);
    static readonly Color RemoteColor = new(0.95f, 0.82f, 0.35f);

    void Awake()
    {
        if (player == null)
            player = GetComponent<TestPlayer>();
    }

    int GetDisplayRow()
    {
        int row = 0;
        NetworkManager networkManager = NetworkManager.Singleton;

        if (networkManager == null || networkManager.SpawnManager == null)
            return row;

        foreach (var spawnedObject in networkManager.SpawnManager.SpawnedObjectsList)
        {
            if (!spawnedObject.TryGetComponent(out TestPlayer other)) continue;
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
        DrawGridRow(rowRect, "IsOwner", player.IsOwner);
        rowRect.y += RowHeight;
        DrawGridRow(rowRect, "IsServer", player.IsServer);
        rowRect.y += RowHeight;
        DrawGridRow(rowRect, "IsClient", player.IsClient);
        rowRect.y += RowHeight;
        DrawGridRow(rowRect, "IsHost", player.IsHost);
    }

    static void DrawGridRow(Rect rowRect, string label, bool value)
    {
        DrawGridRow(rowRect, label, value ? "True" : "False", value ? TrueColor : FalseColor);
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
