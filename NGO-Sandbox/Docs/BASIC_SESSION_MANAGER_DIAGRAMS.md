# Basic Session Manager Diagrams

These diagrams show the first session-management lesson: starting a host, joining as a client, disconnecting, and updating the replicated player count.

## 1. What Each Script Is Responsible For

```mermaid
flowchart LR
    Student[Student clicks button]
    Hud[BasicSessionHud<br/>Draws buttons and labels]
    Manager[BasicSessionManager<br/>Starts and stops sessions<br/>Tracks session state]
    NGO[NetworkManager<br/>Netcode for GameObjects]
    Count[NetworkVariable&lt;int&gt;<br/>Replicated player count]

    Student --> Hud
    Hud -->|StartHost / StartClient / Disconnect| Manager
    Manager -->|StartHost / StartClient / Shutdown| NGO
    NGO -->|connection event| Manager
    Manager -->|server writes| Count
    Count -->|clients read| Hud
    Manager -->|IsConnected / LocalRole / PlayerCount| Hud
```

Key idea: the HUD is not the session. It only shows controls and reads state from `BasicSessionManager`.

## 2. Starting A Host

```mermaid
sequenceDiagram
    participant H as Host HUD
    participant SM as BasicSessionManager
    participant NM as NetworkManager
    participant NV as PlayerCount NetworkVariable
    participant C as Connected clients

    H->>SM: StartHost()
    SM->>NM: NetworkManager.Singleton.StartHost()
    NM-->>SM: OnNetworkSpawn()
    SM->>NM: subscribe OnConnectionEvent
    NM-->>SM: OnConnectionEvent(host client connected)
    SM->>C: read ConnectedClientsIds.Count
    SM->>NV: write player count
    NV-->>H: HUD reads PlayerCount
```

Key idea: a host is both the server and a local client, so the host counts as one connected player.

## 3. Joining As A Client

```mermaid
sequenceDiagram
    participant CH as Client HUD
    participant CSM as Client BasicSessionManager
    participant CNM as Client NetworkManager
    participant SNM as Host NetworkManager
    participant SSM as Host BasicSessionManager
    participant NV as PlayerCount NetworkVariable

    CH->>CSM: StartClient()
    CSM->>CNM: NetworkManager.Singleton.StartClient()
    CNM->>SNM: connection request
    SNM-->>SSM: OnConnectionEvent(client connected)
    SSM->>SNM: read ConnectedClientsIds.Count
    SSM->>NV: server writes new player count
    NV-->>CSM: replicated value arrives on client
    CH-->>CSM: reads PlayerCount for display
```

Key idea: the client asks to join, but the server is the peer that updates shared session state.

## 4. Disconnecting A Client

```mermaid
sequenceDiagram
    participant CH as Client HUD
    participant CSM as Client BasicSessionManager
    participant CNM as Client NetworkManager
    participant SNM as Host NetworkManager
    participant SSM as Host BasicSessionManager
    participant NV as PlayerCount NetworkVariable

    CH->>CSM: Disconnect()
    CSM->>CNM: NetworkManager.Singleton.Shutdown()
    CNM-->>SNM: client disconnects
    SNM-->>SSM: OnConnectionEvent(client disconnected)
    SSM->>SNM: read ConnectedClientsIds.Count
    SSM->>NV: server writes new player count
    NV-->>CH: host HUD shows updated count
```

Key idea: a disconnect is also a connection event. The manager does not need separate increment and decrement code; it asks Netcode how many clients are connected now.

## 5. Host Shutdown

```mermaid
sequenceDiagram
    participant HH as Host HUD
    participant HSM as Host BasicSessionManager
    participant HNM as Host NetworkManager
    participant Clients as Connected Clients
    participant NV as PlayerCount NetworkVariable

    HH->>HSM: Disconnect()
    HSM->>HNM: NetworkManager.Singleton.Shutdown()
    HNM-->>Clients: session ends
    HNM-->>HSM: OnNetworkDespawn()
    HSM->>HNM: unsubscribe OnConnectionEvent
    HSM->>NV: reset player count to 0
```

Key idea: when the host shuts down, the server goes away. The session is over for everyone.

## 6. The Repeated Pattern

```mermaid
flowchart TD
    Click[Local button click]
    Request[Call BasicSessionManager method]
    NGO[Ask NetworkManager to start, join, or shut down]
    Event[Netcode raises connection event]
    Server{Is this peer the server?}
    Count[Read ConnectedClientsIds.Count]
    Replicate[Write NetworkVariable player count]
    Display[HUD displays current state]

    Click --> Request --> NGO --> Event --> Server
    Server -->|yes| Count --> Replicate --> Display
    Server -->|no| Display
```

Teaching shortcut:

```text
Button -> Manager action -> NetworkManager -> connection event -> server updates count -> HUD displays state
```

