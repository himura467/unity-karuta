using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UniKaruta.Framework.Scripts.Scene
{
    public abstract class AbstractSceneUI : ISceneUI
    {
        public UniTask ShowErrorIfNeedAsync(IReadOnlyList<int> errorCodes, CancellationToken cancelToken)
        {
            return UniTask.Yield(cancelToken);
        }

        public virtual void Dispose()
        {
        }
    }
}
