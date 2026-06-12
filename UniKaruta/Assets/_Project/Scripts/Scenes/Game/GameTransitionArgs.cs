using System;
using Fusion;
using UniKaruta.Framework.Scripts.Scene;
using UniKaruta.Scripts.Scenes.Game.Network;
using VContainer;

namespace UniKaruta.Scripts.Scenes.Game
{
    public class GameTransitionArgs : ISceneTransitionArgs<GameController, GameService, GameHierarchy, GameUI>
    {
        public (GameController controller, GameService service, Func<GameHierarchy> getHierarchy) GetSceneArgs(
            IObjectResolver objectResolver)
        {
            var runner = objectResolver.Resolve<NetworkRunner>();
            var authority = objectResolver.Resolve<GameStateAuthority>();
            return (
                new GameController(),
                new GameService(runner, authority),
                UnityEngine.Object.FindObjectOfType<GameHierarchy>
            );
        }
    }
}
