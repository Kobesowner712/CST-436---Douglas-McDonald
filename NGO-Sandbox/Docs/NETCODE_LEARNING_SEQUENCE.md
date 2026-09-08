# Netcode for GameObjects Learning Sequence

One scene per concept. Work through demos **01–08** in order, then apply the same authority split in Pong.

For every networked feature, ask:

```text
Who owns this object?
Who is allowed to request a change?
Who is allowed to mutate authoritative state?
How does the result replicate to the other peers?
```

Separate:

- Local input
- Server authority
- Object ownership
- Replicated state
- Visual / UI response

See `BASIC_SESSION_MANAGER_DIAGRAMS.md` for the Demo 01 host / client / disconnect flow.

Each Demo 01–08 scene already includes a `NetworkManager`. Open that scene; do not look for a Demo 00 bootstrap scene.

## Recommended Sequence

## 1. Connection Lifecycle Tracking

### Demo Scene

`Assets/Demos/Scenes/[Demo 01] Sessions.unity`

### Supporting Assets

- `Assets/Demos/Prefabs/Session Manager.prefab`
- `Assets/Demos/Prefabs/Player.prefab`
- `Assets/Demos/Scripts/BasicSessionManager.cs`
- `Assets/Demos/Scripts/BasicSessionHud.cs`

### Goal

Make the session UI accurately show how many players are connected.

### Additions

- Increment the count when a client connects.
- Decrement the count when a client disconnects.
- Show or hide session UI based on whether the local process is connected.
- Log connection events clearly in the Console.

### Concepts To Learn

- `NetworkManager.StartHost()`
- `NetworkManager.StartClient()`
- `NetworkManager.Shutdown()`
- `NetworkManager.OnConnectionEvent`
- `NetworkManager.OnClientDisconnectCallback`
- Server-only writes to `NetworkVariable<T>`

### Key Questions

- Does the host count as a connected client?
- Which machine receives each connection event?
- Why should only the server update the connection count?
- What happens when a client disconnects versus when the host shuts down?

### Success Check

Start one host and one client. The UI should show the expected player count. Disconnect the client and confirm the count updates correctly on remaining peers.

## 2. Player Spawn Identity

### Goal

Make each spawned player object display or log its identity clearly.

### Demo Scene

`Assets/Demos/Scenes/[Demo 02] Player Spawn Identity.unity`

### Supporting Assets

- `Assets/Demos/Prefabs/Player Identity.prefab`
- `Assets/Demos/Scripts/TestPlayer.cs`
- `Assets/Demos/Scripts/TestPlayerHud.cs`

### Additions

- Show each player object's `OwnerClientId`.
- Log `IsOwner`, `IsServer`, `IsClient`, and `IsHost` from `OnNetworkSpawn()`.
- Optionally give each player a distinct material/color based on owner ID.

### Concepts To Learn

- `NetworkBehaviour.OnNetworkSpawn()`
- `NetworkBehaviour.OnNetworkDespawn()`
- `NetworkObject.OwnerClientId`
- `IsOwner`
- `IsServer`
- `IsClient`
- `IsHost`

### Key Questions

- How many copies of each player object exist across the session?
- Which copy is owned by the local client?
- Why does every peer see every networked player object?
- How is object ownership different from server authority?

### Success Check

With host and client running, each peer should be able to identify its own player object and the remote player object.

## 3. Replicated Ready State

### Goal

Let each player toggle a ready state and have that state appear on all peers.

### Demo Scene

`Assets/Demos/Scenes/[Demo 03] Ready State.unity`

### Supporting Assets

- `Assets/Demos/Prefabs/Player Ready State.prefab`
- `Assets/Demos/Scripts/TestReadyPlayer.cs`
- `Assets/Demos/Scripts/TestReadyPlayerHud.cs`

### Additions

- Add a ready state to the player or session model.
- Let only the owning player request a ready toggle.
- Use a `ServerRpc` to send the request to the server.
- Let the server update the replicated ready state.
- Display ready/not ready in UI or logs.

### Concepts To Learn

- Owner input
- `ServerRpc`
- Server-side validation
- `NetworkVariable<bool>`
- Replicated player state

### Key Questions

- Why should the client request a ready change instead of directly writing the value?
- What happens if a non-owner tries to toggle another player's ready state?
- Where should ready state live: on the player object or the session object?
- How does the UI know when the value changed?

### Success Check

Each client can toggle only its own ready state. All peers see the updated ready state.

## 4. Server-Assigned Spawn Positions

### Goal

Make the server decide where each player appears.

### Demo Scene

`Assets/Demos/Scenes/[Demo 04] Server-Assigned Spawn Positions.unity`

### Supporting Assets

- `Assets/Demos/Prefabs/Player Spawn Position.prefab`
- `Assets/Demos/Scripts/TestSpawnPositionPlayer.cs`
- `Assets/Demos/Scripts/TestSpawnPositionPlayerHud.cs`

