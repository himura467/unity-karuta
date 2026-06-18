using System;
using Fusion;

namespace UniKaruta.Scripts.Network
{
    public class KarutaState : NetworkBehaviour
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

        private ChangeDetector _changeDetector;

        public event Action<int> ReadingCueFired;
        public event Action<int, int> PlayerScoreChanged;
        public event Action<int, bool> PlayerPenaltyChanged;

        private int _lastFiredTargetCardId = -1;

        public override void Spawned()
        {
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            if (Object.HasStateAuthority)
            {
                CurrentTargetCardId = -1;
                ReadingCueTargetTick = -1;
            }
        }

        public override void Render()
        {
            foreach (var propertyName in _changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (propertyName)
                {
                    case nameof(PlayerScores):
                    {
                        var reader = GetArrayReader<int>(nameof(PlayerScores));
                        var previous = reader.Read(previousBuffer);
                        var current = reader.Read(currentBuffer);
                        for (var i = 0; i < current.Length; i++)
                        {
                            if (previous[i] != current[i])
                                PlayerScoreChanged?.Invoke(i, current[i]);
                        }
                        break;
                    }
                }
            }

            for (var i = 0; i < PlayerRegistry.MaxPlayers; i++)
                PlayerPenaltyChanged?.Invoke(i, Runner.Tick < PlayerLockoutUntilTick[i]);
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
                    ReadingCueFired?.Invoke(CurrentTargetCardId);
                }
            }

            if (!Object.HasStateAuthority) return;

            if (CurrentTargetCardId < 0 && CardCount > 0)
                AdvanceToNextPhrase();

            HandleCardTakeInputs();
        }
    }
}
