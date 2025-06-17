using UnityEngine;

public class Flammable : MonoBehaviour
{
	public float heat;

	public GameObject fire;

	public GameObject simpleFire;

	private GameObject currentFire;

	private AudioSource currentFireAud;

	private Light currentFireLight;

	public bool burning;

	private bool fading;

	public bool secondary;

	private bool enemy;

	private EnemyIdentifierIdentifier eidid;

	private Flammable[] flammables;

	private void Start()
	{
		if (base.gameObject.GetComponent<EnemyIdentifierIdentifier>() != null)
		{
			enemy = true;
			eidid = base.gameObject.GetComponent<EnemyIdentifierIdentifier>();
		}
	}

	public void Burn(float newHeat)
	{
		if (GetComponent<Collider>() != null)
		{
			burning = true;
			if (newHeat > heat)
			{
				heat = newHeat;
			}
			if (currentFire == null)
			{
				if (!secondary)
				{
					currentFire = Object.Instantiate(fire);
				}
				else
				{
					currentFire = Object.Instantiate(simpleFire);
				}
				currentFire.transform.position = GetComponent<Collider>().bounds.center;
				currentFire.transform.localScale = GetComponent<Collider>().bounds.size;
				currentFire.transform.SetParent(base.transform, worldPositionStays: true);
				currentFireAud = currentFire.GetComponentInChildren<AudioSource>();
				if (!secondary && PlayerPrefs.GetInt("SimFir", 0) == 0)
				{
					currentFireLight = currentFire.GetComponent<Light>();
					currentFireLight.enabled = true;
				}
			}
			if (secondary)
			{
				return;
			}
			if (enemy)
			{
				CancelInvoke("Pulse");
				Pulse();
			}
			flammables = GetComponentsInChildren<Flammable>();
			Flammable[] array = flammables;
			foreach (Flammable flammable in array)
			{
				if (flammable != this)
				{
					flammable.secondary = true;
					flammable.Burn(heat);
					flammable.Pulse();
				}
			}
		}
		else
		{
			Object.Destroy(this);
		}
	}

	private void Pulse()
	{
		if (burning)
		{
			if (enemy && !secondary && eidid != null && eidid.eid != null)
			{
				eidid.eid.hitter = "fire";
				eidid.eid.DeliverDamage(eidid.gameObject, Vector3.zero, eidid.transform.position, 0.2f, tryForExplode: false, 0f);
			}
			heat -= 0.25f;
			if (heat <= 0f)
			{
				burning = false;
				fading = true;
			}
			Invoke("Pulse", 0.5f);
		}
		else
		{
			if (!fading || !(currentFire != null))
			{
				return;
			}
			if (currentFire != null)
			{
				currentFire.transform.localScale *= 0.75f;
				if (currentFireAud == null)
				{
					currentFireAud = currentFire.GetComponentInChildren<AudioSource>();
				}
				currentFireAud.volume *= 0.75f;
				if (!secondary && currentFireLight != null)
				{
					currentFireLight.range *= 0.75f;
				}
			}
			if (currentFire.transform.localScale.x < 0.1f)
			{
				fading = false;
				Object.Destroy(currentFire);
			}
			else
			{
				Invoke("Pulse", Random.Range(0.25f, 0.5f));
			}
		}
	}

	private void OnDestroy()
	{
		if (currentFire != null)
		{
			Object.Destroy(currentFire);
		}
	}
}
