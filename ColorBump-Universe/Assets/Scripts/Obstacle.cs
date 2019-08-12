using UnityEngine;

namespace Assets.Scripts
{
	public enum ObstacleColor
	{
		White,
		Blue,
		Violet,
		Orange
	}

	public class Obstacle: MonoBehaviour
	{
		private ObstacleColor _color;
		private GameManager _gameManager;
		private Renderer _renderer;
		private Mesh _mesh;

		private void Awake()
		{
			_gameManager = GameManager.Instance;
			_renderer = GetComponent<Renderer>();
			_mesh = GetComponent<MeshFilter>().mesh;
		}

		public void Init(Material material, ObstacleColor color, Vector3 size)
		{
			Init(material, color);
			transform.localScale = size;
		}

		public void Init(Material material, ObstacleColor color)
		{
			_renderer.material = material;
			_color = color;
		}

		public float GetXSize()
		{
			return _mesh.bounds.size.x * transform.localScale.x;
		}

		public float GetYSize()
		{
			return _mesh.bounds.size.y * transform.localScale.y;
		}

		public void SetPosition(Vector3 position)
		{
			transform.position = position;
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (collision.gameObject.tag == "Player")
			{
				Debug.Log("Touch");
				_gameManager.CheckColor(_color);
			}
		}
	}
}
