using UnityEngine;

public class BlackHoleCannon : MonoBehaviour
{
	public Transform kickBackPos;

	public Transform pickUpPos;

	public Transform shootPoint;

	private Vector3 origPos;

	private Quaternion origRot;

	private bool gunReady;

	public GameObject bh;

	private GameObject currentbh;

	public LayerMask lmask;

	private RaycastHit rhit;

	private GameObject cam;

	private CameraController cc;

	private AudioSource aud;

	private WeaponHUD whud;

	private GunControl gc;

	public AudioClip emptyClick;

	private void Start()
	{
		origPos = base.transform.localPosition;
		origRot = base.transform.localRotation;
		base.transform.localPosition = pickUpPos.localPosition;
		base.transform.localRotation = pickUpPos.localRotation;
		cam = Camera.main.gameObject;
		cc = cam.GetComponent<CameraController>();
		aud = GetComponent<AudioSource>();
		gc = GetComponentInParent<GunControl>();
	}

	private void OnDisable()
	{
		base.transform.localPosition = pickUpPos.localPosition;
		base.transform.localRotation = pickUpPos.localRotation;
	}

	private void Update()
	{
		if (whud == null)
		{
			whud = Camera.main.GetComponentInChildren<WeaponHUD>();
		}
		else if (whud.currentWeapon != 2 || whud.currentVariation != 2)
		{
			whud.UpdateImage(3, 2);
		}
		if (!gunReady && base.transform.localPosition == origPos && base.transform.localRotation == origRot)
		{
			gunReady = true;
		}
		if (base.transform.localPosition != origPos)
		{
			gunReady = false;
			base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, origPos, Time.deltaTime * 1.5f);
		}
		if (base.transform.localRotation != origRot)
		{
			gunReady = false;
			base.transform.localRotation = Quaternion.RotateTowards(base.transform.localRotation, origRot, Time.deltaTime * 80f);
		}
		if (gunReady && Input.GetButtonDown("Fire1"))
		{
			if (gc.killCharge == 10f)
			{
				if (Physics.Raycast(cam.transform.position, cam.transform.forward, out rhit, float.PositiveInfinity, lmask))
				{
					Shoot();
				}
			}
			else
			{
				aud.PlayOneShot(emptyClick, 1f);
			}
		}
		if (currentbh != null && Input.GetButtonDown("Fire2"))
		{
			currentbh.GetComponent<BlackHoleProjectile>().Activate();
		}
	}

	private void Shoot()
	{
		gunReady = false;
		gc.ClearKills();
		Vector3 position = cam.transform.position + cam.transform.forward;
		currentbh = Object.Instantiate(bh, position, cam.transform.rotation);
		currentbh.transform.LookAt(rhit.point);
		base.transform.localPosition = kickBackPos.localPosition;
		base.transform.localRotation = kickBackPos.localRotation;
		aud.Play();
		cc.CameraShake(0.5f);
	}
}
