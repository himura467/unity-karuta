using R3;
using UniKaruta.Framework.Scripts.Scene;
using UniKaruta.Framework.Scripts.UI.UIElements;
using UnityEngine.UIElements;

namespace UniKaruta.Scripts.Scenes.Lobby
{
    public class LobbyUI : AbstractSceneUI
    {
        private const string HostButtonName = "host-button";
        private const string JoinButtonName = "join-button";

        private readonly StandardButton _hostButton;
        private readonly StandardButton _joinButton;

        public LobbyUI(VisualElement root) : base(root)
        {
            _hostButton = root.Q<StandardButton>(HostButtonName);
            _joinButton = root.Q<StandardButton>(JoinButtonName);
        }

        public Observable<Unit> OnHostClicked => _hostButton.OnClicked;
        public Observable<Unit> OnJoinClicked => _joinButton.OnClicked;
    }
}
