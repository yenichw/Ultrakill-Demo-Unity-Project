using UnityEngine;

public class CancerousRodent : MonoBehaviour
{
	private Rigidbody rb;

	private Transform player;

	private Machine mach;

	private Statue stat;

	public bool harmless;

	public GameObject[] activateOnDeath;

	public Transform shootPoint;

	public GameObject projectile;

	private float coolDown = 2f;

	public int projectileAmount;

	private int currentProjectiles;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		player = GameObject.FindWithTag("Player").transform;
		if (harmless)
		{
			mach = GetComponent<Machine>();
		}
		else
		{
			stat = GetComponent<Statue>();
		}
	}

	private void OnDisable()
	{
		if (!harmless && stat.health <= 0f)
		{
			GameObject[] array = activateOnDeath;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
		}
	}

	private void Update()
	{
		if (rb != null)
		{
			base.transform.LookAt(new Vector3(player.position.x, base.transform.position.y, player.position.z));
			rb.velocity = base.transform.forward * Time.deltaTime * 100f;
		}
		if (harmless)
		{
			if (mach.health <= 0f)
			{
				GameObject[] array = activateOnDeath;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(value: true);
				}
				Object.Destroy(GetComponentInChildren<Light>().gameObject);
				Object.Destroy(this);
			}
		}
		else if (stat.health > 0f)
		{
			if (coolDown != 0f)
			{
				coolDown = Mathf.MoveTowards(coolDown, 0f, Time.deltaTime);
			}
			else if (!Physics.Raycast(shootPoint.position, player.transform.position - shootPoint.position, Vector3.Distance(player.transform.position, shootPoint.position), stat.lmask))
			{
				coolDown = 3f;
				currentProjectiles = projectileAmount;
				FireBurst();
			}
		}
	}

	private void FireBurst()
	{
		GameObject obj = Object.Instantiate(projectile, shootPoint.position, shootPoint.rotation);
		obj.GetComponent<Rigidbody>().AddForce(shootPoint.forward * 2f, ForceMode.VelocityChange);
		obj.GetComponent<Projectile>().target = player;
		currentProjectiles--;
		if (currentProjectiles > 0)
		{
			Invoke("FireBurst", 0.1f);
		}
	}
}
