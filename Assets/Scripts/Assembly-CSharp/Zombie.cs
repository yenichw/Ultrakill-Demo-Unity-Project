using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
	public bool spawnIn;

	public GameObject spawnEffect;

	public float health;

	private float originalHealth;

	private int difficulty;

	private Rigidbody[] rbs;

	public bool limp;

	public GameObject player;

	public NavMeshAgent nma;

	public Animator anim;

	private float currentSpeed;

	private Rigidbody rb;

	private ZombieMelee zm;

	private ZombieProjectiles zp;

	private AudioSource aud;

	public AudioClip[] hurtSounds;

	public float hurtSoundVol;

	public AudioClip deathSound;

	public float deathSoundVol;

	public AudioClip scream;

	private GroundCheckEnemy gc;

	public bool grounded;

	private float defaultSpeed;

	public Vector3 agentVelocity;

	public GameObject bodyBlood;

	public GameObject limbBlood;

	public GameObject headBlood;

	public GameObject smallBlood;

	public GameObject skullFragment;

	public GameObject eyeBall;

	public GameObject jawHalf;

	public GameObject brainChunk;

	public GameObject[] giblet;

	private StyleCalculator scalc;

	private EnemyIdentifier eid;

	private GoreZone gz;

	public Material deadMaterial;

	public Material simplifiedMaterial;

	private OptionsManager oman;

	private SkinnedMeshRenderer smr;

	private Material originalMaterial;

	public GameObject chest;

	private float chestHP = 3f;

	public bool chestExploding;

	public GameObject chestExplosionStuff;

	public bool attacking;

	public LayerMask lmask;

	public Transform target;

	public List<EnemyIdentifier> enemyTargets = new List<EnemyIdentifier>();

	public bool friendly;

	public EnemyIdentifier targetedEnemy;

	private bool noheal;

	private float speedMultiplier = 1f;

	public bool stopped;

	public bool knockedBack;

	private float knockBackCharge;

	public float brakes;

	public float juggleWeight;

	public bool falling;

	public bool musicRequested;

	private float fallSpeed;

	private float fallTime;

	private float reduceFallTime;

	private BloodsplatterManager bsm;

	public bool variableSpeed;

	private bool blindedByCheat;

	private void Start()
	{
		rbs = GetComponentsInChildren<Rigidbody>();
		player = GameObject.FindWithTag("Player");
		nma = GetComponent<NavMeshAgent>();
		rb = GetComponent<Rigidbody>();
		zm = GetComponent<ZombieMelee>();
		zp = GetComponent<ZombieProjectiles>();
		anim = GetComponent<Animator>();
		gc = GetComponentInChildren<GroundCheckEnemy>();
		eid = GetComponent<EnemyIdentifier>();
		if (spawnIn)
		{
			Object.Instantiate(position: new Vector3(base.transform.position.x, base.transform.position.y + 1.5f, base.transform.position.z), original: spawnEffect, rotation: base.transform.rotation);
			spawnIn = false;
		}
		originalHealth = health;
		oman = GameObject.FindWithTag("RoomManager").GetComponent<OptionsManager>();
		smr = GetComponentInChildren<SkinnedMeshRenderer>();
		originalMaterial = smr.sharedMaterial;
		if (!friendly)
		{
			target = player.transform;
			EnemyScanner componentInChildren = GetComponentInChildren<EnemyScanner>();
			if (componentInChildren != null)
			{
				componentInChildren.gameObject.SetActive(value: false);
			}
		}
		if (limp)
		{
			noheal = true;
		}
		difficulty = PlayerPrefs.GetInt("Diff", 2);
		if (difficulty != 2 && difficulty >= 3)
		{
			if (nma == null)
			{
				nma = GetComponent<NavMeshAgent>();
			}
			if (nma != null)
			{
				nma.acceleration = 60f;
				nma.angularSpeed = 2600f;
			}
			speedMultiplier = 1.25f;
		}
		if (nma != null)
		{
			defaultSpeed = nma.speed;
		}
	}

	private void Update()
	{
		if (anim != null && variableSpeed)
		{
			anim.SetFloat("Speed", anim.speed);
		}
		if (knockBackCharge > 0f)
		{
			knockBackCharge = Mathf.MoveTowards(knockBackCharge, 0f, Time.deltaTime);
		}
		if (!limp && friendly && enemyTargets.Count > 0)
		{
			if (target == null)
			{
				float num = 100f;
				foreach (EnemyIdentifier enemyTarget in enemyTargets)
				{
					if (Vector3.Distance(base.transform.position, enemyTarget.transform.position) < num)
					{
						num = Vector3.Distance(base.transform.position, enemyTarget.transform.position);
						target = enemyTarget.transform;
						targetedEnemy = enemyTarget;
					}
				}
			}
			else if (targetedEnemy.dead)
			{
				enemyTargets.Remove(targetedEnemy);
				if (enemyTargets.Count == 0)
				{
					target = null;
				}
				else
				{
					float num2 = 100f;
					foreach (EnemyIdentifier enemyTarget2 in enemyTargets)
					{
						if (Vector3.Distance(base.transform.position, enemyTarget2.transform.position) < num2)
						{
							num2 = Vector3.Distance(base.transform.position, enemyTarget2.transform.position);
							target = enemyTarget2.transform;
							targetedEnemy = enemyTarget2;
						}
					}
				}
			}
		}
		if (falling && !limp)
		{
			fallTime += Time.deltaTime;
			if (gc.onGround)
			{
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
				if (aud == null)
				{
					aud = GetComponent<AudioSource>();
				}
				if (aud.clip == scream && aud.isPlaying)
				{
					aud.Stop();
				}
				rb.isKinematic = true;
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
		if (limp)
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
			nma.updatePosition = false;
			nma.updateRotation = false;
			nma.enabled = false;
			rb.isKinematic = false;
			rb.useGravity = true;
		}
		if (grounded && nma != null && nma.enabled && variableSpeed)
		{
			if (nma.isStopped || nma.velocity == Vector3.zero || stopped)
			{
				anim.speed = 1f * speedMultiplier;
			}
			else
			{
				anim.speed = nma.velocity.magnitude / nma.speed * speedMultiplier;
			}
		}
		else if (!grounded && gc.onGround)
		{
			grounded = true;
			nma.speed = defaultSpeed;
		}
		if (!gc.onGround && !falling)
		{
			rb.isKinematic = false;
			rb.useGravity = true;
			nma.enabled = false;
			anim.speed = 1f;
			falling = true;
			anim.SetBool("Falling", value: true);
			anim.SetTrigger("StartFalling");
			if (zp != null)
			{
				zp.CancelAttack();
			}
			if (zm != null)
			{
				zm.CancelAttack();
			}
		}
	}

	public void KnockBack(Vector3 force)
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

	public void StopKnockBack()
	{
		if (!(nma != null))
		{
			return;
		}
		if (Physics.Raycast(base.transform.position + Vector3.up * 0.1f, Vector3.down, out var hitInfo, float.PositiveInfinity, lmask))
		{
			_ = Vector3.zero;
			Debug.Log("rhit.point: " + hitInfo.point);
			if (NavMesh.SamplePosition(hitInfo.point, out var hit, 4f, nma.areaMask))
			{
				knockedBack = false;
				nma.updatePosition = true;
				nma.updateRotation = true;
				nma.enabled = true;
				rb.isKinematic = true;
				juggleWeight = 0f;
				eid.magneted = false;
				nma.Warp(hit.position);
			}
			else
			{
				Debug.Log("Failed Sample");
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
		bool flag = false;
		bool flag2 = false;
		bool flag3 = true;
		if ((bool)gc && !gc.onGround)
		{
			multiplier *= 1.5f;
		}
		if (eid == null)
		{
			eid = GetComponent<EnemyIdentifier>();
			if (eid == null)
			{
				return;
			}
		}
		if (player == null)
		{
			player = Object.FindObjectOfType<NewMovement>().gameObject;
		}
		if (force != Vector3.zero && !limp)
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
		if (PlayerPrefs.GetInt("BlOn", 1) == 0)
		{
			flag3 = false;
		}
		if (chestExploding && health <= 0f && (target.gameObject.tag == "Limb" || target.gameObject.tag == "EndLimb") && target.GetComponentInParent<EnemyIdentifier>() != null)
		{
			ChestExplodeEnd();
		}
		GameObject gameObject = null;
		if (bsm == null)
		{
			bsm = Object.FindObjectOfType<BloodsplatterManager>();
		}
		if (target.gameObject.tag == "Head")
		{
			float num = 1f * multiplier + multiplier * critMultiplier;
			health -= num;
			if (eid.hitter != "fire" && num > 0f)
			{
				gameObject = ((!(num >= 1f) && !(health <= 0f)) ? bsm.GetGore(GoreType.Small) : bsm.GetGore(GoreType.Head));
			}
			_ = (player.transform.position - base.transform.position).normalized;
			if (!limp)
			{
				flag2 = true;
				hitLimb = "head";
			}
			if (health <= 0f)
			{
				if (!limp)
				{
					GoLimp();
				}
				if (eid.hitter != "fire")
				{
					float num2 = 1f;
					if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone")
					{
						num2 = 0.5f;
					}
					else if (eid.hitter == "Explosion")
					{
						num2 = 0.25f;
					}
					if (target.transform.parent != null && target.transform.parent.GetComponentInParent<Rigidbody>() != null)
					{
						target.transform.parent.GetComponentInParent<Rigidbody>().AddForce(force * 10f);
					}
					if (flag3 && eid.hitter != "harpoon")
					{
						GameObject gameObject2 = null;
						if (gz == null)
						{
							gz = GetComponentInParent<GoreZone>();
						}
						for (int i = 0; (float)i < 6f * num2; i++)
						{
							gameObject2 = Object.Instantiate(skullFragment, target.transform.position, Random.rotation);
							if (gz != null && gz.goreZone != null)
							{
								gameObject2.transform.SetParent(gz.goreZone);
							}
						}
						for (int j = 0; (float)j < 4f * num2; j++)
						{
							gameObject2 = Object.Instantiate(brainChunk, target.transform.position, Random.rotation);
							if (gz != null && gz.goreZone != null)
							{
								gameObject2.transform.SetParent(gz.goreZone);
							}
						}
						for (int k = 0; (float)k < 2f * num2; k++)
						{
							gameObject2 = Object.Instantiate(eyeBall, target.transform.position, Random.rotation);
							if (gz != null && gz.goreZone != null)
							{
								gameObject2.transform.SetParent(gz.goreZone);
							}
							gameObject2 = Object.Instantiate(jawHalf, target.transform.position, Random.rotation);
							if (gz != null && gz.goreZone != null)
							{
								gameObject2.transform.SetParent(gz.goreZone);
							}
						}
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
			if (eid.hitter != "fire" && num > 0f)
			{
				if (((num >= 1f || health <= 0f) && eid.hitter != "explosion") || (eid.hitter == "explosion" && target.gameObject.tag == "EndLimb"))
				{
					gameObject = bsm.GetGore(GoreType.Limb);
				}
				else if (eid.hitter != "explosion")
				{
					gameObject = bsm.GetGore(GoreType.Small);
				}
			}
			_ = (player.transform.position - base.transform.position).normalized;
			if (!limp)
			{
				flag2 = true;
				hitLimb = "limb";
			}
			if (health <= 0f)
			{
				if (!limp)
				{
					GoLimp();
				}
				if (eid.hitter != "fire")
				{
					if (target.gameObject.tag == "Limb" && flag3 && eid.hitter != "harpoon" && eid.hitter != "explosion")
					{
						float num3 = 1f;
						if (gz == null)
						{
							gz = GetComponentInParent<GoreZone>();
						}
						if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone")
						{
							num3 = 0.5f;
						}
						for (int l = 0; (float)l < 4f * num3; l++)
						{
							GameObject gameObject3 = Object.Instantiate(giblet[Random.Range(0, giblet.Length)], target.transform.position, Random.rotation);
							if (gz != null && gz.goreZone != null)
							{
								gameObject3.transform.SetParent(gz.goreZone);
							}
						}
					}
					else
					{
						target.transform.localScale = Vector3.zero;
					}
				}
			}
		}
		else if (target.gameObject.tag == "Body" || (attacking && (eid.hitter == "shotgunzone" || eid.hitter == "punch") && player.GetComponent<Rigidbody>().velocity.magnitude > 18f))
		{
			float num = 1f * multiplier;
			if (eid == null)
			{
				eid = GetComponent<EnemyIdentifier>();
			}
			if (eid.hitter == "shotgunzone")
			{
				if (!attacking && (target.gameObject != chest || health - num > 0f))
				{
					num = 0f;
				}
				else if (attacking && (target.gameObject == chest || player.GetComponent<Rigidbody>().velocity.magnitude > 18f))
				{
					num *= 2f;
					Object.FindObjectOfType<Punch>().Parry();
				}
			}
			else if (eid.hitter == "punch" && attacking)
			{
				num = 2f;
				attacking = false;
				Object.FindObjectOfType<Punch>().Parry();
			}
			health -= num;
			if (eid.hitter != "fire" && num > 0f)
			{
				gameObject = ((!(num >= 1f) && !(health <= 0f)) ? bsm.GetGore(GoreType.Small) : bsm.GetGore(GoreType.Body));
			}
			if (health <= 0f && target.gameObject == chest && eid.hitter != "fire")
			{
				if (eid.hitter == "shotgunzone")
				{
					chestHP = 0f;
				}
				else
				{
					chestHP -= num;
				}
				if (chestHP <= 0f && eid.hitter != "harpoon")
				{
					CharacterJoint[] componentsInChildren = target.GetComponentsInChildren<CharacterJoint>();
					GoreZone componentInParent = GetComponentInParent<GoreZone>();
					if (componentsInChildren.Length != 0)
					{
						CharacterJoint[] array = componentsInChildren;
						foreach (CharacterJoint characterJoint in array)
						{
							if (characterJoint.transform.parent.parent == chest.transform)
							{
								Rigidbody[] componentsInChildren2 = characterJoint.transform.GetComponentsInChildren<Rigidbody>();
								foreach (Rigidbody obj in componentsInChildren2)
								{
									obj.isKinematic = false;
									obj.useGravity = true;
								}
								Object.Destroy(characterJoint);
							}
						}
					}
					if (!limp && !eid.exploded && !eid.dead)
					{
						if (gc.onGround)
						{
							rb.isKinematic = true;
							knockedBack = false;
						}
						anim.Rebind();
						anim.speed = 1f;
						anim.SetTrigger("ChestExplosion");
						chestExploding = true;
					}
					if (componentInParent == null)
					{
						componentInParent = GetComponentInParent<GoreZone>();
					}
					if (flag3)
					{
						float num4 = 1f;
						if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone" || eid.hitter == "explosion")
						{
							num4 = 0.5f;
						}
						for (int num5 = 0; (float)num5 < 2f * num4; num5++)
						{
							GameObject gameObject4 = Object.Instantiate(giblet[Random.Range(0, giblet.Length)], target.transform.position, Random.rotation);
							if (componentInParent != null && componentInParent.goreZone != null)
							{
								gameObject4.transform.SetParent(componentInParent.goreZone, worldPositionStays: true);
							}
						}
						Object.Instantiate(chestExplosionStuff, target.transform.parent.position, target.transform.parent.rotation).transform.SetParent(target.transform.parent, worldPositionStays: true);
					}
					GameObject gameObject5 = Object.Instantiate(headBlood, target.transform.position, Quaternion.identity);
					gameObject5.GetComponent<Bloodsplatter>().hpAmount = 10;
					if (componentInParent != null && componentInParent.goreZone != null)
					{
						gameObject5.transform.SetParent(componentInParent.goreZone, worldPositionStays: true);
					}
					if (!noheal)
					{
						gameObject5.GetComponent<Bloodsplatter>().GetReady();
					}
					target.transform.localScale = Vector3.zero;
				}
			}
			if (!limp)
			{
				flag2 = true;
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
					target.GetComponentInParent<Rigidbody>().AddForce(force * 10f);
				}
			}
		}
		if (gameObject != null)
		{
			if (gz == null)
			{
				gz = GetComponentInParent<GoreZone>();
			}
			gameObject.transform.position = target.transform.position;
			if (gz != null && gz.goreZone != null)
			{
				gameObject.transform.SetParent(gz.goreZone, worldPositionStays: true);
			}
			Bloodsplatter component = gameObject.GetComponent<Bloodsplatter>();
			ParticleSystem.CollisionModule collision = component.GetComponent<ParticleSystem>().collision;
			if (eid.hitter == "shotgun" || eid.hitter == "shotgunzone" || eid.hitter == "explosion")
			{
				if (Random.Range(0f, 1f) > 0.5f)
				{
					collision.enabled = false;
				}
				component.hpAmount = 3;
			}
			else if (eid.hitter == "nail")
			{
				component.hpAmount = 1;
				component.GetComponent<AudioSource>().volume *= 0.8f;
			}
			if (!noheal)
			{
				component.GetReady();
			}
		}
		if (health <= 0f && (target.gameObject.tag == "Limb" || target.gameObject.tag == "Head") && eid.hitter != "harpoon" && eid.hitter != "fire")
		{
			if (target.transform.childCount > 0)
			{
				Transform child = target.transform.GetChild(0);
				CharacterJoint[] componentsInChildren3 = target.GetComponentsInChildren<CharacterJoint>();
				GoreZone componentInParent2 = GetComponentInParent<GoreZone>();
				if (componentsInChildren3.Length != 0)
				{
					CharacterJoint[] array = componentsInChildren3;
					foreach (CharacterJoint characterJoint2 in array)
					{
						if (componentInParent2 != null && componentInParent2.goreZone != null)
						{
							characterJoint2.transform.SetParent(componentInParent2.goreZone);
						}
						Object.Destroy(characterJoint2);
					}
				}
				CharacterJoint component2 = target.GetComponent<CharacterJoint>();
				if (component2 != null)
				{
					component2.connectedBody = null;
					Object.Destroy(component2);
				}
				target.transform.position = child.position;
				target.transform.SetParent(child);
				if (componentInParent2 != null && componentInParent2.gibZone != null)
				{
					child.SetParent(componentInParent2.gibZone, worldPositionStays: true);
				}
				Object.Destroy(target.GetComponent<Rigidbody>());
			}
			Object.Destroy(target.GetComponent<Collider>());
			target.transform.localScale = Vector3.zero;
		}
		else if (health <= 0f && target.gameObject.tag == "EndLimb" && eid.hitter != "harpoon" && eid.hitter != "fire")
		{
			target.transform.localScale = Vector3.zero;
		}
		if (health > 0f && hurtSounds.Length != 0)
		{
			if (aud == null)
			{
				aud = GetComponent<AudioSource>();
			}
			aud.clip = hurtSounds[Random.Range(0, hurtSounds.Length)];
			aud.volume = hurtSoundVol;
			aud.pitch = Random.Range(0.85f, 1.35f);
			aud.priority = 12;
			aud.Play();
		}
		if (eid == null)
		{
			eid = GetComponent<EnemyIdentifier>();
		}
		if (!flag2 || !(eid.hitter != "enemy"))
		{
			return;
		}
		if (scalc == null)
		{
			scalc = GameObject.FindWithTag("StyleHUD").GetComponent<StyleCalculator>();
		}
		if (health <= 0f)
		{
			flag = true;
			if (!gc.onGround && (eid.hitter == "explosion" || eid.hitter == "ffexplosion" || eid.hitter == "railcannon"))
			{
				scalc.shud.AddPoints(120, "<color=cyan>FIREWORKS</color>");
			}
			else if (!gc.onGround && eid.hitter != "deathzone")
			{
				scalc.shud.AddPoints(50, "<color=cyan>AIRSHOT</color>");
			}
		}
		if (eid.hitter != "secret")
		{
			scalc.HitCalculator(eid.hitter, "zombie", hitLimb, flag, base.gameObject);
		}
		if (flag && GetComponentInChildren<Flammable>().burning && eid.hitter != "fire")
		{
			scalc.shud.AddPoints(50, "<color=cyan>FINISHED OFF</color>");
		}
	}

	public void GoLimp()
	{
		gz = GetComponentInParent<GoreZone>();
		attacking = false;
		Invoke("StopHealing", 1f);
		if (eid == null)
		{
			eid = GetComponent<EnemyIdentifier>();
		}
		if (!friendly)
		{
			if (gz != null && gz.checkpoint != null)
			{
				gz.AddDeath();
				gz.checkpoint.sm.kills++;
			}
			else
			{
				GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>().kills++;
			}
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
		else
		{
			smr.sharedMaterial = originalMaterial;
		}
		if (zm != null)
		{
			zm.track = false;
			if (!chestExploding)
			{
				anim.StopPlayback();
			}
			if (zm.tr != null)
			{
				zm.tr.enabled = false;
			}
			Object.Destroy(base.gameObject.GetComponentInChildren<SwingCheck2>().gameObject);
			Object.Destroy(zm);
		}
		if (zp != null)
		{
			zp.DamageEnd();
			if (!chestExploding)
			{
				anim.StopPlayback();
			}
			Object.Destroy(zp);
			Projectile componentInChildren = GetComponentInChildren<Projectile>();
			if (componentInChildren != null)
			{
				Object.Destroy(componentInChildren.gameObject);
			}
		}
		if (nma != null)
		{
			Object.Destroy(nma);
		}
		if (!chestExploding)
		{
			Object.Destroy(anim);
		}
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
			if (eid.hitter != "fire")
			{
				aud.volume = deathSoundVol;
			}
			else
			{
				aud.volume = 0.5f;
			}
			aud.pitch = Random.Range(0.85f, 1.35f);
			aud.priority = 11;
			aud.Play();
		}
		if (!limp && !chestExploding)
		{
			rbs = GetComponentsInChildren<Rigidbody>();
			Rigidbody[] array = rbs;
			foreach (Rigidbody obj in array)
			{
				obj.isKinematic = false;
				obj.useGravity = true;
			}
		}
		if (!limp)
		{
			ActivateNextWave componentInParent = GetComponentInParent<ActivateNextWave>();
			if (componentInParent != null)
			{
				componentInParent.deadEnemies++;
			}
			if (gz != null && gz.gibZone != null)
			{
				base.transform.SetParent(gz.gibZone, worldPositionStays: true);
			}
		}
		if (musicRequested)
		{
			musicRequested = false;
			GameObject.FindWithTag("RoomManager").GetComponentInChildren<MusicManager>().PlayCleanMusic();
		}
		limp = true;
		EnemyScanner componentInChildren2 = GetComponentInChildren<EnemyScanner>();
		if (componentInChildren2 != null)
		{
			Object.Destroy(componentInChildren2.gameObject);
		}
	}

	public void ChestExplodeEnd()
	{
		anim.enabled = false;
		anim.StopPlayback();
		Object.Destroy(anim);
		rbs = GetComponentsInChildren<Rigidbody>();
		Rigidbody[] array = rbs;
		foreach (Rigidbody rigidbody in array)
		{
			if (rigidbody != null)
			{
				rigidbody.isKinematic = false;
				rigidbody.useGravity = true;
			}
		}
		chestExploding = false;
	}

	public void StopHealing()
	{
		noheal = true;
	}
}
