using R3;
using UniKaruta.Framework.Scripts.Scene;
using UniKaruta.Scripts.UI.UIElements;
using UnityEngine.UIElements;

namespace UniKaruta.Scripts.Scenes.Game
{
    public class GameUI : AbstractSceneUI
    {
        private const string ReadingCueLabelName = "reading-cue";
        private const string CardFieldName = "card-field";

        private readonly Label _readingCueLabel;
        private readonly VisualElement _cardField;

        private readonly Subject<int> _cardPointerDown = new();
        private readonly CompositeDisposable _disposables = new();

        public GameUI(VisualElement root)
        {
            _readingCueLabel = root.Q<Label>(ReadingCueLabelName);
            _cardField = root.Q<VisualElement>(CardFieldName);
        }

        public Observable<int> OnCardPointerDown => _cardPointerDown;

        public void OnReadingCueFired(string phraseText) => _readingCueLabel.text = phraseText;

        public void AddCard(int cardId)
        {
            var card = new Card(cardId);
            _cardField.Add(card);
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
