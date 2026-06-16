using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using R3;
using UniKaruta.Framework.Scripts.Scene;
using UniKaruta.Scripts.Data;
using UniKaruta.Scripts.Scenes.Game.Network;

namespace UniKaruta.Scripts.Scenes.Game
{
    public class GameService : AbstractSceneService
    {
        private readonly NetworkRunner _runner;
        private readonly GameStateAuthority _authority;
        private readonly GameNetworkCallbacks _networkCallbacks;

        public GameService(NetworkRunner runner, GameStateAuthority authority)
        {
            _runner = runner;
            _authority = authority;
            _networkCallbacks = new GameNetworkCallbacks(runner);
        }

        public Observable<int> OnReadingCueFired => _authority.OnReadingCueFired;
        public Observable<int> GetPlayerScore(int playerId) => _authority.GetPlayerScore(playerId);
        public Observable<bool> GetPlayerPenalty(int playerId) => _authority.GetPlayerPenalty(playerId);

        public void SpawnCards(NetworkObject cardPrefab, IReadOnlyList<CardData> cards)
        {
            if (!_runner.IsServer) return;
            for (var i = 0; i < cards.Count; i++)
            {
                var cardId = i;
                _runner.Spawn(cardPrefab, onBeforeSpawned: (runner, obj) =>
                {
                    obj.GetComponent<NetworkCard>().CardId = cardId;
                });
            }
            _authority.CardCount = cards.Count;
        }

        public async UniTask WaitForCardsAsync(CancellationToken cancelToken)
        {
            await UniTask.WaitUntil(() =>
            {
                var cards = _runner.GetAllBehaviours<NetworkCard>();
                return _authority.CardCount > 0 && cards.Count == _authority.CardCount;
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
            _networkCallbacks.Dispose();
            base.Dispose();
        }
    }
}
