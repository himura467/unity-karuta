using Fusion;
using UniKaruta.Framework.Scripts.Scene;
using UniKaruta.Scripts.Network;

namespace UniKaruta.Scripts.Scenes.Game
{
    public class GameService : AbstractSceneService
    {
        private readonly GameNetworkCallbacks _networkCallbacks;

        public GameService(NetworkRunner runner)
        {
            _networkCallbacks = new GameNetworkCallbacks(runner);
        }

        public void TakeCard(int cardId) => _networkCallbacks.SetInput(new KarutaInput { TargetCardId = cardId });

        public override void Dispose()
        {
            _networkCallbacks.Dispose();
            base.Dispose();
        }
    }
}
