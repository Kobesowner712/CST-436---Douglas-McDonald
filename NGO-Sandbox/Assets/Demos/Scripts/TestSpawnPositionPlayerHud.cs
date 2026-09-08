using Unity.Netcode;
using UnityEngine;

/*
 * TestSpawnPositionPlayerHud is Demo 04's readout for server placement. It
 * keeps network identity separate from game placement by showing OwnerClientId,
 * the compact spawn slot, its human-readable lane, and the derived position.
 */

public class TestSpawnPositionPlayerHud : MonoBehaviour
{
    [SerializeField] TestSpawnPositionPlayer player;

    const float PanelX = 16f;
    const float PanelY = 86f;
    const float PanelWidth = 304f;
    const float PanelGap = 8f;
    const float Padding = 12f;
    const float RowHeight = 22f;
    const int RowCount = 6;
    const float PanelHeight = (Padding * 2f) + (RowHeight * RowCount);
    const float LabelWidth = 112f;

    static readonly Color LabelColor = new(0.78f, 0.82f, 0.88f);
    static readonly Color OwnerColor = new(0.45f, 0.78f, 1f);
    static readonly Color RemoteColor = new(0.95f, 0.82f, 0.35f);
    static readonly Color SlotColor = new(0.35f, 0.9f, 0.55f);
    static readonly Color PendingColor = new(0.95f, 0.58f, 0.5f);

    void Awake()
    {
        if (player == null)
            player = GetComponent<TestSpawnPositionPlayer>();
    }

    int GetDisplayRow()
    {
        int row = 0;
        NetworkManager networkManager = NetworkManager.Singleton;

        if (networkManager == null || networkManager.SpawnManager == null)
            return row;

        foreach (var spawnedObject in networkManager.SpawnManager.SpawnedObjectsList)
        {
            if (!spawnedObject.TryGetComponent(out TestSpawnPositionPlayer other)) continue;
            if (!other.HasSpawned) continue;
            if (other.NetworkObject.OwnerClientId < player.NetworkObject.OwnerClientId)
                row++;
        }

        return row;
    }

    void OnGUI()
    {
        if (player == null || !player.HasSpawned) return;

        float y = PanelY + (GetDisplayRow() * (PanelHeight + PanelGap));
        Rect panelRect = new(PanelX, y, PanelWidth, PanelHeight);
        Rect rowRect = new(
            panelRect.x + Padding,
            panelRect.y + Padding,
            panelRect.width - (Padding * 2f),
            RowHeight);

        GUI.Box(panelRect, GUIContent.none);
        DrawColoredLabel(rowRect, player.DisplayName, OwnerColor);
        rowRect.y += RowHeight;
        DrawGridRow(rowRect, "Local copy", player.IsOwnedByLocalClient ? "owner" : "remote",
            player.IsOwnedByLocalClient ? OwnerColor : RemoteColor);
        rowRect.y += RowHeight;
        DrawGridRow(rowRect, "OwnerClientId", player.NetworkObject.OwnerClientId.ToString(), OwnerColor);
        rowRect.y += RowHeight;
        DrawGridRow(rowRect, "Spawn slot", player.SpawnSlot < 0 ? "pending" : player.SpawnSlot.ToString(),
            player.SpawnSlot < 0 ? PendingColor : SlotColor);
        rowRect.y += RowHeight;
        DrawGridRow(rowRect, "Assigned side", player.SpawnLabel,
            player.SpawnSlot < 0 ? PendingColor : SlotColor);
        rowRect.y += RowHeight;
        DrawGridRow(rowRect, "Position", FormatPosition(player.AssignedPosition),
            player.SpawnSlot < 0 ? PendingColor : SlotColor);
    }

    static string FormatPosition(Vector3 position)
    {
        return $"({position.x:0.0}, {position.y:0.0}, {position.z:0.0})";
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
