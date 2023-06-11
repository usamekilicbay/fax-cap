using FaxCap.Manager;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FaxCap.UI.Dialog
{
    public abstract class UIDialogBase : MonoBehaviour
    {
        [SerializeField] protected TMP_Text messageText;
        [SerializeField] private Button acceptButton;
        [SerializeField] private TMP_Text acceptButtonText;
        [SerializeField] private Button rejectButton;
        [SerializeField] private TMP_Text rejectButtonText;

        protected UIManagerBase _uiManager;
        protected Action<bool> _callback;


        [Inject]
        public void Construct(UIManagerBase uiManager)
        {
            _uiManager = uiManager;
        }

        protected virtual void Awake()
        {
            acceptButton.onClick.AddListener(OnAccept);
            rejectButton.onClick.AddListener(OnReject);
        }

        public virtual void Setup(string message, string acceptText, string rejectText = "", Action<bool> callback = null)
        {
            messageText.text = message;
            acceptButtonText.text = acceptText;

            rejectButtonText.text = rejectText;
            rejectButtonText.gameObject.SetActive(!string.IsNullOrEmpty(rejectText));

            _callback = callback;
        }

        public virtual Task Show()
        {
            gameObject.SetActive(true);
            return Task.CompletedTask;
        }

        public virtual Task Hide()
        {
            gameObject.SetActive(false);
            _callback = null;
            return Task.CompletedTask;
        }

        protected virtual void OnAccept()
        {
            _callback?.Invoke(true);
            _uiManager.HideDialog(this);
        }

        protected virtual void OnReject()
        {
            _callback?.Invoke(false);
            _uiManager.HideDialog(this);
        }
    }
}
