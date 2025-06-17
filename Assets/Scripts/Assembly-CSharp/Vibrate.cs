using UnityEngine;

public class Vibrate : MonoBehaviour
{
	public float intensity;

	private Vector3 origPos;

	private void Start()
	{
		origPos = base.transform.localPosition;
	}

	private void Update()
	{
		base.transform.localPosition = new Vector3(origPos.x + Random.Range(0f - intensity, intensity), origPos.y + Random.Range(0f - intensity, intensity), origPos.z + Random.Range(0f - intensity, intensity));
	}
}
