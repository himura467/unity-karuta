using R3;
using UniKaruta.Framework.Scripts.Scene;
using UniKaruta.Scripts.UI.UIElements;
using UnityEngine.UIElements;

namespace UniKaruta.Scripts.Scenes.Game
{
    public class GameUI : AbstractSceneUI
    {
        private readonly Subject<int> _cardPointerDown = new();
        private readonly CompositeDisposable _disposables = new();

        public GameUI(VisualElement root) : base(root) { }

        public Observable<int> OnCardPointerDown => _cardPointerDown;

        public void AddCard(int cardId)
        {
            var card = new Card(cardId);
            Root.Add(card);
            _disposables.Add(card.OnPointerDown.Subscribe(_ => _cardPointerDown.OnNext(cardId)));
        }

        public override void Dispose()
        {
            _disposables.Dispose();
            _cardPointerDown.Dispose();
            base.Dispose();
        }
    }
}
