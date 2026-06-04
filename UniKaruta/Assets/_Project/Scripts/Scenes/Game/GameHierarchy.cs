using UniKaruta.Framework.Scripts.Scene;
using UnityEngine.UIElements;

namespace UniKaruta.Scripts.Scenes.Game
{
    public class GameHierarchy : AbstractSceneHierarchy<GameUI>
    {
        protected override GameUI GetSceneUI(UIDocument uiDocument)
        {
            return new GameUI(uiDocument.rootVisualElement);
        }
    }
}
