using Cysharp.Threading.Tasks;
using UniKaruta.Framework.Scripts.App;
using UniKaruta.Framework.Scripts.Scene;
using UniKaruta.Scripts.Scenes.Lobby;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace UniKaruta.Scripts.App
{
    public class SceneManagerRunner : IStartable
    {
        [Inject]
        private IObjectResolver _resolver;

        void IStartable.Start()
        {
            var runner = new SceneRunner();
            var context = new SceneContext(_resolver, runner);
            var args = new LobbyTransitionArgs();
            var (controller, service, getHierarchy) = args.GetSceneArgs(_resolver);

            async UniTask RunAsync()
            {
                try
                {
                    await runner.RunAsync(
                        context,
                        (c, ct) => runner.RunSceneAsync(c, controller, service, getHierarchy, false, ct),
                        default);
                }
                catch (RebootException)
                {
                    SceneManager.LoadScene("Manager");
                }
            }

            RunAsync().Forget();
        }
    }
}
