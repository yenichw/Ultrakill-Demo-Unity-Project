using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Statue : MonoBehaviour
{
	public bool spawnIn;

	public GameObject spawnEffect;

	public float health;

	public GameObject bodyBlood;

	public GameObject limbBlood;

	public GameObject headBlood;

	public GameObject smallBlood;

	public GameObject skullFragment;

	public GameObject eyeBall;

	public GameObject jawHalf;

	public GameObject brainChunk;

	public GameObject[] giblet;

	private GameObject player;

	public bool limp;

	private EnemyIdentifier eid;

	public GameObject chest;

	private float chestHP;

	private AudioSource aud;

	public AudioClip[] hurtSounds;

	private StyleCalculator scalc;

	private GoreZone gz;

	public Material deadMaterial;

	private Material originalMaterial;

	public SkinnedMeshRenderer smr;

	private NavMeshAgent nma;

	private Rigidbody rb;

	private Rigidbody[] rbs;

	private Animator anim;

	public AudioClip deathSound;

	private bool noheal;

	public List<GameObject> extraDamageZones = new List<GameObject>();

	public float extraDamageMultiplier;

	private StatueBoss sb;

	public bool massDeath;

	private bool massDying;

	private Vector3 origPos;

	private List<Transform> transforms = new List<Transform>();

	public bool grounded;

	private GroundCheckEnemy gc;

	public bool knockedBack;

	private float knockBackCharge;

	public float brakes;

	public float juggleWeight;

	public bool falling;

	public LayerMask lmask;

	private float fallSpeed;

	private float fallTime;

	private bool affectedByGravity = true;

	[HideInInspector]
	public bool musicRequested;

	private void Start()
	{
		if (!limp)
		{
			player = GameObject.FindWithTag("Player");
			nma = GetComponent<NavMeshAgent>();
			rbs = GetComponentsInChildren<Rigidbody>();
			anim = GetComponentInChildren<Animator>();
			if (smr != null)
			{
				originalMaterial = smr.material;
			}
			if (spawnIn)
			{
				Object.Instantiate(position: new Vector3(base.transform.position.x, base.transform.position.y + 1.5f, base.transform.position.z), original: spawnEffect, rotation: base.transform.rotation);
			}
			gc = GetComponentInChildren<GroundCheckEnemy>();
			if (gc == null)
			{
				affectedByGravity = false;
			}
			rb = GetComponent<Rigidbody>();
			if (!musicRequested)
			{
				musicRequested = true;
				GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>().PlayBattleMusic();
			}
		}
		if (gz == null)
		{
			gz = GetComponentInParent<GoreZone>();
		}
	}

	private void Update()
	{
		if (!massDying)
		{
			return;
		}
		base.transform.position = new Vector3(origPos.x + Random.Range(-0.5f, 0.5f), origPos.y + Random.Range(-0.5f, 0.5f), origPos.z + Random.Range(-0.5f, 0.5f));
		if (Random.Range(0f, 1f) < Time.deltaTime * 5f)
		{
			int index = Random.Range(0, transforms.Count);
			if (transforms[index] != null)
			{
				Object.Instantiate(headBlood, transforms[index].position, Quaternion.identity).transform.SetParent(gz.goreZone, worldPositionStays: true);
			}
			else
			{
				transforms.RemoveAt(index);
			}
		}
	}

	private void FixedUpdate()
	{
		if (!affectedByGravity || limp)
		{
			return;
		}
		if (knockedBack && knockBackCharge <= 0f && rb.velocity.magnitude < 1f && gc.onGround)
		{
			StopKnockBack();
		}
		else if (knockedBack)
		{
			if (knockBackCharge <= 0f)
			{
				brakes = Mathf.MoveTowards(brakes, 0f, 0.0005f * brakes);
			}
			if (rb.velocity.y > 0f)
			{
				rb.velocity = new Vector3(rb.velocity.x * 0.95f * brakes, (rb.velocity.y - juggleWeight) * brakes, rb.velocity.z * 0.95f * brakes);
			}
			else
			{
				rb.velocity = new Vector3(rb.velocity.x * 0.95f * brakes, rb.velocity.y - juggleWeight, rb.velocity.z * 0.95f * brakes);
			}
			juggleWeight += 0.00025f;
			nma.updatePosition = false;
			nma.updateRotation = false;
			nma.enabled = false;
			rb.isKinematic = false;
			rb.useGravity = true;
		}
		else if (!grounded && gc.onGround)
		{
			grounded = true;
		}
		else if (grounded && !gc.onGround)
		{
			grounded = false;
		}
		if (!gc.onGround && !falling)
		{
			rb.isKinematic = false;
			rb.useGravity = true;
			nma.enabled = false;
			anim.speed = 1f;
			falling = true;
			anim.SetBool("Falling", value: true);
		}
		else
		{
			if (!gc.onGround || !falling)
			{
				return;
			}
			if (fallSpeed <= -50f)
			{
				if (eid == null)
				{
					eid = GetComponent<EnemyIdentifier>();
				}
				eid.Splatter();
				return;
			}
			fallSpeed = 0f;
			nma.updatePosition = true;
			nma.updateRotation = true;
			rb.isKinematic = true;
			rb.useGravity = false;
			nma.enabled = true;
			nma.Warp(base.transform.position);
			falling = false;
			anim.SetBool("Falling", value: false);
		}
	}

	public void KnockBack(Vector3 force)
	{
		if (affectedByGravity && sb != null && !sb.inAction)
		{
			nma.enabled = false;
			rb.isKinematic = false;
			rb.useGravity = true;
			if (!knockedBack || (!gc.onGround && rb.velocity.y < 0f))
			{
				rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
			}
			if (!gc.onGround)
			{
				rb.AddForce(Vector3.up, ForceMode.VelocityChange);
			}
			rb.AddForce(force / 10f, ForceMode.VelocityChange);
			knockedBack = true;
			knockBackCharge += force.magnitude / 1500f;
			brakes = 1f;
		}
	}

	public void StopKnockBack()
	{
		if (!(nma != null))
		{
			return;
		}
		if (Physics.Raycast(base.transform.position + Vector3.up * 0.1f, Vector3.down, out var hitInfo, float.PositiveInfinity, lmask))
		{
			_ = Vector3.zero;
			if (NavMesh.SamplePosition(hitInfo.point, out var hit, 4f, nma.areaMask))
			{
				knockedBack = false;
				nma.updatePosition = true;
				nma.updateRotation = true;
				nma.enabled = true;
				rb.isKinematic = true;
				juggleWeight = 0f;
				nma.Warp(hit.position);
			}
			else
			{
				knockBackCharge = 0.5f;
			}
		}
		else
		{
			knockBackCharge = 0.5f;
		}
	}

	public void GetHurt(GameObject target, Vector3 force, float multiplier, float critMultiplier, Vector3 hurtPos)
	{
		string hitLimb = "";
		bool dead = false;
		bool flag = false;
		float num = 0f;
		if (massDying)
		{
			return;
		}
		if (target.gameObject.tag == "Head")
		{
			if (eid == null)
			{
				eid = GetComponent<EnemyIdentifier>();
			}
			num = 1f * multiplier + multiplier * critMultiplier;
			if (extraDamageZones.Count > 0 && extraDamageZones.Contains(target))
			{
				if (num >= 1f || (eid.hitter == "shotgun" && Random.Range(0f, 1f) > 0.5f) || (eid.hitter == "nail" && Random.Range(0f, 1f) > 0.85f))
				{
					GameObject obj = Object.Instantiate(headBlood, hurtPos, Quaternion.identity);
					obj.transform.SetParent(gz.goreZone, worldPositionStays: true);
					Bloodsplatter component = obj.GetComponent<Bloodsplatter>();
					ParticleSystem component2 = component.GetComponent<ParticleSystem>();
					ParticleSystem.CollisionModule collision = component2.collision;
					if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone" || eid.hitter == "explosion")
					{
						if (Random.Range(0f, 1f) > 0.5f)
						{
							collision.enabled = false;
						}
						component.hpAmount = Mathf.RoundToInt((float)component.hpAmount * 0.25f);
					}
					component.GetComponent<SphereCollider>().radius *= 1.5f;
					if (!noheal)
					{
						component.GetReady();
					}
					component2.Play();
				}
				num *= extraDamageMultiplier;
			}
			health -= num;
			if (eid.hitter != "fire")
			{
				GameObject gameObject = null;
				gameObject = ((!(num >= 1f) && !(health <= 0f)) ? Object.Instantiate(smallBlood, hurtPos, Quaternion.identity) : Object.Instantiate(headBlood, hurtPos, Quaternion.identity));
				gameObject.transform.SetParent(gz.goreZone, worldPositionStays: true);
				Bloodsplatter component3 = gameObject.GetComponent<Bloodsplatter>();
				ParticleSystem component4 = component3.GetComponent<ParticleSystem>();
				ParticleSystem.CollisionModule collision2 = component4.collision;
				if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone" || eid.hitter == "explosion")
				{
					if (Random.Range(0f, 1f) > 0.5f)
					{
						collision2.enabled = false;
					}
					component3.hpAmount = Mathf.RoundToInt((float)component3.hpAmount * 0.25f);
				}
				if (!noheal)
				{
					component3.GetReady();
				}
				component4.Play();
			}
			_ = (player.transform.position - base.transform.position).normalized;
			if (!limp)
			{
				flag = true;
				hitLimb = "head";
			}
			if (health <= 0f && !limp)
			{
				GoLimp();
			}
		}
		else if (target.gameObject.tag == "Limb" || target.gameObject.tag == "EndLimb")
		{
			if (eid == null)
			{
				eid = GetComponent<EnemyIdentifier>();
			}
			num = 1f * multiplier + 0.5f * multiplier * critMultiplier;
			if (extraDamageZones.Count > 0 && extraDamageZones.Contains(target))
			{
				if (num >= 1f || (eid.hitter == "shotgun" && Random.Range(0f, 1f) > 0.5f) || (eid.hitter == "nail" && Random.Range(0f, 1f) > 0.85f))
				{
					GameObject obj2 = Object.Instantiate(headBlood, hurtPos, Quaternion.identity);
					obj2.transform.SetParent(gz.goreZone, worldPositionStays: true);
					Bloodsplatter component5 = obj2.GetComponent<Bloodsplatter>();
					ParticleSystem component6 = component5.GetComponent<ParticleSystem>();
					ParticleSystem.CollisionModule collision3 = component6.collision;
					if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone" || eid.hitter == "explosion")
					{
						if (Random.Range(0f, 1f) > 0.5f)
						{
							collision3.enabled = false;
						}
						component5.hpAmount = Mathf.RoundToInt((float)component5.hpAmount * 0.25f);
					}
					component5.GetComponent<SphereCollider>().radius *= 1.5f;
					if (!noheal)
					{
						component5.GetReady();
					}
					component6.Play();
				}
				num *= extraDamageMultiplier;
			}
			health -= num;
			if (eid.hitter != "fire")
			{
				GameObject gameObject2 = null;
				if (((num >= 1f || health <= 0f) && eid.hitter != "explosion") || (eid.hitter == "explosion" && target.gameObject.tag == "EndLimb"))
				{
					gameObject2 = Object.Instantiate(limbBlood, hurtPos, Quaternion.identity);
				}
				else if (eid.hitter != "explosion")
				{
					gameObject2 = Object.Instantiate(smallBlood, hurtPos, Quaternion.identity);
				}
				if (gameObject2 != null)
				{
					gameObject2.transform.SetParent(gz.goreZone, worldPositionStays: true);
					Bloodsplatter component7 = gameObject2.GetComponent<Bloodsplatter>();
					ParticleSystem component8 = component7.GetComponent<ParticleSystem>();
					ParticleSystem.CollisionModule collision4 = component8.collision;
					if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone" || eid.hitter == "explosion")
					{
						if (Random.Range(0f, 1f) > 0.5f)
						{
							collision4.enabled = false;
						}
						component7.hpAmount = Mathf.RoundToInt((float)component7.hpAmount * 0.25f);
					}
					if (!noheal)
					{
						component7.GetReady();
					}
					component8.Play();
				}
			}
			if (!limp)
			{
				flag = true;
				hitLimb = "limb";
			}
			if (health <= 0f && !limp)
			{
				GoLimp();
			}
		}
		else if (target.gameObject.tag == "Body")
		{
			num = 1f * multiplier;
			if (eid == null)
			{
				eid = GetComponent<EnemyIdentifier>();
			}
			if (eid.hitter == "shotgunzone" && (target.gameObject != chest || health - num > 0f))
			{
				num = 0f;
			}
			if (extraDamageZones.Count > 0 && extraDamageZones.Contains(target))
			{
				if (num >= 1f || (eid.hitter == "shotgun" && Random.Range(0f, 1f) > 0.5f) || (eid.hitter == "nail" && Random.Range(0f, 1f) > 0.85f))
				{
					GameObject obj3 = Object.Instantiate(headBlood, hurtPos, Quaternion.identity);
					obj3.transform.SetParent(gz.goreZone, worldPositionStays: true);
					Bloodsplatter component9 = obj3.GetComponent<Bloodsplatter>();
					ParticleSystem component10 = component9.GetComponent<ParticleSystem>();
					ParticleSystem.CollisionModule collision5 = component10.collision;
					if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone" || eid.hitter == "explosion")
					{
						if (Random.Range(0f, 1f) > 0.5f)
						{
							collision5.enabled = false;
						}
						component9.hpAmount = Mathf.RoundToInt((float)component9.hpAmount * 0.25f);
					}
					component9.GetComponent<SphereCollider>().radius *= 1.5f;
					if (!noheal)
					{
						component9.GetReady();
					}
					component10.Play();
				}
				num *= extraDamageMultiplier;
			}
			health -= num;
			if (eid.hitter != "fire")
			{
				GameObject gameObject3 = null;
				if (((num >= 1f || health <= 0f) && eid.hitter != "explosion") || (eid.hitter == "explosion" && target.gameObject.tag == "EndLimb"))
				{
					gameObject3 = Object.Instantiate(bodyBlood, hurtPos, Quaternion.identity);
				}
				else if (eid.hitter != "explosion")
				{
					gameObject3 = Object.Instantiate(smallBlood, hurtPos, Quaternion.identity);
				}
				if (gameObject3 != null)
				{
					gameObject3.transform.SetParent(gz.goreZone, worldPositionStays: true);
					Bloodsplatter component11 = gameObject3.GetComponent<Bloodsplatter>();
					ParticleSystem component12 = component11.GetComponent<ParticleSystem>();
					ParticleSystem.CollisionModule collision6 = component12.collision;
					if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone" || eid.hitter == "explosion")
					{
						if (Random.Range(0f, 1f) > 0.5f)
						{
							collision6.enabled = false;
						}
						component11.hpAmount = Mathf.RoundToInt((float)component11.hpAmount * 0.25f);
					}
					if (!noheal)
					{
						component11.GetReady();
					}
					component12.Play();
				}
			}
			_ = (player.transform.position - base.transform.position).normalized;
			if (!limp)
			{
				flag = true;
				hitLimb = "body";
			}
			if (health <= 0f)
			{
				if (!limp)
				{
					GoLimp();
				}
				if (target.GetComponentInParent<Rigidbody>() != null)
				{
					target.GetComponentInParent<Rigidbody>().AddForce(force);
				}
			}
		}
		if (health > 0f && hurtSounds.Length != 0)
		{
			if (aud == null)
			{
				aud = GetComponent<AudioSource>();
			}
			aud.clip = hurtSounds[Random.Range(0, hurtSounds.Length)];
			aud.volume = 0.75f;
			aud.pitch = Random.Range(0.85f, 1.35f);
			aud.priority = 12;
			aud.Play();
		}
		if (eid == null)
		{
			eid = GetComponent<EnemyIdentifier>();
		}
		if (flag && eid.hitter != "enemy")
		{
			if (scalc == null)
			{
				scalc = GameObject.FindWithTag("StyleHUD").GetComponent<StyleCalculator>();
			}
			if (health <= 0f)
			{
				dead = true;
			}
			if (eid.hitter != "secret")
			{
				scalc.HitCalculator(eid.hitter, "spider", hitLimb, dead, base.gameObject);
			}
		}
	}

	public void GoLimp()
	{
		gz = GetComponentInParent<GoreZone>();
		Debug.Log("Instakill Phase 1");
		Invoke("StopHealing", 1f);
		StatueBoss component = GetComponent<StatueBoss>();
		SwingCheck2[] componentsInChildren = GetComponentsInChildren<SwingCheck2>();
		if (component != null)
		{
			anim.StopPlayback();
			SwingCheck2[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				Object.Destroy(array[i]);
			}
			StatueBoss[] componentsInChildren2 = GetComponentInParent<GoreZone>().GetComponentsInChildren<StatueBoss>();
			if (componentsInChildren2.Length != 0)
			{
				StatueBoss[] array2 = componentsInChildren2;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].Enrage();
				}
			}
			if (component.currentEnrageEffect != null)
			{
				Object.Destroy(component.currentEnrageEffect);
			}
			Object.Destroy(component);
		}
		if (gz != null && gz.checkpoint != null)
		{
			gz.AddDeath();
			gz.checkpoint.sm.kills++;
		}
		else
		{
			GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>().kills++;
		}
		EnemySimplifier component2 = GetComponent<EnemySimplifier>();
		if (component2 != null)
		{
			component2.Begone();
		}
		if (smr != null)
		{
			if (deadMaterial != null)
			{
				smr.sharedMaterial = deadMaterial;
			}
			else
			{
				smr.sharedMaterial = originalMaterial;
			}
		}
		if (!massDying)
		{
			Object.Destroy(nma);
			nma = null;
			Object.Destroy(anim);
			Object.Destroy(base.gameObject.GetComponent<Collider>());
			if (rb == null)
			{
				rb = GetComponent<Rigidbody>();
			}
			Object.Destroy(rb);
			if (!limp)
			{
				rbs = GetComponentsInChildren<Rigidbody>();
				Rigidbody[] array3 = rbs;
				foreach (Rigidbody rigidbody in array3)
				{
					if (rigidbody != null)
					{
						rigidbody.isKinematic = false;
						rigidbody.useGravity = true;
					}
				}
			}
			if (!limp)
			{
				ActivateNextWave componentInParent = GetComponentInParent<ActivateNextWave>();
				if (componentInParent != null)
				{
					componentInParent.deadEnemies++;
				}
			}
			if (musicRequested)
			{
				musicRequested = false;
				GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>().PlayCleanMusic();
			}
		}
		if (deathSound != null)
		{
			if (aud == null)
			{
				aud = GetComponent<AudioSource>();
			}
			aud.clip = deathSound;
			aud.volume = 1f;
			aud.pitch = Random.Range(0.85f, 1.35f);
			aud.priority = 11;
			aud.Play();
		}
		limp = true;
		EnemyScanner componentInChildren = GetComponentInChildren<EnemyScanner>();
		if (componentInChildren != null)
		{
			Object.Destroy(componentInChildren.gameObject);
		}
	}

	private void StopHealing()
	{
		noheal = true;
	}

	private void BloodExplosion()
	{
		List<Transform> list = new List<Transform>();
		foreach (Transform transform in transforms)
		{
			if (transform != null && Random.Range(0f, 1f) < 0.33f)
			{
				Object.Instantiate(headBlood, transform.position, Quaternion.identity).transform.SetParent(gz.goreZone, worldPositionStays: true);
			}
			else if (transform == null)
			{
				list.Add(transform);
			}
		}
		if (list.Count > 0)
		{
			foreach (Transform item in list)
			{
				transforms.Remove(item);
			}
			list.Clear();
		}
		for (int i = 0; i < 40; i++)
		{
			if (i < 30)
			{
				GameObject obj = Object.Instantiate(giblet[Random.Range(0, giblet.Length)], transforms[Random.Range(0, transforms.Count)].position, Quaternion.identity);
				obj.transform.localScale *= Random.Range(4f, 7f);
				obj.transform.SetParent(gz.goreZone, worldPositionStays: true);
			}
			else if (i < 35)
			{
				GameObject obj2 = Object.Instantiate(eyeBall, transforms[Random.Range(0, transforms.Count)].position, Quaternion.identity);
				obj2.transform.localScale *= Random.Range(3f, 6f);
				obj2.transform.SetParent(gz.goreZone, worldPositionStays: true);
			}
			else
			{
				GameObject obj3 = Object.Instantiate(brainChunk, transforms[Random.Range(0, transforms.Count)].position, Quaternion.identity);
				obj3.transform.localScale *= Random.Range(3f, 4f);
				obj3.transform.SetParent(gz.goreZone, worldPositionStays: true);
			}
		}
		ActivateNextWave componentInParent = GetComponentInParent<ActivateNextWave>();
		if (componentInParent != null)
		{
			componentInParent.deadEnemies++;
		}
		if (musicRequested)
		{
			GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>().PlayCleanMusic();
		}
		base.gameObject.SetActive(value: false);
	}
}