### Additions

- Assign player spawn positions based on connection order or client ID.
- Place one player on the left and one player on the right.
- Assign additional clients to deterministic overflow rows so more than two clients can still connect.

### Concepts To Learn

- Server-authoritative spawning
- Player object placement
- Connection order versus client ID
- Game-specific session rules

### Key Questions

- Who decides the initial transform of a spawned player?
- Should clients be allowed to choose their own spawn location?
- What should happen when the second player joins?
- What should happen if a third player joins?

### Success Check

When host and client connect, their player objects appear in predictable, server-assigned positions.

## 5. Owner-Authoritative Movement Experiment

### Goal

Let each player move their own object using a simple owner-authoritative approach.

### Demo Scene

`Assets/Demos/Scenes/[Demo 05] Owner-Authoritative Movement.unity`

### Supporting Assets

- `Assets/Demos/Prefabs/Player Transform Test.prefab`
- `Assets/Demos/Scripts/TestTransformSync.cs`

### Additions

- Only process movement input on the owning player's local copy.
- Use a network transform setup that replicates movement.
- Compare what the owner sees versus what remote peers see.

### Concepts To Learn

- Client-side input ownership
- Owner-authoritative movement
- Transform replication
- Perceived latency
- Trusting client movement

### Key Questions

- Why should non-owned player objects ignore local input?
- What does the owner see immediately?
- What do remote clients see after replication?
- Why might this be unsuitable for competitive Pong movement?

### Success Check

Each player can move only their own object. Remote peers see that movement replicated.

## 6. Server-Authoritative Movement Experiment

### Goal

Rebuild simple player movement so the server has final authority.

### Demo Scene

`Assets/Demos/Scenes/[Demo 06] Server-Authoritative Movement.unity`

### Supporting Assets

- `Assets/Demos/Prefabs/Player Server Movement.prefab`
- `Assets/Demos/Scripts/TestServerMovementPlayer.cs`

### Additions

- The owning client reads input locally.
- The client sends movement intent to the server using a `ServerRpc`.
- The server moves the object.
- All clients observe the result through replicated transform/state.
- Keep this as a separate comparison point from `[Demo 05] Owner-Authoritative Movement.unity`, where the owner moves directly.

### Concepts To Learn

- Input intent versus direct state mutation
- Server-authoritative movement
- Server validation
- Latency tradeoffs
- Competitive multiplayer authority

### Key Questions

- What exactly should the client send: position, velocity, or input direction?
- Why is sending input intent usually safer than sending final position?
- How does movement feel different from owner-authoritative movement?
- Which model fits Pong paddles, and which fits the ball?

### Success Check

Players still move their own objects, but only the server applies the final movement.

## 7. Server-Owned Ball Object

### Goal

Introduce a non-player network object controlled only by the server.

### Demo Scene

`Assets/Demos/Scenes/[Demo 07] Server-Owned Ball.unity`

### Supporting Assets

- `Assets/Demos/Prefabs/Server Ball.prefab`
- `Assets/Demos/Scripts/TestServerBall.cs`
- `Assets/Demos/Scripts/TestServerBallSpawner.cs`

### Additions

- Create a simple ball prefab with `NetworkObject`.
- Spawn the ball from the server.
- Move the ball only on the server.
- Replicate its movement to all clients.
- Reset or despawn the ball when the session ends.

### Concepts To Learn

- Networked non-player objects
- Server-owned objects
- Runtime spawn/despawn
- Server-authoritative simulation
- Difference between player objects and game objects

### Key Questions

- Who owns the ball?
- Should any client own the ball?
- Why is the server the natural authority for the ball in Pong?
- What state must replicate for clients to see the same game?

### Success Check

The ball exists for all peers, but only the server controls its movement.

## 8. Session State Machine

### Goal

Represent the game/session phase explicitly.

### Demo Scene

`Assets/Demos/Scenes/[Demo 08] Session State Machine.unity`

### Supporting Assets

- `Assets/Demos/Scripts/TestSessionStateMachine.cs`
- `Assets/Demos/Scripts/TestSessionStateHud.cs`

### Additions

- Add a replicated session state such as:
  - `Lobby`
  - `Countdown`
  - `Playing`
  - `GameOver`
- Only the server changes the session state.
- Clients update UI based on the replicated state.
- Start gameplay only when enough players are connected and ready.

### Concepts To Learn

- Replicated session state
- Server-owned state machines
- UI as a response to network state
- Separating lobby/session logic from player logic

### Key Questions

- Why should session state be centralized?
- Which transitions are valid?
- Who is allowed to start the game?
- What should happen when a player disconnects during each state?

### Success Check

All peers agree on the current session phase and update their UI consistently.

