using FaxCap.UI.Dialog;
using FaxCap.UI.Screen;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace FaxCap.Manager
{
    public abstract class UIManagerBase : MonoBehaviour
    {
        [SerializeField] private UIScreenBase firstScreen;

        private UIScreenBase _activeScreen;
        private List<UIDialogBase> _activeDialogs = new();

        private void Awake()
        {
            ShowScreen(firstScreen);
        }

        public async void ShowScreen(UIScreenBase screen)
        {
            if (_activeScreen != null)
            {
                await _activeScreen.Hide();
            }

            await screen.Show();
            _activeScreen = screen;
        }

        public async void ShowDialog(UIDialogBase dialog, bool hideRest = true)
        {
            if (hideRest)
            {
                await HideAllDialogs();
            }

            dialog.transform.SetAsLastSibling();
            await dialog.Show();
            _activeDialogs.Add(dialog);
        }

        public async void HideDialog(UIDialogBase dialog)
        {
            await dialog.Hide();

            if (!_activeDialogs.Contains(dialog))
            {
                Debug.LogWarning($"Dialog {dialog} is not registered as active in {name}", this);
            }
            else
            {
                _activeDialogs.Remove(dialog);
            }
        }

        public async Task HideAllDialogs()
        {
            var tasks = new Task[_activeDialogs.Count];

            for (var i = 0; i < _activeDialogs.Count; i++)
            {
                tasks[i] = _activeDialogs[i].Hide();
            }

            await Task.WhenAll(tasks);
            _activeDialogs.Clear();
        }

        public UIScreenBase GetActiveUIScreen()
            => _activeScreen;
    }
}
