using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UniKaruta.Framework.Scripts.Scene;

namespace UniKaruta.Scripts.Scenes.Game
{
    public class GameController : AbstractController<GameService, GameHierarchy, GameUI>
    {
        private IReadOnlyList<int> _playerIds;

        public override async UniTask OnSceneCreated(
            GameService service,
            GameHierarchy hierarchy,
            CancellationToken cancelToken)
        {
            _playerIds = service.GetActivePlayerIds().ToList();
            hierarchy.UI.InitPlayerDashboards(_playerIds);

            service.SpawnCards(hierarchy.CardPrefab, hierarchy.Cards);
            await service.WaitForCardsAsync(cancelToken);
            foreach (var id in service.GetSpawnedCardIds())
                hierarchy.UI.AddCard(id);
        }

        public override async UniTask Run(
            ISceneContext context,
            GameService service,
            GameHierarchy hierarchy,
            CancellationToken cancelToken)
        {
            var pendingPhraseId = -1;
            var pendingCardId = -1;

            using (service.OnReadingCueFired.Subscribe(id => pendingPhraseId = id))
            using (service.OnPlayerScoreChanged.Subscribe(t => hierarchy.UI.UpdatePlayerScore(t.playerId, t.score)))
            using (service.OnPlayerPenaltyChanged.Subscribe(t => hierarchy.UI.UpdatePlayerPenalty(t.playerId, t.isInPenalty)))
            using (hierarchy.UI.OnCardPointerDown.Subscribe(id => pendingCardId = id))
            {
                while (true)
                {
                    if (pendingPhraseId >= 0)
                    {
                        UnityEngine.Debug.Assert(pendingPhraseId < hierarchy.Cards.Count, $"Out-of-range phrase ID: {pendingPhraseId}");
                        hierarchy.OnReadingCueFired(hierarchy.Cards[pendingPhraseId]);
                        pendingPhraseId = -1;
                    }
                    if (pendingCardId >= 0)
                    {
                        service.TakeCard(pendingCardId);
                        pendingCardId = -1;
                    }
                    await UniTask.Yield(cancelToken);
                }
            }
        }
    }
}
