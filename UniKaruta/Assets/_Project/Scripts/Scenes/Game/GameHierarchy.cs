using Fusion;
using UniKaruta.Framework.Scripts.Scene;
using UniKaruta.Scripts.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniKaruta.Scripts.Scenes.Game
{
    public class GameHierarchy : AbstractSceneHierarchy<GameUI>
    {
        [SerializeField]
        private NetworkObject _cardPrefab;
        [SerializeField]
        private CardDatabase _cardDatabase;

        public NetworkObject CardPrefab => _cardPrefab;
        public CardData[] Cards => _cardDatabase.Cards;

        protected override GameUI GetSceneUI(UIDocument uiDocument)
        {
            return new GameUI(uiDocument.rootVisualElement);
        }
    }
}
