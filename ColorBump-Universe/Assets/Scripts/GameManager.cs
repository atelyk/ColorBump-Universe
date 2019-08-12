using Assets.Scripts.UI;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
	public enum GameStatus
	{
		Menu,
		WaitForStart,
		InGame,
		GameOver,
		NextLevel
	}

	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<GameManager>();
				}
				return _instance;
			}
		}
		private static GameManager _instance;

		[SerializeField] InputController _inputController;
		[SerializeField] PlayerController _playerController;
		[SerializeField] LevelMove _levelMove;
		[SerializeField] UIController _uiController;
		[SerializeField] AudioController _audioController;

		private ObstacleColor _playerColor = ObstacleColor.White;
		private GameStatus _currentGameStatus = GameStatus.Menu;

		private void Awake()
		{
			if (_instance == null)
			{
				_instance = this;
			}
			else if (_instance != this)
			{
				Destroy(gameObject);
			}

			DontDestroyOnLoad(gameObject);
		}

		private void Start()
		{
			_inputController.OnMove += _playerController.HandleMove;
			_inputController.OnSwipe += _playerController.HandleForce;
			_inputController.OnMove += StartGame;
			_inputController.OnSwipe += StartGame;
			_inputController.gameObject.SetActive(false);

			_playerController.GetToFinish += GetToFinish;
		}

		public ObstacleColor GetPlayerColor()
		{
			return _playerColor;
		}
		public float GetPlayerWidth()
		{
			return 1f;
		}

		public void CheckColor(ObstacleColor color)
		{
			if (_playerColor != color)
			{
				GameOver();
			}
		}

		public void OpenStartScreen()
		{
			_inputController.gameObject.SetActive(true);
			_currentGameStatus = GameStatus.WaitForStart;
		}

		public void ToggleSound(bool soundOn)
		{
			if (soundOn)
				_audioController.Unmute();
			else
				_audioController.Mute();
		}

		private void GameOver()
		{
			_currentGameStatus = GameStatus.GameOver;
			_uiController.GameOver();
			StartCoroutine(FinishAnimation());
			Debug.Log("Game Over");
		}

		private void GetToFinish()
		{
			if (_currentGameStatus == GameStatus.InGame)
			{
				_uiController.FinalScreen();
				_currentGameStatus = GameStatus.NextLevel;
				StartCoroutine(FinishAnimation());
			}
		}

		private void StartGame(Vector3 obj)
		{
			if (_currentGameStatus == GameStatus.WaitForStart)
			{
				_currentGameStatus = GameStatus.InGame;
				_levelMove.enabled = true;
			}
		}

		private IEnumerator FinishAnimation()
		{
			yield return new WaitForSeconds(2f);
			_levelMove.enabled = false;
			_inputController.enabled = false;
		}
	}
}
