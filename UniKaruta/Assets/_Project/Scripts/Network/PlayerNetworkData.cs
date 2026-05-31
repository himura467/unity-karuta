using Fusion;

namespace UniKaruta.Scripts.Network
{
    public struct PlayerNetworkData : INetworkStruct
    {
        public PlayerRef Player;
        public bool IsRegistered;
    }
}
