using Unity.Netcode;
using UnityEngine;

/*
 * TestReadyPlayer is the Demo 03 player and is reused by Demo 08. The owner
 * requests a ready toggle, the server validates the sender and writes the
 * NetworkVariable, and every peer reads the result. Ownership permits the
 * request; it does not grant permission to write shared state directly.
 */

public class TestReadyPlayer : NetworkBehaviour
{
    public bool IsReady => _isReady.Value;

    readonly NetworkVariable<bool> _isReady = new(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        name = $"Ready Player {NetworkObject.OwnerClientId}";
        ApplyOwnerColor();

        Debug.Log(
            $"[ReadyState] {name} spawned | ownerClientId={NetworkObject.OwnerClientId} | " +
            $"isOwner={IsOwner} | isServer={IsServer} | isClient={IsClient} | isHost={IsHost}");
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        Debug.Log($"[ReadyState] {name} despawned");
    }

    public void RequestToggleReady()
    {
        if (!IsOwner)
        {
            Debug.LogWarning(
                $"[ReadyState] Rejected local toggle attempt on non-owner copy | " +
                $"ownerClientId={NetworkObject.OwnerClientId}");
            return;
        }

        ToggleReadyRpc();
    }

    [Rpc(SendTo.Server)]
    void ToggleReadyRpc(RpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;
        if (senderClientId != NetworkObject.OwnerClientId)
        {
            Debug.LogWarning(
                $"[ReadyState] Rejected ready toggle | senderClientId={senderClientId} | " +
                $"ownerClientId={NetworkObject.OwnerClientId}");
            return;
        }

        _isReady.Value = !_isReady.Value;
        Debug.Log(
            $"[ReadyState] Server toggled {name} | ownerClientId={NetworkObject.OwnerClientId} | " +
            $"isReady={_isReady.Value}");
    }

    void ApplyOwnerColor()
    {
        if (!TryGetComponent(out Renderer playerRenderer)) return;

        playerRenderer.material.color = Color.HSVToRGB((NetworkObject.OwnerClientId * 0.17f) % 1f, 0.75f, 1f);
    }
}
