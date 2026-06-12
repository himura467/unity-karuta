using R3;
using UniKaruta.Framework.Scripts.UI.Extensions;
using UnityEngine.UIElements;

namespace UniKaruta.Scripts.UI.UIElements
{
    public class Card : VisualElement
    {
        private const string RootClassName = "card";

        public Card()
        {
            AddToClassList(RootClassName);
        }

        public Card(int cardId) : this()
        {
            CardId = cardId;
        }

        public int CardId { get; private set; }

        public Observable<Unit> OnPointerDown => this.OnPointerDownAsObservable();

        public new class UxmlFactory : UxmlFactory<Card, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlIntAttributeDescription _cardId = new() { name = "card-id", defaultValue = -1 };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var cardId = _cardId.GetValueFromBag(bag, cc);
                var card = (Card)ve;
                card.CardId = cardId;
            }
        }
    }
}
