using System;
using Fusion;
using UniKaruta.Framework.Scripts.Scene;
using VContainer;

namespace UniKaruta.Scripts.Scenes.Game
{
    public class GameTransitionArgs : ISceneTransitionArgs<GameController, GameService, GameHierarchy, GameUI>
    {
        public (GameController controller, GameService service, Func<GameHierarchy> getHierarchy) GetSceneArgs(
            IObjectResolver objectResolver)
        {
            var runner = objectResolver.Resolve<NetworkRunner>();
            return (
                new GameController(),
                new GameService(runner),
                UnityEngine.Object.FindObjectOfType<GameHierarchy>
            );
        }
    }
}
