using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using R3;
using UniKaruta.Framework.Scripts.Scene;

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
            GameMode? selectedMode = null;

            using (hierarchy.UI.OnHostClicked.Subscribe(_ => selectedMode = GameMode.Host))
            using (hierarchy.UI.OnJoinClicked.Subscribe(_ => selectedMode = GameMode.Client))
            {
                while (true)
                {
                    if (selectedMode.HasValue)
                    {
                        var mode = selectedMode.Value;
                        selectedMode = null;
                        hierarchy.UI.SetButtonsEnabled(false);
                        var ok = await service.StartGameAsync(mode, cancelToken);
                        if (ok)
                        {
                            // context.ChangeScene(new GameTransitionArgs());
                        }
                        else
                        {
                            hierarchy.UI.SetButtonsEnabled(true);
                        }
                    }
                    await UniTask.Yield(cancelToken);
                }
            }
        }
    }
}
