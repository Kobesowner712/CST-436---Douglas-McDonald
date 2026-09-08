# NGO Sandbox — Pong demos

Isolated Netcode for GameObjects scenes for converting **Pong Starter** into a two-player networked match. Open one demo at a time. Start **Host** in one editor (or build), **Client** in the other.

Unity **6000.5.9f1**. First open will download packages.

## How this maps to Pong

| Demo | Concept | In Pong |
|------|---------|---------|
| 01 Sessions | Host / client / disconnect and a replicated player count | `SessionManager` |
| 02 Player Spawn Identity | `OnNetworkSpawn`, `OwnerClientId`, `IsOwner` | Who owns this paddle |
| 03 Ready State | Owner sends a `ServerRpc`; server writes a `NetworkVariable` | Same request-then-replicate shape as score |
| 04 Server-Assigned Spawn Positions | Server decides left / right placement | Pong applies left/right locally from owner id; this demo shows the server-assigned alternative |
| 05 Owner-Authoritative Movement | Owner moves; `NetworkTransform` replicates | Paddle Z movement |
| 06 Server-Authoritative Movement | Client sends input intent; server moves | Contrast with 05 — use this for the **ball**, not the paddles |
| 07 Server-Owned Ball | Non-player object the server simulates | The ball |
| 08 Session State Machine | Replicated lobby / playing phase | Optional. Pong can start when two clients connect |

Pong's split: **owner** reads paddle input; **server** runs ball physics, goals, and score.

## Setup

1. Unzip so you have a folder named `NGO-Sandbox` (this file should sit next to `Assets/`).
2. Unity Hub → Add → pick that folder.
3. Open `Assets/Demos/Scenes/[Demo 01] Sessions.unity`.
4. Enter Play, click **Host**, then start a second instance as **Client**.

Every demo scene already has a `NetworkManager`. There is no Demo 00 scene in this package.

## Read next

- `Docs/NETCODE_LEARNING_SEQUENCE.md` — goals, concepts, and success checks for demos 01–08
- `Docs/BASIC_SESSION_MANAGER_DIAGRAMS.md` — host / client / disconnect flow
