using System;
using Fusion;
using UniKaruta.Framework.Scripts.Scene;
using UniKaruta.Scripts.Network;
using VContainer;

namespace UniKaruta.Scripts.Scenes.Game
{
    public class GameTransitionArgs : ISceneTransitionArgs<GameController, GameService, GameHierarchy, GameUI>
    {
        public (GameController controller, GameService service, Func<GameHierarchy> getHierarchy) GetSceneArgs(
            IObjectResolver objectResolver)
        {
            var runner = objectResolver.Resolve<NetworkRunner>();
            var state = objectResolver.Resolve<KarutaState>();
            return (
                new GameController(),
                new GameService(runner, state),
                UnityEngine.Object.FindObjectOfType<GameHierarchy>
            );
        }
    }
}
