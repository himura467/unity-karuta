using System.Threading;
using Cysharp.Threading.Tasks;

namespace UniKaruta.Framework.Scripts.Scene
{
    public abstract class AbstractSceneService : ISceneService
    {
        public virtual UniTask OnSceneCreated(CancellationToken cancelToken)
        {
            return UniTask.Yield(cancelToken);
        }

        public virtual UniTask OnSceneDestroyed(CancellationToken cancelToken)
        {
            return UniTask.Yield(cancelToken);
        }

        public virtual void Dispose()
        {
        }
    }
}
