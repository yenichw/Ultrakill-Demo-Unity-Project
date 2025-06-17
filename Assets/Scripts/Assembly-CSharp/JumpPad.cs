using UnityEngine;

public class JumpPad : MonoBehaviour
{
	public float force;

	private float origPitch;

	private AudioSource aud;

	public AudioClip launchSound;

	public AudioClip lightLaunchSound;

	private void Start()
	{
		aud = GetComponent<AudioSource>();
		origPitch = aud.pitch;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.isStatic)
		{
			return;
		}
		float num = 1f;
		if (other.gameObject.tag == "Player")
		{
			NewMovement component = other.GetComponent<NewMovement>();
			if (component.gc.heavyFall)
			{
				num = 1.2f;
			}
			component.Launch(other.transform.position, 0f);
		}
		else if (other.gameObject.tag == "Enemy")
		{
			EnemyIdentifier component2 = other.gameObject.GetComponent<EnemyIdentifier>();
			if (component2 != null && !component2.dead)
			{
				component2.DeliverDamage(other.gameObject, Vector3.up * 10f, other.transform.position, 0f, tryForExplode: false, 0f);
			}
		}
		Rigidbody component3 = other.gameObject.GetComponent<Rigidbody>();
		if (component3 != null && !component3.isKinematic)
		{
			Vector3 velocity = component3.velocity;
			if (base.transform.up.x != 0f)
			{
				velocity.x = base.transform.up.x * force * num;
			}
			if (base.transform.up.y != 0f)
			{
				velocity.y = base.transform.up.y * force * num;
			}
			if (base.transform.up.z != 0f)
			{
				velocity.z = base.transform.up.z * force * num;
			}
			component3.velocity = velocity;
			int layer = other.gameObject.layer;
			if (layer == 14)
			{
				other.transform.LookAt(other.transform.position + component3.velocity);
			}
			if (layer == 11 || layer == 12 || layer == 2 || layer == 15)
			{
				aud.clip = launchSound;
			}
			else
			{
				aud.clip = lightLaunchSound;
			}
			aud.pitch = origPitch + Random.Range(-0.1f, 0.1f);
			aud.Play();
		}
	}
}
