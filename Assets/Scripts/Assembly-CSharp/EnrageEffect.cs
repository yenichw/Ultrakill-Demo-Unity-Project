using UnityEngine;

public class EnrageEffect : MonoBehaviour
{
	public GameObject endSound;

	private void Start()
	{
		GameObject.FindWithTag("Player").GetComponentInChildren<StyleHUD>().AddPoints(250, "<color=red>ENRAGED</color>");
		GameObject.FindWithTag("MainCamera").GetComponent<CameraController>().CameraShake(1f);
	}

	private void OnDestroy()
	{
		Object.Instantiate(endSound, base.transform.position, base.transform.rotation);
	}
}
