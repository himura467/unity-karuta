using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UniKaruta.Framework.Scripts.Scene;

namespace UniKaruta.Scripts.Scenes.Game
{
    public class GameController : AbstractController<GameService, GameHierarchy, GameUI>
    {
        public override async UniTask OnSceneCreated(
            GameService service,
            GameHierarchy hierarchy,
            CancellationToken cancelToken)
        {
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
