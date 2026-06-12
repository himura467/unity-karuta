using Fusion;
using UniKaruta.Scripts.Scenes.Game.Network;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UniKaruta.Scripts.App
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private NetworkRunner _runnerPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInNewPrefab(_runnerPrefab, Lifetime.Singleton);
            builder.RegisterComponentInHierarchy<GameStateAuthority>();
            builder.RegisterEntryPoint<SceneManagerRunner>();
        }
    }
}
