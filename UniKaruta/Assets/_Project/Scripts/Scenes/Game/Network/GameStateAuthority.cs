using Fusion;
using UniKaruta.Scripts.Network;

namespace UniKaruta.Scripts.Scenes.Game.Network
{
    public class GameStateAuthority : NetworkBehaviour
    {
        private const int MaxCards = 128;
        private const int LockoutDurationTicks = 30;

        [Networked]
        public int CardCount { get; set; }

        [Networked]
        public int CurrentTargetCardId { get; set; }

        [Networked, Capacity(MaxCards)]
        public NetworkArray<PlayerRef> CardOwners { get; }

        [Networked, Capacity(PlayerRegistry.MaxPlayers)]
        public NetworkArray<int> PlayerScores { get; }

        [Networked, Capacity(PlayerRegistry.MaxPlayers)]
        public NetworkArray<int> PlayerLockoutUntilTick { get; }

        public override void FixedUpdateNetwork()
        {
            if (!Object.HasStateAuthority) return;

            foreach (var player in Runner.ActivePlayers)
            {
                if (!Runner.TryGetInputForPlayer(player, out KarutaInput input)) continue;

                var playerIndex = player.AsIndex;
                if (playerIndex >= PlayerRegistry.MaxPlayers)
                {
                    UnityEngine.Debug.LogError($"Player {player} has out-of-range index {playerIndex}");
                    continue;
                }
                if (Runner.Tick < PlayerLockoutUntilTick[playerIndex]) continue;

                var cardId = input.TargetCardId;
                if (cardId < 0 || cardId >= MaxCards)
                {
                    UnityEngine.Debug.LogError($"Player {playerIndex} submitted out-of-range cardId {cardId}");
                    continue;
                }
                if (CardOwners[cardId] != PlayerRef.None) continue;

                if (cardId == CurrentTargetCardId)
                {
                    CardOwners.Set(cardId, player);
                    PlayerScores.Set(playerIndex, PlayerScores[playerIndex] + 1);
                }
                else
                {
                    PlayerLockoutUntilTick.Set(playerIndex, Runner.Tick + LockoutDurationTicks);
                }
            }
        }
    }
}
