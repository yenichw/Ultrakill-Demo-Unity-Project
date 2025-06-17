using System.Collections;
using UnityEngine;

public class DebugArm : MonoBehaviour
{
	[HideInInspector]
	public GunControl gunCtrl;

	[HideInInspector]
	public CameraController cameraCtrl;

	[SerializeField]
	private LayerMask raycastLayers;

	[SerializeField]
	private Transform holder;

	[SerializeField]
	private Animator armAnimator;

	[SerializeField]
	private AudioSource jabSound;

	[SerializeField]
	private AudioSource selectSound;

	private SpawnMenu menu;

	private GameObject currentPreview;

	private SpawnableObject currentObject;

	private GoreZone goreZone;

	private static readonly int Holding = Animator.StringToHash("Holding");

	private static readonly int Punch = Animator.StringToHash("Punch");

	private void Awake()
	{
		menu = base.transform.parent.parent.parent.GetComponentInChildren<SpawnMenu>(includeInactive: true);
		menu.arm = this;
	}

	private GoreZone GetGoreZone()
	{
		if (!goreZone)
		{
			goreZone = new GameObject("Debug Gore Zone").AddComponent<GoreZone>();
		}
		return goreZone;
	}

	public void PreviewObject(SpawnableObject obj)
	{
		selectSound.Play();
		currentObject = obj;
		GameObject gameObject = Object.Instantiate(obj.preview, holder, worldPositionStays: false);
		menu.gameObject.SetActive(value: false);
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		cameraCtrl.enabled = true;
		gunCtrl.enabled = true;
		if ((bool)currentPreview)
		{
			Object.Destroy(currentPreview);
		}
		currentPreview = gameObject;
	}

	private void OnEnable()
	{
		armAnimator.SetBool(Holding, value: true);
		holder.gameObject.SetActive(value: true);
	}

	private IEnumerator HandClosedAnimationThing()
	{
		holder.gameObject.SetActive(value: false);
		yield return new WaitForSeconds(0.85f);
		holder.gameObject.SetActive(value: true);
	}

	private void Update()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		if (Input.GetButtonDown("Fire1"))
		{
			if (menu.gameObject.activeSelf || currentObject == null)
			{
				return;
			}
			StopAllCoroutines();
			StartCoroutine(HandClosedAnimationThing());
			jabSound.Play();
			armAnimator.SetTrigger(Punch);
			if (Physics.Raycast(cameraCtrl.transform.position, cameraCtrl.transform.forward, out var hitInfo, 50f, raycastLayers))
			{
				Object.Instantiate(currentObject.gameObject, hitInfo.point, Quaternion.identity, GetGoreZone().transform);
			}
		}
		if (Input.GetButtonDown("Fire2"))
		{
			menu.gameObject.SetActive(value: true);
			cameraCtrl.enabled = false;
			gunCtrl.enabled = false;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}
}
