using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;

namespace UniKaruta.Scripts.Network
{
    public class PlayerRegistry : NetworkBehaviour, INetworkRunnerCallbacks
    {
        public const int MaxPlayers = 8;
        public const int MinPlayers = 2;

        public event Action GameBelowMinimumPlayers;

        [Networked, Capacity(MaxPlayers)]
        public NetworkArray<PlayerNetworkData> Players { get; }

        public int RegisteredPlayerCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < MaxPlayers; i++)
                    if (Players[i].IsRegistered) count++;
                return count;
            }
        }

        public bool IsBelowMinimumPlayers => RegisteredPlayerCount < MinPlayers;

        public override void Spawned() => Runner.AddCallbacks(this);

        public override void Despawned(NetworkRunner runner, bool hasState) => runner.RemoveCallbacks(this);

        void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (!Object.HasStateAuthority) return;
            if (player.AsIndex >= MaxPlayers) return;
            Players.Set(player.AsIndex, new PlayerNetworkData { Player = player, IsRegistered = true });
        }

        void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (!Object.HasStateAuthority) return;
            if (player.AsIndex >= MaxPlayers) return;
            Players.Set(player.AsIndex, default);
            if (IsBelowMinimumPlayers) GameBelowMinimumPlayers?.Invoke();
        }

        void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
        void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
        void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input) { }
        void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) { }
        void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { }
        void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }
    }
}
