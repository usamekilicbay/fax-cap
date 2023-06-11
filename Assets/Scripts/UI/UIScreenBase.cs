using FaxCap.Manager;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace FaxCap.UI.Screen
{
    public abstract class UIScreenBase : MonoBehaviour
    {
        protected UIManagerBase uiManager;

        [Inject]
        public void Construct(UIManagerBase mainSceneUIManager)
        {
            uiManager = mainSceneUIManager;
        }

        public virtual Task Show()
        {
            gameObject.SetActive(true);

            return Task.CompletedTask;
        }

        public virtual Task Hide()
        {
            gameObject.SetActive(false);

            return Task.CompletedTask;
        }
    }
}
