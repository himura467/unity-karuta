using Fusion;
using R3;
using UniKaruta.Scripts.Network;

namespace UniKaruta.Scripts.Scenes.Game.Network
{
    public class GameStateAuthority : NetworkBehaviour
    {
        private const int MaxCards = 128;
        private const int LockoutDurationTicks = 30;
        private const int ReadingCueDelayTicks = 60;

        [Networked]
        public int CardCount { get; set; }

        // -1 means no phrase is active yet
        [Networked]
        public int CurrentTargetCardId { get; set; }

        // The network tick at which all clients should fire the reading cue
        [Networked]
        public int ReadingCueTargetTick { get; set; }

        [Networked, Capacity(MaxCards)]
        public NetworkArray<PlayerRef> CardOwners { get; }

        [Networked, Capacity(PlayerRegistry.MaxPlayers)]
        public NetworkArray<int> PlayerScores { get; }

        [Networked, Capacity(PlayerRegistry.MaxPlayers)]
        public NetworkArray<int> PlayerLockoutUntilTick { get; }

        private readonly Subject<int> _readingCueFired = new();
        public Observable<int> OnReadingCueFired => _readingCueFired;

        private int _lastFiredTargetCardId = -1;

        public override void Spawned()
        {
            if (Object.HasStateAuthority)
                CurrentTargetCardId = -1;
        }

        private void AdvanceToNextPhrase()
        {
            var start = CurrentTargetCardId < 0 ? 0 : (CurrentTargetCardId + 1) % CardCount;
            for (var i = 0; i < CardCount; i++)
            {
                var next = (start + i) % CardCount;
                if (CardOwners[next] == PlayerRef.None)
                {
                    CurrentTargetCardId = next;
                    ReadingCueTargetTick = Runner.Tick + ReadingCueDelayTicks;
                    return;
                }
            }
            // All cards have been taken
            CurrentTargetCardId = -1;
        }

        private void HandleCardTakeInputs()
        {
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
                    AdvanceToNextPhrase();
                }
                else
                {
                    PlayerLockoutUntilTick.Set(playerIndex, Runner.Tick + LockoutDurationTicks);
                }
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (!Runner.IsResimulation &&
                ReadingCueTargetTick > 0 &&
                Runner.Tick >= ReadingCueTargetTick &&
                CurrentTargetCardId >= 0 &&
                CurrentTargetCardId != _lastFiredTargetCardId)
            {
                _lastFiredTargetCardId = CurrentTargetCardId;
                _readingCueFired.OnNext(CurrentTargetCardId);
            }

            if (!Object.HasStateAuthority) return;

            if (CurrentTargetCardId < 0 && CardCount > 0)
                AdvanceToNextPhrase();

            HandleCardTakeInputs();
        }
    }
}
