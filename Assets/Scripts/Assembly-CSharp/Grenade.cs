using UnityEngine;

public class Grenade : MonoBehaviour
{
	public string hitterWeapon;

	public GameObject explosion;

	private bool exploded;

	public bool enemy;

	private Rigidbody rb;

	public GameObject ineffectiveSound;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (exploded)
		{
			return;
		}
		bool flag = false;
		if (collision.gameObject.layer == 11)
		{
			EnemyIdentifier eid = collision.gameObject.GetComponent<EnemyIdentifierIdentifier>().eid;
			if (eid != null && !eid.dead && eid.type == EnemyType.Spider)
			{
				flag = true;
			}
		}
		if (!flag && collision.gameObject.tag == "Armor")
		{
			flag = true;
		}
		if (flag)
		{
			LayerMask layerMask = 256;
			layerMask = (int)layerMask | 0x1000000;
			if (rb == null)
			{
				rb = GetComponent<Rigidbody>();
			}
			if (Physics.Raycast(base.transform.position - rb.velocity.normalized, base.transform.forward, out var hitInfo, float.PositiveInfinity, layerMask))
			{
				Vector3 velocity = rb.velocity;
				rb.velocity = Vector3.zero;
				rb.AddForce(Vector3.Reflect(velocity.normalized, hitInfo.normal) * velocity.magnitude * 2f, ForceMode.VelocityChange);
			}
			Object.Instantiate(ineffectiveSound, base.transform.position, Quaternion.identity).GetComponent<AudioSource>().volume = 0.75f;
			return;
		}
		exploded = true;
		Explosion[] componentsInChildren = Object.Instantiate(this.explosion, base.transform.position, Quaternion.identity).GetComponentsInChildren<Explosion>();
		foreach (Explosion explosion in componentsInChildren)
		{
			explosion.hitterWeapon = hitterWeapon;
			if (enemy)
			{
				explosion.enemy = true;
			}
		}
		if (collision.gameObject.layer != 8)
		{
			GameObject.FindWithTag("MainCamera").GetComponent<CameraController>().HitStop(0.05f);
		}
		Object.Destroy(base.gameObject);
	}
}
