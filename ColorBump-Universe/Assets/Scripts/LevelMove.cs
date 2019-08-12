using UnityEngine;

namespace Assets.Scripts
{
	public class LevelMove : MonoBehaviour
	{
		[SerializeField] private float _speed = 1f;
		[SerializeField] private Vector3 _initialPosition;

		private void Awake()
		{
			this.enabled = false;
		}

		private void Update()
		{
			transform.position += new Vector3(0, 0, -_speed * Time.deltaTime);
		}

		public void ResetLevel()
		{
			transform.position = _initialPosition;
			this.enabled = false;
		}
	}
}
