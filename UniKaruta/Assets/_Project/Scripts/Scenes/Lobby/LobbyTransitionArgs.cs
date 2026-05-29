using System;
using Fusion;
using UniKaruta.Framework.Scripts.Scene;
using VContainer;

namespace UniKaruta.Scripts.Scenes.Lobby
{
    public class LobbyTransitionArgs : ISceneTransitionArgs<LobbyController, LobbyService, LobbyHierarchy, LobbyUI>
    {
        public (LobbyController controller, LobbyService service, Func<LobbyHierarchy> getHierarchy) GetSceneArgs(
            IObjectResolver objectResolver)
        {
            var runner = objectResolver.Resolve<NetworkRunner>();
            return (
                new LobbyController(),
                new LobbyService(runner),
                UnityEngine.Object.FindObjectOfType<LobbyHierarchy>
            );
        }
    }
}
