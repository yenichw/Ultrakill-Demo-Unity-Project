using UnityEngine;

public class MusicDeactivator : MonoBehaviour
{
	public bool oneTime;

	private void OnEnable()
	{
		Object.FindObjectOfType<MusicManager>().StopMusic();
		if (oneTime)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
