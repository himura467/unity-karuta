using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using UniKaruta.Framework.Scripts.Scene;

namespace UniKaruta.Scripts.Scenes.Lobby
{
    public class LobbyService : AbstractSceneService
    {
        private readonly NetworkRunner _runner;

        public LobbyService(NetworkRunner runner)
        {
            _runner = runner;
        }

        public async UniTask<bool> StartGameAsync(CancellationToken cancelToken)
        {
            var result = await _runner.StartGame(new StartGameArgs { GameMode = GameMode.Shared });
            return result.Ok;
        }
    }
}
