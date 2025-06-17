using UnityEngine;

public class RandomForce : MonoBehaviour
{
	public float force;

	private void Start()
	{
		GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * force, ForceMode.VelocityChange);
	}
}
