using Unity.Netcode;
using UnityEngine;

/*
 * TestPlayer is the minimal player object used in Demos 01–02. Each peer has a
 * copy of every spawned player, so OnNetworkSpawn logs how IsOwner, IsServer,
 * IsClient, and IsHost describe this peer's particular copy. This class does
 * not mutate shared network state.
 */

public class TestPlayer : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        name = $"Player {NetworkObject.OwnerClientId}";
        ApplyOwnerColor();

        // These flags describe this peer's local copy of the spawned network object.
        Debug.Log(
            $"[PlayerIdentity] {name} spawned | ownerClientId={NetworkObject.OwnerClientId} | " +
            $"isOwner={IsOwner} | isServer={IsServer} | isClient={IsClient} | isHost={IsHost}");
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        Debug.Log($"[PlayerIdentity] {name} despawned");
    }

    void ApplyOwnerColor()
    {
        if (!TryGetComponent(out Renderer playerRenderer)) return;

        playerRenderer.material.color = Color.HSVToRGB((NetworkObject.OwnerClientId * 0.17f) % 1f, 0.75f, 1f);
    }
}
