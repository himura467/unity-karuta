using R3;
using UniKaruta.Framework.Scripts.Scene;
using UniKaruta.Framework.Scripts.UI.UIElements;
using UnityEngine.UIElements;

namespace UniKaruta.Scripts.Scenes.Lobby
{
    public class LobbyUI : AbstractSceneUI
    {
        private const string PlayButtonName = "play-button";

        private readonly StandardButton _playButton;

        public LobbyUI(VisualElement root)
        {
            _playButton = root.Q<StandardButton>(PlayButtonName);
        }

        public Observable<Unit> OnPlayClicked => _playButton.OnClicked;

        public void SetPlayEnabled(bool enabled) => _playButton.SetEnabled(enabled);
    }
}
