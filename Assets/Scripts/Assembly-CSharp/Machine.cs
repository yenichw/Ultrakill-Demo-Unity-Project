using UnityEngine;
using UnityEngine.AI;

public class Machine : MonoBehaviour
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

	private float chestHP = 3f;

	private AudioSource aud;

	public AudioClip[] hurtSounds;

	public StyleCalculator scalc;

	private GoreZone gz;

	public Material deadMaterial;

	private Material originalMaterial;

	public SkinnedMeshRenderer smr;

	private NavMeshAgent nma;

	private Rigidbody rb;

	private Rigidbody[] rbs;

	private Animator anim;

	public AudioClip deathSound;

	public AudioClip scream;

	private bool noheal;

	public bool bigKill;

	public bool parryable;

	private SwordsMachine sm;

	public GameObject[] destroyOnDeath;

	public Machine symbiote;

	private bool symbiotic;

	private bool healing;

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

	private float reduceFallTime;

	public bool noFallDamage;

	public bool dontDie;

	[HideInInspector]
	public bool musicRequested;

	private void Start()
	{
		player = GameObject.FindWithTag("Player");
		nma = GetComponent<NavMeshAgent>();
		rbs = GetComponentsInChildren<Rigidbody>();
		anim = GetComponentInChildren<Animator>();
		eid = GetComponent<EnemyIdentifier>();
		if (smr != null)
		{
			originalMaterial = smr.material;
		}
		if (spawnIn)
		{
			spawnIn = false;
			Object.Instantiate(position: new Vector3(base.transform.position.x, base.transform.position.y + 1.5f, base.transform.position.z), original: spawnEffect, rotation: base.transform.rotation);
		}
		sm = GetComponent<SwordsMachine>();
		if (symbiote != null)
		{
			symbiotic = true;
		}
		gc = GetComponentInChildren<GroundCheckEnemy>();
		rb = GetComponent<Rigidbody>();
		if (gz == null)
		{
			gz = GetComponentInParent<GoreZone>();
		}
	}

	private void Update()
	{
		if (knockBackCharge > 0f)
		{
			knockBackCharge = Mathf.MoveTowards(knockBackCharge, 0f, Time.deltaTime);
		}
		if (healing && !limp && symbiotic && symbiote != null)
		{
			health = Mathf.MoveTowards(health, symbiote.health, Time.deltaTime * 10f);
			if (health >= symbiote.health)
			{
				healing = false;
				sm.downed = false;
			}
		}
		if (falling && !noFallDamage && rb != null)
		{
			fallTime += Time.deltaTime;
			if (gc.onGround && falling && nma != null)
			{
				if (fallSpeed <= -60f)
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
				if (sm == null)
				{
					rb.isKinematic = true;
				}
				if (aud == null)
				{
					aud = GetComponent<AudioSource>();
				}
				if (aud.clip == scream && aud.isPlaying)
				{
					aud.Stop();
				}
				rb.useGravity = false;
				nma.enabled = true;
				nma.Warp(base.transform.position);
				falling = false;
				anim.SetBool("Falling", value: false);
			}
			else if (fallTime > 0.05f && rb.velocity.y < fallSpeed)
			{
				fallSpeed = rb.velocity.y;
				reduceFallTime = 0.5f;
				if (aud == null)
				{
					aud = GetComponent<AudioSource>();
				}
				if (!aud.isPlaying && !limp && (!Physics.Raycast(base.transform.position, Vector3.down, out var _, 28f, lmask) || rb.velocity.y < -50f))
				{
					aud.clip = scream;
					aud.volume = 1f;
					aud.priority = 78;
					aud.pitch = Random.Range(0.8f, 1.2f);
					aud.Play();
				}
			}
			else if (fallTime > 0.05f && rb.velocity.y > fallSpeed)
			{
				reduceFallTime = Mathf.MoveTowards(reduceFallTime, 0f, Time.deltaTime);
				if (reduceFallTime <= 0f)
				{
					fallSpeed = rb.velocity.y;
				}
			}
			else if (rb.velocity.y > 0f)
			{
				fallSpeed = 0f;
			}
		}
		else if (fallTime > 0f)
		{
			fallTime = 0f;
		}
	}

	private void FixedUpdate()
	{
		if (limp || !(gc != null))
		{
			return;
		}
		if (knockedBack && knockBackCharge <= 0f && rb.velocity.magnitude < 1f && gc.onGround)
		{
			StopKnockBack();
		}
		else if (knockedBack)
		{
			if (eid.useBrakes || gc.onGround)
			{
				if (knockBackCharge <= 0f && gc.onGround)
				{
					brakes = Mathf.MoveTowards(brakes, 0f, 0.0005f * brakes);
				}
				rb.velocity = new Vector3(rb.velocity.x * 0.95f * brakes, rb.velocity.y - juggleWeight, rb.velocity.z * 0.95f * brakes);
			}
			else if (!eid.useBrakes)
			{
				brakes = 1f;
			}
			if (nma != null)
			{
				nma.updatePosition = false;
				nma.updateRotation = false;
				nma.enabled = false;
				rb.isKinematic = false;
				rb.useGravity = true;
			}
		}
		else if (!grounded && gc.onGround)
		{
			grounded = true;
		}
		else if (grounded && !gc.onGround)
		{
			grounded = false;
		}
		if (!gc.onGround && !falling && nma != null)
		{
			rb.isKinematic = false;
			rb.useGravity = true;
			nma.enabled = false;
			anim.speed = 1f;
			falling = true;
			anim.SetBool("Falling", value: true);
		}
	}

	public void KnockBack(Vector3 force)
	{
		if ((sm != null && !sm.inAction) || nma == null)
		{
			if (nma != null)
			{
				nma.enabled = false;
				rb.isKinematic = false;
				rb.useGravity = true;
			}
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
				if (sm == null)
				{
					rb.isKinematic = true;
				}
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

	public void GetHurt(GameObject target, Vector3 force, float multiplier, float critMultiplier)
	{
		string hitLimb = "";
		bool dead = false;
		bool flag = false;
		if (eid == null)
		{
			eid = GetComponent<EnemyIdentifier>();
		}
		if (force != Vector3.zero && !limp && sm == null)
		{
			KnockBack(force / 100f);
			if (eid.hitter == "heavypunch")
			{
				eid.useBrakes = false;
			}
			else
			{
				eid.useBrakes = true;
			}
		}
		if (target.gameObject.tag == "Head")
		{
			float num = 1f * multiplier + multiplier * critMultiplier;
			health -= num;
			if (eid.hitter != "fire")
			{
				GameObject gameObject = null;
				gameObject = ((!(num >= 1f) && !(health <= 0f)) ? Object.Instantiate(smallBlood, target.transform.position, Quaternion.identity) : Object.Instantiate(headBlood, target.transform.position, Quaternion.identity));
				gameObject.transform.SetParent(gz.goreZone, worldPositionStays: true);
				Bloodsplatter component = gameObject.GetComponent<Bloodsplatter>();
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
				if (!noheal)
				{
					component.GetReady();
				}
				component2.Play();
			}
			_ = (player.transform.position - base.transform.position).normalized;
			if (!limp)
			{
				flag = true;
				hitLimb = "head";
			}
			if (health <= 0f)
			{
				if (symbiotic)
				{
					if (sm != null && !sm.downed && symbiote.health > 0f)
					{
						if (symbiote.health > 0f)
						{
							sm.downed = true;
							sm.Down();
							Invoke("StartHealing", 6f);
						}
					}
					else if (symbiote.health <= 0f)
					{
						symbiotic = false;
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
				else
				{
					if (!limp)
					{
						GoLimp();
					}
					float num2 = 1f;
					if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone" || eid.hitter == "explosion")
					{
						num2 = 0.5f;
					}
					if (target.transform.parent != null && target.transform.parent.GetComponentInParent<Rigidbody>() != null)
					{
						target.transform.parent.GetComponentInParent<Rigidbody>().AddForce(force);
					}
					for (int i = 0; (float)i < 6f * num2; i++)
					{
						Object.Instantiate(skullFragment, target.transform.position, Random.rotation).transform.SetParent(gz.goreZone, worldPositionStays: true);
					}
					for (int j = 0; (float)j < 4f * num2; j++)
					{
						Object.Instantiate(brainChunk, target.transform.position, Random.rotation).transform.SetParent(gz.goreZone, worldPositionStays: true);
					}
					for (int k = 0; (float)k < 2f * num2; k++)
					{
						Object.Instantiate(eyeBall, target.transform.position, Random.rotation).transform.SetParent(gz.goreZone, worldPositionStays: true);
						Object.Instantiate(jawHalf, target.transform.position, Random.rotation).transform.SetParent(gz.goreZone, worldPositionStays: true);
					}
				}
			}
		}
		else if (target.gameObject.tag == "Limb" || target.gameObject.tag == "EndLimb")
		{
			if (eid == null)
			{
				eid = GetComponent<EnemyIdentifier>();
			}
			float num = 1f * multiplier + 0.5f * multiplier * critMultiplier;
			health -= num;
			if (eid.hitter != "fire")
			{
				GameObject gameObject2 = null;
				if (((num >= 1f || health <= 0f) && eid.hitter != "explosion") || (eid.hitter == "explosion" && target.gameObject.tag == "EndLimb"))
				{
					gameObject2 = Object.Instantiate(limbBlood, target.transform.position, Quaternion.identity);
				}
				else if (eid.hitter != "explosion")
				{
					gameObject2 = Object.Instantiate(smallBlood, target.transform.position, Quaternion.identity);
				}
				if (gameObject2 != null)
				{
					gameObject2.transform.SetParent(gz.goreZone, worldPositionStays: true);
					Bloodsplatter component3 = gameObject2.GetComponent<Bloodsplatter>();
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
			}
			if (!limp)
			{
				flag = true;
				hitLimb = "limb";
			}
			if (health <= 0f)
			{
				if (symbiotic)
				{
					if (sm != null && !sm.downed && symbiote.health > 0f)
					{
						if (symbiote.health > 0f)
						{
							sm.downed = true;
							sm.Down();
							Invoke("StartHealing", 3f);
						}
					}
					else if (symbiote.health <= 0f)
					{
						symbiotic = false;
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
				else
				{
					if (!limp)
					{
						GoLimp();
					}
					if (target.gameObject.tag == "Limb")
					{
						float num3 = 1f;
						if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone" || eid.hitter == "explosion")
						{
							num3 = 0.5f;
						}
						for (int l = 0; (float)l < 4f * num3; l++)
						{
							Object.Instantiate(giblet[Random.Range(0, giblet.Length)], target.transform.position, Random.rotation).transform.SetParent(gz.goreZone, worldPositionStays: true);
						}
					}
					else
					{
						target.transform.localScale = Vector3.zero;
					}
				}
			}
		}
		else if (target.gameObject.tag == "Body")
		{
			float num = 1f * multiplier;
			if (eid == null)
			{
				eid = GetComponent<EnemyIdentifier>();
			}
			if (eid.hitter == "shotgunzone")
			{
				if (!parryable && (target.gameObject != chest || health - num > 0f))
				{
					num = 0f;
				}
				else if (parryable && (target.gameObject == chest || player.GetComponent<Rigidbody>().velocity.magnitude > 18f))
				{
					num *= 1.5f;
					parryable = false;
					GameObject.FindWithTag("MainCamera").GetComponentInChildren<Punch>().Parry();
					if (sm != null)
					{
						if (!sm.enraged)
						{
							sm.Knockdown(light: true);
						}
						else
						{
							sm.Enrage();
						}
					}
				}
			}
			health -= num;
			if (eid.hitter != "fire")
			{
				GameObject gameObject3 = null;
				if (((num >= 1f || health <= 0f) && eid.hitter != "explosion") || (eid.hitter == "explosion" && target.gameObject.tag == "EndLimb"))
				{
					gameObject3 = Object.Instantiate(limbBlood, target.transform.position, Quaternion.identity);
				}
				else if (eid.hitter != "explosion")
				{
					gameObject3 = Object.Instantiate(smallBlood, target.transform.position, Quaternion.identity);
				}
				if (gameObject3 != null)
				{
					gameObject3.transform.SetParent(gz.goreZone, worldPositionStays: true);
					Bloodsplatter component5 = gameObject3.GetComponent<Bloodsplatter>();
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
					if (!noheal)
					{
						component5.GetReady();
					}
					component6.Play();
				}
			}
			if (health <= 0f && !symbiotic && target.gameObject == chest)
			{
				chestHP -= num;
				if (chestHP <= 0f || eid.hitter == "shotgunzone")
				{
					CharacterJoint[] componentsInChildren = target.GetComponentsInChildren<CharacterJoint>();
					if (componentsInChildren.Length != 0)
					{
						CharacterJoint[] array = componentsInChildren;
						foreach (CharacterJoint characterJoint in array)
						{
							if (characterJoint.transform.parent.parent == chest.transform)
							{
								Object.Destroy(characterJoint);
								characterJoint.transform.parent = null;
							}
						}
					}
					_ = limp;
					for (int n = 0; n < 2; n++)
					{
						Object.Instantiate(giblet[Random.Range(0, giblet.Length)], target.transform.position, Random.rotation).transform.SetParent(gz.goreZone, worldPositionStays: true);
					}
					Object.Instantiate(headBlood, target.transform.position, Quaternion.identity).transform.SetParent(gz.goreZone, worldPositionStays: true);
					target.transform.localScale = Vector3.zero;
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
				if (symbiotic)
				{
					if (sm != null && !sm.downed && symbiote.health > 0f)
					{
						if (symbiote.health > 0f)
						{
							sm.downed = true;
							sm.Down();
							Invoke("StartHealing", 3f);
						}
					}
					else if (symbiote.health <= 0f)
					{
						symbiotic = false;
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
				else
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
		}
		if (eid.hitter == "punch" && parryable)
		{
			parryable = false;
			float num = 5f;
			health -= num;
			GameObject.FindWithTag("MainCamera").GetComponentInChildren<Punch>().Parry();
			if (sm != null)
			{
				if (!sm.enraged)
				{
					sm.Knockdown(light: true);
				}
				else
				{
					sm.Enrage();
				}
			}
		}
		if (!symbiotic)
		{
			if (health <= 0f && (target.gameObject.tag == "Limb" || target.gameObject.tag == "Head"))
			{
				if (target.gameObject.tag != "Head" && target.transform.childCount > 0)
				{
					Transform child = target.transform.GetChild(0);
					CharacterJoint[] componentsInChildren2 = target.GetComponentsInChildren<CharacterJoint>();
					if (componentsInChildren2.Length != 0)
					{
						CharacterJoint[] array = componentsInChildren2;
						for (int m = 0; m < array.Length; m++)
						{
							Object.Destroy(array[m]);
						}
					}
					CharacterJoint component7 = target.GetComponent<CharacterJoint>();
					if (component7 != null)
					{
						component7.connectedBody = null;
						Object.Destroy(component7);
					}
					target.transform.position = child.position;
					target.transform.SetParent(child);
					child.SetParent(gz.gibZone);
					Object.Destroy(target.GetComponent<Rigidbody>());
				}
				Object.Destroy(target.GetComponent<Collider>());
				target.transform.localScale = Vector3.zero;
			}
			else if (health <= 0f && target.gameObject.tag == "EndLimb")
			{
				target.transform.localScale = Vector3.zero;
			}
		}
		if ((health > 0f || symbiotic) && hurtSounds.Length != 0)
		{
			if (aud == null)
			{
				aud = GetComponent<AudioSource>();
			}
			aud.clip = hurtSounds[Random.Range(0, hurtSounds.Length)];
			aud.volume = 0.5f;
			if (sm != null)
			{
				aud.pitch = Random.Range(0.85f, 1.35f);
			}
			else
			{
				aud.pitch = Random.Range(0.9f, 1.1f);
			}
			aud.priority = 12;
			aud.Play();
		}
		if (eid == null)
		{
			eid = GetComponent<EnemyIdentifier>();
		}
		if (!flag || !(eid.hitter != "enemy"))
		{
			return;
		}
		if (scalc == null)
		{
			scalc = GameObject.FindWithTag("StyleHUD").GetComponent<StyleCalculator>();
		}
		if (health <= 0f && !symbiotic)
		{
			dead = true;
		}
		if (eid.hitter != "secret")
		{
			if (bigKill)
			{
				scalc.HitCalculator(eid.hitter, "spider", hitLimb, dead, base.gameObject);
			}
			else
			{
				scalc.HitCalculator(eid.hitter, "machine", hitLimb, dead, base.gameObject);
			}
		}
	}

	public void GoLimp()
	{
		gz = GetComponentInParent<GoreZone>();
		if (sm == null)
		{
			sm = GetComponent<SwordsMachine>();
		}
		Invoke("StopHealing", 1f);
		SwingCheck2[] componentsInChildren = GetComponentsInChildren<SwingCheck2>();
		if (sm != null)
		{
			anim.StopPlayback();
			SwingCheck2[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				Object.Destroy(array[i]);
			}
			sm.CoolSword();
			if (sm.currentEnrageEffect != null)
			{
				Object.Destroy(sm.currentEnrageEffect);
			}
			Object.Destroy(sm);
		}
		if (destroyOnDeath.Length != 0)
		{
			GameObject[] array2 = destroyOnDeath;
			foreach (GameObject obj in array2)
			{
				Transform transform = obj.GetComponentInParent<Rigidbody>().transform;
				obj.transform.SetParent(transform);
				obj.transform.position = transform.position;
				obj.transform.localScale = Vector3.zero;
			}
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
		EnemySimplifier component = GetComponent<EnemySimplifier>();
		if (component != null)
		{
			component.Begone();
		}
		if (deadMaterial != null)
		{
			smr.sharedMaterial = deadMaterial;
		}
		else if (smr != null)
		{
			smr.sharedMaterial = originalMaterial;
		}
		if (nma != null)
		{
			Object.Destroy(nma);
		}
		nma = null;
		Object.Destroy(anim);
		Object.Destroy(base.gameObject.GetComponent<Collider>());
		if (rb == null)
		{
			rb = GetComponent<Rigidbody>();
		}
		Object.Destroy(rb);
		if (aud == null)
		{
			aud = GetComponent<AudioSource>();
		}
		if (deathSound != null)
		{
			aud.clip = deathSound;
			aud.pitch = Random.Range(0.85f, 1.35f);
			aud.priority = 11;
			aud.Play();
		}
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
			ActivateNextWave componentInParent = GetComponentInParent<ActivateNextWave>();
			if (componentInParent != null)
			{
				componentInParent.deadEnemies++;
			}
		}
		if (musicRequested)
		{
			GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>().PlayCleanMusic();
		}
		limp = true;
		EnemyScanner componentInChildren = GetComponentInChildren<EnemyScanner>();
		if (componentInChildren != null)
		{
			Object.Destroy(componentInChildren.gameObject);
		}
	}

	private void StartHealing()
	{
		if (symbiotic && symbiote != null)
		{
			healing = true;
		}
	}

	private void StopHealing()
	{
		noheal = true;
	}
}
