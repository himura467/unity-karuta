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
            var cardId = -1;

            using (hierarchy.UI.OnCardPointerDown.Subscribe(id => cardId = id))
            {
                while (true)
                {
                    if (cardId >= 0)
                    {
                        service.TakeCard(cardId);
                        cardId = -1;
                    }
                    await UniTask.Yield(cancelToken);
                }
            }
        }
    }
}
