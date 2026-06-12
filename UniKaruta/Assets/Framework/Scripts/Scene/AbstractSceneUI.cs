using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;

namespace UniKaruta.Framework.Scripts.Scene
{
    public abstract class AbstractSceneUI : ISceneUI
    {
        protected readonly VisualElement Root;

        protected AbstractSceneUI(VisualElement root)
        {
            Root = root;
        }

        public UniTask ShowErrorIfNeedAsync(IReadOnlyList<int> errorCodes, CancellationToken cancelToken)
        {
            return UniTask.Yield(cancelToken);
        }

        public virtual void Dispose()
        {
        }
    }
}
