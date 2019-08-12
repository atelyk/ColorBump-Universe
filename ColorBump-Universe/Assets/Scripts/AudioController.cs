using UnityEngine;

namespace Assets.Scripts
{
	public class AudioController: MonoBehaviour
	{
		[SerializeField]
		private AudioSource _audioSource;

		public void Mute()
		{
			_audioSource.Stop();
			AudioListener.pause = true;
			Debug.Log("Mute");
		}

		public void Unmute()
		{
			AudioListener.pause = false;
			_audioSource.Play();
			Debug.Log("Unmute");
		}
	}
}
