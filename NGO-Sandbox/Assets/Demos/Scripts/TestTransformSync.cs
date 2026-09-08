using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * TestTransformSync is Demo 05's owner-authoritative movement. The owning client
 * moves its transform immediately; NetworkTransform on the prefab forwards that
 * result to other peers. The server does not validate the movement.
 */

public class TestTransformSync : NetworkBehaviour
{
    public float speed = 5f;

    public override void OnNetworkSpawn()
    {
        Debug.Log(
            $"[OwnerMovement] {name} spawned | ownerClientId={NetworkObject.OwnerClientId} | " +
            $"isOwner={IsOwner} | isServer={IsServer} | isClient={IsClient}");
    }

    public override void OnNetworkDespawn()
    {
        Debug.Log($"[OwnerMovement] {name} despawned | ownerClientId={NetworkObject.OwnerClientId}");
    }
    
    void Update()
    {
        if (!IsOwner) return;
        if (Keyboard.current == null) return;

        float deltaMove = 0f;
        if (Keyboard.current.aKey.isPressed) deltaMove -= speed;
        if (Keyboard.current.dKey.isPressed) deltaMove += speed;

        if (Mathf.Approximately(deltaMove, 0f)) return;

        transform.Translate(Vector3.right * (deltaMove * Time.deltaTime));
    }
}
