using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using R3;
using UniKaruta.Framework.Scripts.Scene;
using UniKaruta.Scripts.Data;
using UniKaruta.Scripts.Network;
using UniKaruta.Scripts.Scenes.Game.Network;

namespace UniKaruta.Scripts.Scenes.Game
{
    public class GameService : AbstractSceneService
    {
        private readonly NetworkRunner _runner;
        private readonly KarutaState _state;
        private readonly GameNetworkCallbacks _networkCallbacks;

        private readonly Subject<int> _readingCueFired = new();
        private readonly ReactiveProperty<int>[] _playerScores;
        private readonly ReactiveProperty<bool>[] _playerPenalties;

        public GameService(NetworkRunner runner, KarutaState state)
        {
            _runner = runner;
            _state = state;
            _networkCallbacks = new GameNetworkCallbacks(runner);
            _playerScores = new ReactiveProperty<int>[PlayerRegistry.MaxPlayers];
            _playerPenalties = new ReactiveProperty<bool>[PlayerRegistry.MaxPlayers];
            for (var i = 0; i < PlayerRegistry.MaxPlayers; i++)
            {
                _playerScores[i] = new ReactiveProperty<int>();
                _playerPenalties[i] = new ReactiveProperty<bool>();
            }
            _state.ReadingCueFired += HandleReadingCueFired;
            _state.PlayerScoreChanged += HandlePlayerScoreChanged;
            _state.PlayerPenaltyChanged += HandlePlayerPenaltyChanged;
        }

        public Observable<int> OnReadingCueFired => _readingCueFired;
        public Observable<int> GetPlayerScore(int playerId) => _playerScores[playerId];
        public Observable<bool> GetPlayerPenalty(int playerId) => _playerPenalties[playerId];

        private void HandleReadingCueFired(int cardId) => _readingCueFired.OnNext(cardId);
        private void HandlePlayerScoreChanged(int playerId, int score) => _playerScores[playerId].Value = score;
        private void HandlePlayerPenaltyChanged(int playerId, bool isInPenalty) => _playerPenalties[playerId].Value = isInPenalty;

        public void SpawnCards(NetworkObject cardPrefab, IReadOnlyList<CardData> cards)
        {
            if (!_state.Object.HasStateAuthority) return;
            for (var i = 0; i < cards.Count; i++)
            {
                var cardId = i;
                _runner.Spawn(cardPrefab, onBeforeSpawned: (runner, obj) =>
                {
                    obj.GetComponent<NetworkCard>().CardId = cardId;
                });
            }
            _state.CardCount = cards.Count;
        }

        public async UniTask WaitForCardsAsync(CancellationToken cancelToken)
        {
            await UniTask.WaitUntil(() =>
            {
                var cards = _runner.GetAllBehaviours<NetworkCard>();
                return _state.CardCount > 0 && cards.Count == _state.CardCount;
            }, cancellationToken: cancelToken);
        }

        public IEnumerable<int> GetActivePlayerIds()
        {
            foreach (var player in _runner.ActivePlayers)
                yield return player.AsIndex;
        }

        public IEnumerable<int> GetSpawnedCardIds()
        {
            foreach (var card in _runner.GetAllBehaviours<NetworkCard>())
                yield return card.CardId;
        }

        public void TakeCard(int cardId) => _networkCallbacks.SetInput(new KarutaInput { TargetCardId = cardId });

        public override void Dispose()
        {
            _state.ReadingCueFired -= HandleReadingCueFired;
            _state.PlayerScoreChanged -= HandlePlayerScoreChanged;
            _state.PlayerPenaltyChanged -= HandlePlayerPenaltyChanged;
            _readingCueFired.Dispose();
            foreach (var score in _playerScores) score.Dispose();
            foreach (var penalty in _playerPenalties) penalty.Dispose();
            _networkCallbacks.Dispose();
            base.Dispose();
        }
    }
}
