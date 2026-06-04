using System.Linq;
using R3;
using UniKaruta.Framework.Scripts.Scene;
using UniKaruta.Scripts.UI.UIElements;
using UnityEngine.UIElements;

namespace UniKaruta.Scripts.Scenes.Game
{
    public class GameUI : AbstractSceneUI
    {
        private readonly Card[] _cards;

        public GameUI(VisualElement root) : base(root)
        {
            _cards = root.Query<Card>().ToList().ToArray();
        }

        public Observable<int> OnCardPointerDown =>
            Observable.Merge(_cards.Select(card => card.OnPointerDown.Select(_ => card.CardId)));
    }
}
