using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class NewMovement : MonoBehaviour
{
	private InputManager inman;

	[HideInInspector]
	public AssistController asscon;

	public float walkSpeed;

	public float jumpPower;

	public float airAcceleration;

	public float wallJumpPower;

	private bool jumpCooldown;

	private bool falling;

	[HideInInspector]
	public Rigidbody rb;

	private Vector3 movementDirection;

	private Vector3 movementDirection2;

	private Vector3 airDirection;

	public float timeBetweenSteps;

	private float stepTime;

	private int currentStep;

	[HideInInspector]
	public Animator anim;

	private Quaternion tempRotation;

	private GameObject forwardPoint;

	public GroundCheck gc;

	public GroundCheck slopeCheck;

	private WallCheck wc;

	private PlayerAnimations pa;

	private Vector3 wallJumpPos;

	public int currentWallJumps;

	private AudioSource aud;

	private AudioSource aud2;

	private AudioSource aud3;

	private int currentSound;

	public AudioClip jumpSound;

	public AudioClip landingSound;

	public AudioClip finalWallJump;

	public bool walking;

	public int hp = 100;

	public float antiHp;

	private float antiHpCooldown;

	public Image hurtScreen;

	private AudioSource hurtAud;

	private Color hurtColor;

	private Color currentColor;

	private bool hurting;

	public bool dead;

	public bool endlessMode;

	public Image blackScreen;

	private Color blackColor;

	public Text youDiedText;

	private Color youDiedColor;

	public Image greenHpFlash;

	private Color greenHpColor;

	private AudioSource greenHpAud;

	public AudioMixer audmix;

	private float currentAllPitch = 1f;

	private float currentAllVolume;

	public bool boost;

	public Vector3 dodgeDirection;

	private float boostLeft;

	public float boostCharge = 300f;

	public AudioClip dodgeSound;

	public CameraController cc;

	private AudioSource ccAud;

	public GameObject staminaFailSound;

	public GameObject screenHud;

	private Vector3 hudOriginalPos;

	public GameObject dodgeParticle;

	public GameObject scrnBlood;

	private Canvas fullHud;

	public GameObject hudCam;

	private Vector3 camOriginalPos;

	private RigidbodyConstraints defaultRBConstraints;

	private GameObject revolver;

	private StyleHUD shud;

	public GameObject scrapePrefab;

	private GameObject scrapeParticle;

	public LayerMask lmask;

	public StyleCalculator scalc;

	public bool activated;

	private float fallSpeed;

	public bool jumping;

	private float fallTime;

	public GameObject impactDust;

	public GameObject fallParticle;

	private GameObject currentFallParticle;

	private CapsuleCollider playerCollider;

	public bool sliding;

	private float slideSafety;

	public GameObject slideParticle;

	private GameObject currentSlideParticle;

	public GameObject slideScrapePrefab;

	private GameObject slideScrape;

	private Vector3 slideMovDirection;

	public GameObject slideStopSound;

	private bool crouching;

	public bool standing;

	private bool slideEnding;

	private GunControl gunc;

	public float currentSpeed;

	private FistControl punch;

	public GameObject dashJumpSound;

	public bool slowMode;

	public Vector3 pushForce;

	private float slideLength;

	public float longestSlide;

	public bool quakeJump;

	public GameObject quakeJumpSound;

	private bool exploded;

	private float clingFade;

	public bool stillHolding;

	public float slamForce;

	private bool slamStorage;

	private int difficulty;

	private void Start()
	{
		inman = Object.FindObjectOfType<InputManager>();
		asscon = Object.FindObjectOfType<AssistController>();
		rb = GetComponent<Rigidbody>();
		aud = GetComponent<AudioSource>();
		anim = GetComponentInChildren<Animator>();
		wc = GetComponentInChildren<WallCheck>();
		aud2 = gc.GetComponent<AudioSource>();
		pa = GetComponentInChildren<PlayerAnimations>();
		aud3 = wc.GetComponent<AudioSource>();
		cc = GetComponentInChildren<CameraController>();
		ccAud = cc.GetComponent<AudioSource>();
		hurtColor = hurtScreen.color;
		currentColor = hurtColor;
		currentColor.a = 0f;
		hurtScreen.color = currentColor;
		hurtAud = hurtScreen.GetComponent<AudioSource>();
		blackColor = blackScreen.color;
		youDiedColor = youDiedText.color;
		greenHpColor = greenHpFlash.color;
		fullHud = hurtScreen.GetComponentInParent<Canvas>();
		hudOriginalPos = screenHud.transform.localPosition;
		camOriginalPos = hudCam.transform.localPosition;
		currentAllPitch = 1f;
		audmix.SetFloat("allPitch", 1f);
		defaultRBConstraints = rb.constraints;
		playerCollider = GetComponent<CapsuleCollider>();
		scalc = GameObject.FindWithTag("StyleHUD").GetComponent<StyleCalculator>();
		difficulty = PlayerPrefs.GetInt("Diff", 2);
		if (difficulty < 2 && hp == 100)
		{
			hp = 200;
		}
	}

	private void Update()
	{
		float num = 0f;
		float num2 = 0f;
		if (activated)
		{
			num2 = (Input.GetKey(inman.Inputs["W"]) ? 1f : ((!Input.GetKey(inman.Inputs["S"])) ? 0f : (-1f)));
			num = (Input.GetKey(inman.Inputs["A"]) ? (-1f) : ((!Input.GetKey(inman.Inputs["D"])) ? 0f : 1f));
			cc.movementHor = num;
			cc.movementVer = num2;
			movementDirection = (num * base.transform.right + num2 * base.transform.forward).normalized;
			if (punch == null)
			{
				punch = GetComponentInChildren<FistControl>();
			}
			else if (!punch.enabled)
			{
				punch.YesFist();
			}
		}
		else
		{
			rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
			if (currentFallParticle != null)
			{
				Object.Destroy(currentFallParticle);
			}
			if (currentSlideParticle != null)
			{
				Object.Destroy(currentSlideParticle);
			}
			else if (slideScrape != null)
			{
				Object.Destroy(slideScrape);
			}
			if (punch == null)
			{
				punch = GetComponentInChildren<FistControl>();
			}
			else
			{
				punch.NoFist();
			}
		}
		if (dead && !endlessMode)
		{
			currentAllPitch -= 0.1f * Time.deltaTime;
			audmix.SetFloat("allPitch", currentAllPitch);
			if (blackColor.a < 0.5f)
			{
				blackColor.a += 0.75f * Time.deltaTime;
				youDiedColor.a += 0.75f * Time.deltaTime;
			}
			else
			{
				blackColor.a += 0.05f * Time.deltaTime;
				youDiedColor.a += 0.05f * Time.deltaTime;
			}
			blackScreen.color = blackColor;
			youDiedText.color = youDiedColor;
		}
		if (gc.onGround != pa.onGround)
		{
			pa.onGround = gc.onGround;
		}
		if (!gc.onGround)
		{
			if (fallTime < 1f)
			{
				fallTime += Time.deltaTime * 5f;
				if (fallTime > 1f)
				{
					falling = true;
				}
			}
			else if (rb.velocity.y < -2f)
			{
				fallSpeed = rb.velocity.y;
			}
		}
		else if (gc.onGround)
		{
			fallTime = 0f;
			clingFade = 0f;
		}
		if (!gc.onGround && rb.velocity.y < -20f)
		{
			aud3.pitch = rb.velocity.y * -1f / 120f;
			if (activated)
			{
				aud3.volume = rb.velocity.y * -1f / 80f;
			}
			else
			{
				aud3.volume = rb.velocity.y * -1f / 240f;
			}
		}
		else if (rb.velocity.y > -20f)
		{
			aud3.pitch = 0f;
			aud3.volume = 0f;
		}
		if (rb.velocity.y < -100f)
		{
			rb.velocity = new Vector3(rb.velocity.x, -100f, rb.velocity.z);
		}
		if (gc.onGround && falling && !jumpCooldown)
		{
			falling = false;
			slamStorage = false;
			if (fallSpeed > -50f)
			{
				aud2.clip = landingSound;
				aud2.volume = 0.5f + fallSpeed * -0.01f;
				aud2.Play();
			}
			else
			{
				Object.Instantiate(impactDust, gc.transform.position, Quaternion.identity).transform.forward = Vector3.up;
				cc.CameraShake(0.5f);
			}
			fallSpeed = 0f;
			gc.heavyFall = false;
			if (currentFallParticle != null)
			{
				Object.Destroy(currentFallParticle);
			}
		}
		if (!gc.onGround && activated && Input.GetKeyDown(inman.Inputs["Slide"]))
		{
			if (sliding)
			{
				StopSlide();
			}
			if (boost)
			{
				boostLeft = 0f;
				boost = false;
			}
			if (fallTime > 0.5f && !Physics.Raycast(gc.transform.position + base.transform.up, base.transform.up * -1f, out var _, 3f, lmask) && !gc.heavyFall)
			{
				stillHolding = true;
				rb.velocity = new Vector3(0f, -100f, 0f);
				falling = true;
				fallSpeed = -100f;
				gc.heavyFall = true;
				slamForce = 1f;
				if (currentFallParticle != null)
				{
					Object.Destroy(currentFallParticle);
				}
				currentFallParticle = Object.Instantiate(fallParticle, base.transform);
			}
		}
		if (gc.heavyFall && !slamStorage)
		{
			rb.velocity = new Vector3(0f, -100f, 0f);
		}
		if (gc.heavyFall || sliding)
		{
			Physics.IgnoreLayerCollision(2, 12, ignore: true);
		}
		else
		{
			Physics.IgnoreLayerCollision(2, 12, ignore: false);
		}
		if (!slopeCheck.onGround && !jumping && !boost && rb.velocity != Vector3.zero && Physics.Raycast(gc.transform.position + base.transform.up, base.transform.up * -1f, out var hitInfo2, 2f, lmask))
		{
			Vector3 target = new Vector3(base.transform.position.x, base.transform.position.y - hitInfo2.distance, base.transform.position.z);
			base.transform.position = Vector3.MoveTowards(base.transform.position, target, hitInfo2.distance * Time.deltaTime * 10f);
			if (rb.velocity.y > 0f)
			{
				rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
			}
		}
		if (gc.heavyFall)
		{
			slamForce += Time.deltaTime * 5f;
			if (Physics.Raycast(gc.transform.position + base.transform.up, base.transform.up * -1f, out var hitInfo3, 5f, lmask))
			{
				Breakable component = hitInfo3.collider.GetComponent<Breakable>();
				if (component != null && component.weak && !component.precisionOnly)
				{
					Object.Instantiate(impactDust, hitInfo3.point, Quaternion.identity);
					component.Break();
				}
			}
		}
		if (stillHolding && Input.GetKeyUp(inman.Inputs["Slide"]))
		{
			stillHolding = false;
		}
		if (activated)
		{
			if (Input.GetKeyDown(inman.Inputs["Jump"]) && (!falling || gc.canJump || wc.CheckForEnemyCols()) && !jumpCooldown)
			{
				Jump();
			}
			if (!gc.onGround && wc.onWall)
			{
				if (Physics.Raycast(base.transform.position, movementDirection, out var hitInfo4, 1f, lmask))
				{
					if (rb.velocity.y < -1f && !gc.heavyFall)
					{
						rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -1f, 1f), -2f * clingFade, Mathf.Clamp(rb.velocity.z, -1f, 1f));
						if (scrapeParticle == null)
						{
							scrapeParticle = Object.Instantiate(scrapePrefab, hitInfo4.point, Quaternion.identity);
						}
						scrapeParticle.transform.position = new Vector3(hitInfo4.point.x, hitInfo4.point.y + 1f, hitInfo4.point.z);
						scrapeParticle.transform.forward = hitInfo4.normal;
						clingFade = Mathf.MoveTowards(clingFade, 50f, Time.deltaTime * 4f);
					}
				}
				else if (scrapeParticle != null)
				{
					Object.Destroy(scrapeParticle);
					scrapeParticle = null;
				}
				if (Input.GetKeyDown(inman.Inputs["Jump"]) && !jumpCooldown && currentWallJumps < 3)
				{
					WallJump();
				}
			}
			else if (scrapeParticle != null)
			{
				Object.Destroy(scrapeParticle);
				scrapeParticle = null;
			}
		}
		if (Input.GetKeyDown(inman.Inputs["Slide"]) && gc.onGround && (!slowMode || crouching) && !sliding)
		{
			StartSlide();
		}
		if (Input.GetKeyDown(inman.Inputs["Slide"]) && !gc.onGround && !sliding && !jumping && activated && !slowMode && Physics.Raycast(gc.transform.position + base.transform.up, base.transform.up * -1f, out var _, 2f, lmask))
		{
			StartSlide();
		}
		if ((Input.GetKeyUp(inman.Inputs["Slide"]) || (slowMode && !crouching)) && sliding)
		{
			StopSlide();
		}
		if (sliding)
		{
			standing = false;
			slideLength += Time.deltaTime;
			if (cc.defaultPos.y != cc.originalPos.y - 0.625f)
			{
				Vector3 target2 = new Vector3(cc.originalPos.x, cc.originalPos.y - 0.625f, cc.originalPos.z);
				cc.defaultPos = Vector3.MoveTowards(cc.defaultPos, target2, (cc.defaultPos.y - target2.y + 0.5f) * Time.deltaTime * 20f);
			}
			if (currentSlideParticle != null)
			{
				currentSlideParticle.transform.position = base.transform.position + dodgeDirection * 10f;
			}
			if (slideSafety <= 0f)
			{
				if (new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude < 10f)
				{
					StopSlide();
				}
			}
			else if (slideSafety > 0f)
			{
				slideSafety -= Time.deltaTime * 5f;
			}
			if (gc.onGround)
			{
				slideScrape.transform.position = base.transform.position + dodgeDirection;
				cc.CameraShake(0.1f);
			}
			else
			{
				slideScrape.transform.position = Vector3.one * 5000f;
			}
		}
		else
		{
			if (!standing)
			{
				if ((bool)playerCollider && playerCollider.height != 3.5f)
				{
					if (!Physics.Raycast(base.transform.position, Vector3.up, 2.25f, lmask, QueryTriggerInteraction.Ignore))
					{
						playerCollider.height = 3.5f;
						if (Physics.Raycast(base.transform.position, Vector3.up * -1f, 2.25f, lmask, QueryTriggerInteraction.Ignore))
						{
							base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + 0.875f, base.transform.position.z);
						}
						else
						{
							base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y - 0.875f, base.transform.position.z);
							cc.defaultPos = cc.originalPos;
							standing = true;
						}
						if (crouching)
						{
							crouching = false;
							slowMode = false;
						}
					}
					else
					{
						crouching = true;
						slowMode = true;
					}
				}
				else if (cc.defaultPos.y != cc.originalPos.y)
				{
					cc.defaultPos = Vector3.MoveTowards(cc.defaultPos, cc.originalPos, (cc.originalPos.y - cc.defaultPos.y + 0.5f) * Time.deltaTime * 10f);
				}
				else
				{
					standing = true;
				}
			}
			if (currentSlideParticle != null)
			{
				Object.Destroy(currentSlideParticle);
			}
			if (slideScrape != null)
			{
				Object.Destroy(slideScrape);
			}
		}
		if (Input.GetKeyDown(inman.Inputs["Dodge"]) && activated && !slowMode)
		{
			if (boostCharge >= 100f)
			{
				if (sliding)
				{
					StopSlide();
				}
				boostLeft = 100f;
				boost = true;
				dodgeDirection = movementDirection;
				if (dodgeDirection == Vector3.zero)
				{
					dodgeDirection = base.transform.forward;
				}
				Quaternion identity = Quaternion.identity;
				identity.SetLookRotation(dodgeDirection * -1f);
				Object.Instantiate(dodgeParticle, base.transform.position + dodgeDirection * 10f, identity);
				if (!asscon.majorEnabled || !asscon.infiniteStamina)
				{
					boostCharge -= 100f;
				}
				if (dodgeDirection == base.transform.forward)
				{
					cc.dodgeDirection = 0;
				}
				else if (dodgeDirection == base.transform.forward * -1f)
				{
					cc.dodgeDirection = 1;
				}
				else
				{
					cc.dodgeDirection = 2;
				}
				aud.clip = dodgeSound;
				aud.volume = 1f;
				aud.pitch = 1f;
				aud.Play();
				if (gc.heavyFall)
				{
					fallSpeed = 0f;
					gc.heavyFall = false;
					if (currentFallParticle != null)
					{
						Object.Destroy(currentFallParticle);
					}
				}
			}
			else
			{
				Object.Instantiate(staminaFailSound);
			}
		}
		if (!walking && (num2 != 0f || num != 0f) && !sliding && gc.onGround)
		{
			walking = true;
			anim.SetBool("WalkF", value: true);
		}
		else if (walking && ((num2 == 0f && num == 0f) || !gc.onGround || sliding))
		{
			walking = false;
			anim.SetBool("WalkF", value: false);
		}
		if (hurting && hp > 0)
		{
			currentColor.a -= Time.deltaTime;
			hurtScreen.color = currentColor;
			if (currentColor.a <= 0f)
			{
				hurting = false;
			}
		}
		if (greenHpColor.a > 0f)
		{
			greenHpColor.a -= Time.deltaTime;
			greenHpFlash.color = greenHpColor;
		}
		if (boostCharge != 300f && !sliding && !slowMode)
		{
			if (boostCharge + 70f * Time.deltaTime < 300f)
			{
				boostCharge += 70f * Time.deltaTime;
			}
			else
			{
				boostCharge = 300f;
			}
		}
		Vector3 vector = hudOriginalPos - cc.transform.InverseTransformDirection(rb.velocity) / 1000f;
		float num3 = Vector3.Distance(vector, screenHud.transform.localPosition);
		screenHud.transform.localPosition = Vector3.MoveTowards(screenHud.transform.localPosition, vector, Time.deltaTime * 15f * num3);
		Vector3 vector2 = Vector3.ClampMagnitude(camOriginalPos - cc.transform.InverseTransformDirection(rb.velocity) / 350f * -1f, 0.2f);
		float num4 = Vector3.Distance(vector2, hudCam.transform.localPosition);
		hudCam.transform.localPosition = Vector3.MoveTowards(hudCam.transform.localPosition, vector2, Time.deltaTime * 25f * num4);
		if (antiHpCooldown > 0f)
		{
			antiHpCooldown = Mathf.MoveTowards(antiHpCooldown, 0f, Time.deltaTime);
		}
		else if (antiHp > 0f)
		{
			antiHp = Mathf.MoveTowards(antiHp, 0f, Time.deltaTime * 15f);
		}
	}

	private void FixedUpdate()
	{
		if (!boost)
		{
			Move();
			return;
		}
		rb.useGravity = true;
		Dodge();
	}

	private void Move()
	{
		if (!hurting)
		{
			base.gameObject.layer = 2;
			exploded = false;
		}
		if (gc.onGround && !jumping)
		{
			aud.pitch = 1f;
			currentWallJumps = 0;
			float y = rb.velocity.y;
			if (slopeCheck.onGround && movementDirection.x == 0f && movementDirection.z == 0f)
			{
				y = 0f;
				rb.useGravity = false;
			}
			else
			{
				rb.useGravity = true;
			}
			if (slowMode)
			{
				movementDirection2 = new Vector3(movementDirection.x * walkSpeed * Time.deltaTime * 1.25f, y, movementDirection.z * walkSpeed * Time.deltaTime * 1.25f);
			}
			else
			{
				movementDirection2 = new Vector3(movementDirection.x * walkSpeed * Time.deltaTime * 2.75f, y, movementDirection.z * walkSpeed * Time.deltaTime * 2.75f);
			}
			rb.velocity = Vector3.Lerp(rb.velocity, movementDirection2 + pushForce, 0.25f);
			anim.SetBool("Run", value: false);
		}
		else
		{
			rb.useGravity = true;
			if (slowMode)
			{
				movementDirection2 = new Vector3(movementDirection.x * walkSpeed * Time.deltaTime * 1.25f, rb.velocity.y, movementDirection.z * walkSpeed * Time.deltaTime * 1.25f);
			}
			else
			{
				movementDirection2 = new Vector3(movementDirection.x * walkSpeed * Time.deltaTime * 2.75f, rb.velocity.y, movementDirection.z * walkSpeed * Time.deltaTime * 2.75f);
			}
			airDirection.y = 0f;
			if ((movementDirection2.x > 0f && rb.velocity.x < movementDirection2.x) || (movementDirection2.x < 0f && rb.velocity.x > movementDirection2.x))
			{
				airDirection.x = movementDirection2.x;
			}
			else
			{
				airDirection.x = 0f;
			}
			if ((movementDirection2.z > 0f && rb.velocity.z < movementDirection2.z) || (movementDirection2.z < 0f && rb.velocity.z > movementDirection2.z))
			{
				airDirection.z = movementDirection2.z;
			}
			else
			{
				airDirection.z = 0f;
			}
			rb.AddForce(airDirection.normalized * airAcceleration);
		}
	}

	private void Dodge()
	{
		if (sliding)
		{
			rb.velocity = new Vector3(dodgeDirection.x * walkSpeed * Time.deltaTime * 4f, rb.velocity.y, dodgeDirection.z * walkSpeed * Time.deltaTime * 4f);
			return;
		}
		float y = 0f;
		if (slideEnding)
		{
			y = rb.velocity.y;
		}
		movementDirection2 = new Vector3(dodgeDirection.x * walkSpeed * Time.deltaTime * 2.75f, y, dodgeDirection.z * walkSpeed * Time.deltaTime * 2.75f);
		if (!slideEnding || gc.onGround)
		{
			rb.velocity = movementDirection2 * 3f;
		}
		base.gameObject.layer = 15;
		boostLeft -= 4f;
		if (boostLeft <= 0f)
		{
			boost = false;
			if (!gc.onGround && !slideEnding)
			{
				rb.velocity = movementDirection2;
			}
		}
		slideEnding = false;
	}

	private void Jump()
	{
		jumping = true;
		Invoke("NotJumping", 0.25f);
		falling = true;
		if (quakeJump)
		{
			Object.Instantiate(quakeJumpSound).GetComponent<AudioSource>().pitch = 1f + Random.Range(0f, 0.1f);
		}
		aud.clip = jumpSound;
		if (gc.superJumpChance > 0f)
		{
			aud.volume = 0.85f;
			aud.pitch = 2f;
		}
		else
		{
			aud.volume = 0.75f;
			aud.pitch = 1f;
		}
		aud.Play();
		rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
		if (sliding)
		{
			if (slowMode)
			{
				rb.AddForce(Vector3.up * jumpPower * 1500f);
			}
			else
			{
				rb.AddForce(Vector3.up * jumpPower * 1500f * 2f);
			}
			StopSlide();
		}
		else if (boost)
		{
			if (boostCharge >= 100f)
			{
				if (!asscon.majorEnabled || !asscon.infiniteStamina)
				{
					boostCharge -= 100f;
				}
				Object.Instantiate(dashJumpSound);
			}
			else
			{
				rb.velocity = new Vector3(movementDirection.x * walkSpeed * Time.deltaTime * 2.75f, 0f, movementDirection.z * walkSpeed * Time.deltaTime * 2.75f);
				Object.Instantiate(staminaFailSound);
			}
			if (slowMode)
			{
				rb.AddForce(Vector3.up * jumpPower * 1500f * 0.75f);
			}
			else
			{
				rb.AddForce(Vector3.up * jumpPower * 1500f * 1.5f);
			}
		}
		else if (slowMode)
		{
			rb.AddForce(Vector3.up * jumpPower * 1500f * 1.25f);
		}
		else if (gc.superJumpChance > 0f || gc.extraJumpChance > 0f)
		{
			if (slamForce < 5.5f)
			{
				rb.AddForce(Vector3.up * jumpPower * 1500f * (3f + (slamForce - 1f)));
			}
			else
			{
				rb.AddForce(Vector3.up * jumpPower * 1500f * 12.5f);
			}
			slamForce = 0f;
		}
		else
		{
			rb.AddForce(Vector3.up * jumpPower * 1500f * 2.5f);
		}
		jumpCooldown = true;
		Invoke("JumpReady", 0.2f);
		boost = false;
	}

	private void WallJump()
	{
		jumping = true;
		Invoke("NotJumping", 0.25f);
		boost = false;
		currentWallJumps++;
		if (gc.heavyFall)
		{
			slamStorage = true;
		}
		if (quakeJump)
		{
			Object.Instantiate(quakeJumpSound).GetComponent<AudioSource>().pitch = 1.1f + (float)currentWallJumps * 0.05f;
		}
		aud.clip = jumpSound;
		aud.pitch += 0.25f;
		aud.volume = 0.75f;
		aud.Play();
		if (currentWallJumps == 3)
		{
			aud2.clip = finalWallJump;
			aud2.volume = 0.75f;
			aud2.Play();
		}
		wallJumpPos = base.transform.position - wc.GetClosestPoint();
		rb.velocity = new Vector3(0f, 0f, 0f);
		Vector3 vector = new Vector3(wallJumpPos.normalized.x, 1f, wallJumpPos.normalized.z);
		rb.AddForce(vector * wallJumpPower * 2000f);
		jumpCooldown = true;
		Invoke("JumpReady", 0.1f);
	}

	public void Launch(Vector3 position, float strength, float maxDistance = 1f)
	{
		bool flag = false;
		if (jumping)
		{
			flag = true;
		}
		jumping = true;
		Invoke("NotJumping", 0.5f);
		jumpCooldown = true;
		Invoke("JumpReady", 0.2f);
		boost = false;
		if (gc.heavyFall)
		{
			fallSpeed = 0f;
			gc.heavyFall = false;
			if (currentFallParticle != null)
			{
				Object.Destroy(currentFallParticle);
			}
		}
		if (strength > 0f)
		{
			rb.velocity = Vector3.zero;
		}
		Vector3 normalized = (base.transform.position - position).normalized;
		int num = 1;
		Vector3 vector = ((!flag) ? new Vector3(normalized.x * (maxDistance - Vector3.Distance(base.transform.position, position)) * strength * 1000f, strength * 500f * (maxDistance - Vector3.Distance(base.transform.position, position)) * (float)num, normalized.z * (maxDistance - Vector3.Distance(base.transform.position, position)) * strength * 1000f) : new Vector3(normalized.x * maxDistance * strength * 1000f, strength * 500f * maxDistance * (float)num, normalized.z * maxDistance * strength * 1000f));
		rb.AddForce(Vector3.ClampMagnitude(vector, 1000000f));
	}

	private void JumpReady()
	{
		jumpCooldown = false;
	}

	public void GetHurt(int damage, bool invincible, float scoreLossMultiplier = 1f, bool explosion = false, bool instablack = false)
	{
		if (dead || (invincible && base.gameObject.layer == 15))
		{
			return;
		}
		if (explosion)
		{
			exploded = true;
		}
		if (asscon.majorEnabled)
		{
			damage = Mathf.RoundToInt((float)damage * asscon.damageTaken);
		}
		if (invincible)
		{
			base.gameObject.layer = 15;
		}
		if (damage >= 50)
		{
			currentColor.a = 0.8f;
		}
		else
		{
			currentColor.a = 0.5f;
		}
		hurting = true;
		cc.CameraShake(damage / 20);
		hurtAud.pitch = Random.Range(0.8f, 1f);
		hurtAud.PlayOneShot(hurtAud.clip);
		if (hp - damage > 0)
		{
			hp -= damage;
		}
		else
		{
			hp = 0;
		}
		if (invincible && scoreLossMultiplier != 0f && difficulty >= 2 && (!asscon.majorEnabled || !asscon.disableHardDamage) && hp <= 100)
		{
			float num = 0.35f;
			if (difficulty >= 3)
			{
				num = 0.5f;
			}
			if (antiHp + (float)damage * num < 99f)
			{
				antiHp += (float)damage * num;
			}
			else
			{
				antiHp = 99f;
			}
			if (antiHpCooldown == 0f)
			{
				antiHpCooldown += 1f;
			}
			if (difficulty >= 3)
			{
				antiHpCooldown += 1f;
			}
			antiHpCooldown += damage / 10;
		}
		if (shud == null)
		{
			shud = GetComponentInChildren<StyleHUD>();
		}
		if (scoreLossMultiplier > 0.5f)
		{
			shud.RemovePoints(0);
			shud.DescendRank();
			if (damage >= 20 && shud.currentRank >= 4 && scoreLossMultiplier >= 1f)
			{
				shud.DescendRank();
			}
		}
		else
		{
			shud.RemovePoints(Mathf.RoundToInt(damage));
		}
		StatsManager component = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
		if (damage <= 200)
		{
			component.stylePoints -= Mathf.RoundToInt((float)(damage * 5) * scoreLossMultiplier);
		}
		else
		{
			component.stylePoints -= Mathf.RoundToInt(1000f * scoreLossMultiplier);
		}
		component.tookDamage = true;
		if (hp != 0)
		{
			return;
		}
		if (!endlessMode)
		{
			blackScreen.gameObject.SetActive(value: true);
			if (instablack)
			{
				blackColor.a = 1f;
			}
			screenHud.SetActive(value: false);
		}
		rb.constraints = RigidbodyConstraints.None;
		ccAud.Play();
		cc.enabled = false;
		if (gunc == null)
		{
			gunc = GetComponentInChildren<GunControl>();
		}
		gunc.NoWeapon();
		rb.constraints = RigidbodyConstraints.None;
		dead = true;
		activated = false;
		if (punch == null)
		{
			punch = GetComponentInChildren<FistControl>();
		}
		punch.NoFist();
	}

	public void GetHealth(int health, bool silent)
	{
		if (dead || exploded)
		{
			return;
		}
		float num = health;
		float num2 = 100f;
		if (difficulty < 2)
		{
			num2 = 200f;
		}
		if (num < 1f)
		{
			num = 1f;
		}
		if ((float)hp <= num2)
		{
			if ((float)hp + num < num2 - (float)Mathf.RoundToInt(antiHp))
			{
				hp += Mathf.RoundToInt(num);
			}
			else if ((float)hp != num2 - (float)Mathf.RoundToInt(antiHp))
			{
				hp = 100 - Mathf.RoundToInt(antiHp);
			}
			greenHpColor.a = 1f;
			greenHpFlash.color = greenHpColor;
			if (!silent && health > 5)
			{
				if (greenHpAud == null)
				{
					greenHpAud = greenHpFlash.GetComponent<AudioSource>();
				}
				greenHpAud.Play();
			}
		}
		if (!silent && health > 5 && PlayerPrefs.GetInt("BlOn", 1) == 1)
		{
			Object.Instantiate(scrnBlood, fullHud.transform);
		}
	}

	public void SuperCharge()
	{
		GetHealth(100, silent: true);
		hp = 200;
	}

	public void Respawn()
	{
		if (sliding)
		{
			StopSlide();
		}
		hp = 100;
		boostCharge = 299f;
		antiHp = 0f;
		antiHpCooldown = 0f;
		rb.constraints = defaultRBConstraints;
		activated = true;
		blackScreen.gameObject.SetActive(value: false);
		cc.enabled = true;
		StatsManager component = GameObject.FindWithTag("RoomManager").GetComponent<StatsManager>();
		component.stylePoints = component.stylePoints / 3 * 2;
		if (gunc == null)
		{
			gunc = GetComponentInChildren<GunControl>();
		}
		gunc.YesWeapon();
		screenHud.SetActive(value: true);
		dead = false;
		blackColor.a = 0f;
		youDiedColor.a = 0f;
		currentAllPitch = 1f;
		blackScreen.color = blackColor;
		youDiedText.color = youDiedColor;
		audmix.SetFloat("allPitch", currentAllPitch);
		if (punch == null)
		{
			punch = GetComponentInChildren<FistControl>();
		}
		punch.YesFist();
		slowMode = false;
		GetComponentInChildren<WeaponCharges>().MaxCharges();
	}

	public void ResetHardDamage()
	{
		antiHp = 0f;
		antiHpCooldown = 0f;
	}

	private void NotJumping()
	{
		jumping = false;
	}

	private void StartSlide()
	{
		if (currentSlideParticle != null)
		{
			Object.Destroy(currentSlideParticle);
		}
		if (slideScrape != null)
		{
			Object.Destroy(slideScrape);
		}
		if (!crouching)
		{
			playerCollider.height = 1.25f;
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y - 0.625f, base.transform.position.z);
		}
		slideSafety = 1f;
		sliding = true;
		boost = true;
		dodgeDirection = movementDirection;
		if (dodgeDirection == Vector3.zero)
		{
			dodgeDirection = base.transform.forward;
		}
		Quaternion identity = Quaternion.identity;
		identity.SetLookRotation(dodgeDirection * -1f);
		currentSlideParticle = Object.Instantiate(slideParticle, base.transform.position + dodgeDirection * 10f, identity);
		slideScrape = Object.Instantiate(slideScrapePrefab, base.transform.position + dodgeDirection * 2f, identity);
		if (dodgeDirection == base.transform.forward)
		{
			cc.dodgeDirection = 0;
		}
		else if (dodgeDirection == base.transform.forward * -1f)
		{
			cc.dodgeDirection = 1;
		}
		else
		{
			cc.dodgeDirection = 2;
		}
	}

	private void StopSlide()
	{
		if (currentSlideParticle != null)
		{
			Object.Destroy(currentSlideParticle);
		}
		else if (slideScrape != null)
		{
			Object.Destroy(slideScrape);
		}
		Object.Instantiate(slideStopSound);
		cc.ResetToDefaultPos();
		sliding = false;
		slideEnding = true;
		if (slideLength > longestSlide)
		{
			longestSlide = slideLength;
		}
		slideLength = 0f;
	}

	public void EmptyStamina()
	{
		boostCharge = 0f;
	}

	public void FullStamina()
	{
		boostCharge = 300f;
	}
}
