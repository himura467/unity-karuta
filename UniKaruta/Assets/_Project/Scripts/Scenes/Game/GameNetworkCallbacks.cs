using Fusion;
using UniKaruta.Scripts.Network;

namespace UniKaruta.Scripts.Scenes.Game
{
    public class GameNetworkCallbacks : NetworkRunnerCallbacks
    {
        private KarutaInput? _pendingInput;

        public GameNetworkCallbacks(NetworkRunner runner) : base(runner) { }

        public void SetInput(KarutaInput input) => _pendingInput = input;

        public override void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (_pendingInput is not { } pending) return;
            input.Set(pending);
            _pendingInput = null;
        }
    }
}
