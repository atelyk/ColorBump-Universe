using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class SettingsMenu : MonoBehaviour
	{
		public event Action OnBackButton = ()=> { };

		[SerializeField]
		private Button _backButton;

		[SerializeField]
		private ToggleGroup _toggleGroup;

		[SerializeField]
		private Toggle _onButton;

		[SerializeField]
		private Toggle _offButton;

		[SerializeField]
		private Button _privacyPolicy;

		[SerializeField]
		private Button _termOfUse;

		private bool _musicTurnOn = true;
		private bool _firstClick = true;
		private GameManager _gameManager;

		private void Awake()
		{
			_onButton.onValueChanged.AddListener(SwitchMusic);
			_offButton.onValueChanged.AddListener(SwitchMusic);
			_privacyPolicy.onClick.AddListener(PrivacyPolicyClick);
			_termOfUse.onClick.AddListener(TermOfUseClick);
			_backButton.onClick.AddListener(() => OnBackButton());
			_gameManager = GameManager.Instance;
		}

		private void PrivacyPolicyClick()
		{
			Application.OpenURL("https://policies.google.com/privacy?hl=en-US");
		}

		private void TermOfUseClick()
		{
			Application.OpenURL("https://policies.google.com/terms?hl=en-US");
		}

		private void SwitchMusic(bool arg0)
		{
			if (!arg0)	// handle only one time
			{
				return;
			}
			_musicTurnOn = !_musicTurnOn;
			_gameManager.ToggleSound(_musicTurnOn);
			if (_firstClick)
			{
				_onButton.interactable = true;
				_offButton.interactable = true;
			}
		}

		private void OnEnable()
		{
			_onButton.interactable = !_musicTurnOn; // stupid hack to avoid issue with toggle images
			_offButton.interactable = _musicTurnOn;
		}
	}
}
