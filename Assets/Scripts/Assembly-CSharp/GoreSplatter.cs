using UnityEngine;

public class GoreSplatter : MonoBehaviour
{
	private Rigidbody rb;

	private Vector3 direction;

	private float force;

	private bool goreOver;

	private int touchedCollisions;

	private Vector3 defaultScale;

	private bool freezeGore;

	private bool foundParent;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		defaultScale = base.transform.localScale;
		direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		force = Random.Range(20, 60);
		rb.AddForce(direction * force, ForceMode.VelocityChange);
		base.transform.rotation = Random.rotation;
		if (PlayerPrefs.GetInt("FreGor", 0) == 1)
		{
			freezeGore = true;
		}
		Invoke("SlowUpdate", 1f);
		if (freezeGore)
		{
			Invoke("ReadyToStopGore", 5f);
		}
	}

	private void SlowUpdate()
	{
		if (freezeGore && goreOver && rb.velocity.y > -0.1f && rb.velocity.y < 0.1f)
		{
			StopGore();
		}
		if (base.transform.position.y < -300f || base.transform.position.x < -1000f || base.transform.position.x > 1000f || base.transform.position.z < -1000f || base.transform.position.z > 1000f)
		{
			Object.Destroy(base.gameObject);
		}
		Invoke("SlowUpdate", 1f);
	}

	private void OnCollisionEnter(Collision other)
	{
		if (freezeGore && (other.gameObject.layer == 8 || other.gameObject.layer == 24) && (other.gameObject.tag == "Wall" || other.gameObject.tag == "Floor"))
		{
			touchedCollisions++;
			goreOver = true;
		}
	}

	private void OnCollisionExit(Collision other)
	{
		if (freezeGore && (other.gameObject.layer == 8 || other.gameObject.layer == 24) && (other.gameObject.tag == "Wall" || other.gameObject.tag == "Floor"))
		{
			touchedCollisions--;
			if (touchedCollisions <= 0)
			{
				goreOver = false;
			}
		}
	}

	private void ReadyToStopGore()
	{
		if (freezeGore && !goreOver)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void StopGore()
	{
		Object.Destroy(rb);
		Object.Destroy(base.gameObject.GetComponent<Collider>());
		Object.Destroy(this);
	}
}
