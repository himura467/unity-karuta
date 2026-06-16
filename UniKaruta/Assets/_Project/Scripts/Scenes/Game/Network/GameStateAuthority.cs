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
        private readonly Subject<(int playerId, int score)> _playerScoreChanged = new();
        private readonly Subject<(int playerId, bool isInPenalty)> _playerPenaltyChanged = new();

        public Observable<int> OnReadingCueFired => _readingCueFired;
        public Observable<(int playerId, int score)> OnPlayerScoreChanged => _playerScoreChanged;
        public Observable<(int playerId, bool isInPenalty)> OnPlayerPenaltyChanged => _playerPenaltyChanged;

        private int _lastFiredTargetCardId = -1;
        private int[] _lastScores;
        private bool[] _lastPenaltyStates;

        public override void Spawned()
        {
            _lastScores = new int[PlayerScores.Length];
            _lastPenaltyStates = new bool[PlayerScores.Length];

            if (Object.HasStateAuthority)
            {
                CurrentTargetCardId = -1;
                ReadingCueTargetTick = -1;
            }
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
            if (!Runner.IsResimulation)
            {
                if (ReadingCueTargetTick >= 0 &&
                    Runner.Tick >= ReadingCueTargetTick &&
                    CurrentTargetCardId >= 0 &&
                    CurrentTargetCardId != _lastFiredTargetCardId)
                {
                    _lastFiredTargetCardId = CurrentTargetCardId;
                    _readingCueFired.OnNext(CurrentTargetCardId);
                }

                foreach (var player in Runner.ActivePlayers)
                {
                    var i = player.AsIndex;
                    var score = PlayerScores[i];
                    if (score != _lastScores[i])
                    {
                        _lastScores[i] = score;
                        _playerScoreChanged.OnNext((i, score));
                    }
                    var isInPenalty = Runner.Tick < PlayerLockoutUntilTick[i];
                    if (isInPenalty != _lastPenaltyStates[i])
                    {
                        _lastPenaltyStates[i] = isInPenalty;
                        _playerPenaltyChanged.OnNext((i, isInPenalty));
                    }
                }
            }

            if (!Object.HasStateAuthority) return;

            if (CurrentTargetCardId < 0 && CardCount > 0)
                AdvanceToNextPhrase();

            HandleCardTakeInputs();
        }
    }
}
