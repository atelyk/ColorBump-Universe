using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
	public class InputController : MonoBehaviour	// todo: work with mouse input
	{
		public event Action<Vector3> OnMove;
		public event Action<Vector3> OnSwipe;

		[SerializeField]
		private float _moveSpeed = 0.035f;	//800x600 - speed must depend on dpi
		private bool _startInput = false;
		private Vector2 _lastInputPosition;
		private float _swipeSpeed = 1f;
		private const float _forseLimit = 10f;
		private const float _speedLimit = 1f;
		private const float _xBorderLimit = 4.5f;

		private const float BASE_HEIGHT = 800f;
		private float _adaptiveMoveSpeed;
		private float _adaptiveSwipeSpeed;

		private void Start()
		{
			Input.multiTouchEnabled = false;
			_adaptiveMoveSpeed = _moveSpeed * BASE_HEIGHT/ Screen.height;
			_adaptiveSwipeSpeed = _swipeSpeed * BASE_HEIGHT / Screen.height;
		}

		private void FixedUpdate()
		{

			if (Input.touches.Any())
			{

				var currentTouch = Input.touches[0];
				var currentInputPosition = currentTouch.position;
				if (!_startInput)
				{
					_startInput = true;
					_lastInputPosition = currentInputPosition;
				}
				else
				{
					var vector = currentInputPosition - _lastInputPosition;
					_lastInputPosition = currentInputPosition;
					if (currentTouch.phase == TouchPhase.Ended)
					{
						var move = _adaptiveSwipeSpeed * new Vector3(vector.x, 0, vector.y);
						OnSwipe(move);
						Debug.Log("Swipe");
					}
					else
					{
						var move = _adaptiveMoveSpeed * new Vector3(vector.x, 0, vector.y);
						OnMove(move);
					}
				}
			}
			else
			{
				_startInput = false;
			}
		}
	}
}