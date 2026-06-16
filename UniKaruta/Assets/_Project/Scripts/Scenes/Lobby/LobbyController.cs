using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UniKaruta.Framework.Scripts.Scene;
using UniKaruta.Scripts.Scenes.Game;

namespace UniKaruta.Scripts.Scenes.Lobby
{
    public class LobbyController : AbstractController<LobbyService, LobbyHierarchy, LobbyUI>
    {
        public override async UniTask Run(
            ISceneContext context,
            LobbyService service,
            LobbyHierarchy hierarchy,
            CancellationToken cancelToken)
        {
            var playClicked = false;

            using (hierarchy.UI.OnPlayClicked.Subscribe(_ => playClicked = true))
            {
                while (true)
                {
                    if (playClicked)
                    {
                        playClicked = false;
                        hierarchy.UI.SetPlayEnabled(false);
                        var ok = await service.StartGameAsync(cancelToken);
                        if (ok)
                        {
                            context.ChangeScene(new GameTransitionArgs());
                        }
                        else
                        {
                            hierarchy.UI.SetPlayEnabled(true);
                        }
                    }
                    await UniTask.Yield(cancelToken);
                }
            }
        }
    }
}
