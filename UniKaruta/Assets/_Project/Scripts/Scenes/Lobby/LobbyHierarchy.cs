using UniKaruta.Framework.Scripts.Scene;
using UnityEngine.UIElements;

namespace UniKaruta.Scripts.Scenes.Lobby
{
    public class LobbyHierarchy : AbstractSceneHierarchy<LobbyUI>
    {
        protected override LobbyUI GetSceneUI(UIDocument uiDocument)
        {
            return new LobbyUI(uiDocument.rootVisualElement);
        }
    }
}
