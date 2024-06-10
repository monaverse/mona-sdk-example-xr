using System;
using System.Collections;
using System.Threading.Tasks;
using Monaverse.Core;
using Monaverse.Modal.UI.Components;
using TMPro;
using UnityEngine;

namespace Monaverse.Modal.UI.Views
{
    public class ConnectMagicLinkWalletView : MonaModalView
    {
        [SerializeField] private TMP_Text _connectStatusText;
        [SerializeField] private MonaModalView _authorizeWalletView;

        [SerializeField] private TMP_InputField _email;

        private MonaWalletConnection _options;
        private string _walletName;

        public override void Show(MonaModal modal, IEnumerator effectCoroutine, object options = null)
        {
            if (options == null)
            {
                modal.CloseView();
                return;
            }
            
            base.Show(modal, effectCoroutine, options);
        }

        protected override async void OnOpened(object options = null)
        {
            _options = (MonaWalletConnection)options;
            _connectStatusText.text = $"Please Login...";
            await Task.CompletedTask;
        }

        public async void Login()
        {
            _options.Email = _email.text;

            _connectStatusText.text = $"Connecting to {_walletName}...";
            await ConnectWallet(_options);
        }

        private async Task ConnectWallet(MonaWalletConnection options)
        {
            try
            {
                _options = (MonaWalletConnection)options;

                _walletName = _options.MonaWalletProvider switch
                {
                    MonaWalletProvider.WalletConnect => "WalletConnect",
                    MonaWalletProvider.LocalWallet => "Local Wallet",
                    _ => throw new ArgumentOutOfRangeException("Unexpected WalletProvider: " + options.MonaWalletProvider)
                };

                parentModal.Header.Title = _walletName;

                await MonaverseManager.Instance.SDK.ConnectWallet(options);
                parentModal.OpenView(_authorizeWalletView);
            }
            catch (Exception exception)
            {
                parentModal.CloseView();
                parentModal.Header.Snackbar.Show(MonaSnackbar.Type.Error, "Connection Aborted");
                MonaDebug.LogError("[ConnectingWalletView] ConnectWallet Exception: " + exception.Message);
            }
        }
    }
}