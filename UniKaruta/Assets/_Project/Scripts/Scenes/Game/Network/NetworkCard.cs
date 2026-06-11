using Fusion;

namespace UniKaruta.Scripts.Scenes.Game.Network
{
    public class NetworkCard : NetworkBehaviour
    {
        [Networked]
        public int CardId { get; set; }
    }
}
