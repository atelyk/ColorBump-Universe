using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class MainMenu: MonoBehaviour
	{
		public event Action OnPlayClick = () => { };
		public event Action OnSettingClick = () => { };

		[SerializeField]
		private Button PlayButton;

		[SerializeField]
		private Button SettingsButton;

		private void Awake()
		{
			PlayButton.onClick.AddListener(() => OnPlayClick());
			SettingsButton.onClick.AddListener(() => OnSettingClick());
		}
	}
}
