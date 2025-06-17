using UnityEngine;

public class MusicFadeOut : MonoBehaviour
{
	private void Start()
	{
		Object.FindObjectOfType<MusicManager>().off = true;
		Object.Destroy(this);
	}
}
