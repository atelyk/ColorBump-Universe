using System;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class UIController: MonoBehaviour
	{
		[SerializeField]
		private MainMenu _mainMenu;

		[SerializeField]
		private SettingsMenu _settingsMenu;

		[SerializeField]
		private FinalScreen _finalScreen;

		[SerializeField]
		private GameOverScreen _gameOverScreen;

		// in game menu


		private GameManager _gameManager;

		private void Awake()
		{
			_gameManager = GameManager.Instance;

			OpenMainMenu();

			_mainMenu.OnPlayClick += HideMenu;
			_mainMenu.OnSettingClick += OpenSettings;

			_settingsMenu.OnBackButton += OpenMainMenu;
		}

		public void GameOver()
		{
			_gameOverScreen.gameObject.SetActive(true);
		}

		public void FinalScreen()
		{
			_finalScreen.gameObject.SetActive(true);
		}

		private void HideMenu()
		{
			_mainMenu.gameObject.SetActive(false);
			_settingsMenu.gameObject.SetActive(false);

			// show in game menu


			_gameManager.OpenStartScreen();
		}

		private void OpenSettings()
		{
			_mainMenu.gameObject.SetActive(false);
			_settingsMenu.gameObject.SetActive(true);
		}

		private void OpenMainMenu()
		{
			_mainMenu.gameObject.SetActive(true);
			_settingsMenu.gameObject.SetActive(false);
		}
	}
}
