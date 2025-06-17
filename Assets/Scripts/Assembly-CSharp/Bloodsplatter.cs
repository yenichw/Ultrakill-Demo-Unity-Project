using System.Collections.Generic;
using UnityEngine;

public class Bloodsplatter : MonoBehaviour
{
	public ParticleSystem part;

	public List<ParticleCollisionEvent> collisionEvents;

	public GameObject bloodStain;

	private GameObject bldstn;

	private int i;

	private AudioSource[] bloodStainAud;

	private AudioSource aud;

	[HideInInspector]
	public bool beenPlayed;

	public Material[] materials;

	private MeshRenderer mr;

	private NewMovement nmov;

	public int hpAmount;

	private SphereCollider col;

	public bool hpOnParticleCollision;

	private OptionsManager opm;

	public bool halfChance;

	public bool ready;

	private GoreZone gz;

	private void Start()
	{
		part = GetComponent<ParticleSystem>();
		collisionEvents = new List<ParticleCollisionEvent>();
		if (!beenPlayed)
		{
			aud = GetComponent<AudioSource>();
			if (aud != null)
			{
				beenPlayed = true;
				aud.pitch = Random.Range(0.75f, 1.5f);
				aud.Play();
			}
		}
		col = GetComponent<SphereCollider>();
		Invoke("DestroyCollider", 0.25f);
		if (hpOnParticleCollision)
		{
			Invoke("DestroyIt", Random.Range(2.5f, 5f));
		}
		else
		{
			Invoke("DestroyIt", Random.Range(2.5f, 5f));
		}
		if (PlayerPrefs.GetInt("BlOn", 1) == 1)
		{
			part.Play();
		}
	}

	private void OnParticleCollision(GameObject other)
	{
		if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Floor" || other.gameObject.tag == "Moving" || ((other.gameObject.tag == "Glass" || other.gameObject.tag == "GlassFloor") && other.transform.childCount > 0))
		{
			part.GetCollisionEvents(other, collisionEvents);
			if (opm == null)
			{
				opm = GameObject.FindWithTag("RoomManager").GetComponent<OptionsManager>();
			}
			if ((halfChance || !((float)Random.Range(0, 100) < opm.bloodstainChance)) && (!halfChance || !((float)Random.Range(0, 100) < opm.bloodstainChance / 2f)))
			{
				return;
			}
			bldstn = Object.Instantiate(bloodStain, collisionEvents[0].intersection, base.transform.rotation, base.transform);
			bldstn.transform.forward = collisionEvents[0].normal * -1f;
			mr = bldstn.GetComponent<MeshRenderer>();
			mr.material = materials[Random.Range(0, materials.Length - 1)];
			bldstn.transform.Rotate(Vector3.forward * Random.Range(0, 359), Space.Self);
			_ = bldstn.transform.localScale;
			if (other.gameObject.tag == "Moving")
			{
				bldstn.transform.SetParent(other.transform, worldPositionStays: true);
				if (gz == null)
				{
					gz = base.transform.GetComponentInParent<GoreZone>();
				}
				if (gz == null)
				{
					gz = Object.FindObjectOfType<GoreZone>();
				}
				if (gz != null)
				{
					gz.outsideGore.Add(bldstn);
				}
			}
			else if (other.gameObject.tag == "Glass" || other.gameObject.tag == "GlassFloor")
			{
				bldstn.transform.SetParent(other.transform, worldPositionStays: true);
			}
			else
			{
				bldstn.transform.SetParent(base.transform.parent, worldPositionStays: true);
			}
		}
		else if (ready && hpOnParticleCollision && other.gameObject.tag == "Player")
		{
			if (nmov == null)
			{
				nmov = other.GetComponent<NewMovement>();
			}
			nmov.GetHealth(3, silent: false);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (ready && other.gameObject.tag == "Player")
		{
			Object.Destroy(col);
			if (nmov == null)
			{
				nmov = other.GetComponent<NewMovement>();
			}
			nmov.GetHealth(hpAmount, silent: false);
		}
	}

	private void DestroyIt()
	{
		Object.Destroy(base.gameObject);
	}

	private void DestroyCollider()
	{
		if (col != null)
		{
			Object.Destroy(col);
		}
	}

	public void GetReady()
	{
		ready = true;
	}
}
