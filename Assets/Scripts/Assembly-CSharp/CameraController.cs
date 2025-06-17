using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class CameraController : MonoBehaviour
{
	public bool invert;

	public float minimumX = -89f;

	public float maximumX = 89f;

	public float minimumY = -360f;

	public float maximumY = 360f;

	public OptionsManager opm;

	public float scroll;

	public Vector3 originalPos;

	public Vector3 defaultPos;

	private Vector3 targetPos;

	public GameObject player;

	public NewMovement pm;

	private Camera cam;

	public bool activated;

	public GameObject gun;

	public float rotationY;

	public float rotationX;

	public bool reverseX;

	public bool reverseY;

	public float cameraShaking;

	public float movementHor;

	public float movementVer;

	public int dodgeDirection;

	public float additionalRotationY;

	public float additionalRotationX;

	public float defaultFov;

	public AudioMixer[] audmix;

	private bool mouseUnlocked;

	public bool slide;

	private AssistController asscon;

	private float slowDown = 1f;

	public float timeScale;

	public float timeScaleModifier = 1f;

	private Camera virtualCamera;

	private Camera hudCamera;

	public RenderTexture mainTargetMaterial;

	public RenderTexture ultraWideTargetMaterial;

	public RenderTexture hudTargetMaterial;

	public RenderTexture ultraWideHudTargetMaterial;

	private GameObject standardhud;

	private float aspectRatio;

	private bool pixeled;

	private bool tilt;

	private float currentStop;

	private bool zooming;

	private float zoomTarget;

	private void Start()
	{
		player = GameObject.FindWithTag("Player");
		pm = player.GetComponent<NewMovement>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		cam = GetComponent<Camera>();
		asscon = Object.FindObjectOfType<AssistController>();
		if (asscon.majorEnabled)
		{
			timeScale = asscon.gameSpeed;
		}
		else
		{
			timeScale = 1f;
		}
		Time.timeScale = timeScale * timeScaleModifier;
		gun = base.transform.GetChild(0).gameObject;
		originalPos = base.transform.localPosition;
		defaultPos = base.transform.localPosition;
		targetPos = new Vector3(defaultPos.x, defaultPos.y - 0.2f, defaultPos.z);
		cam.fieldOfView = PlayerPrefs.GetFloat("FOV", 105f);
		defaultFov = cam.fieldOfView;
		opm = GameObject.FindWithTag("RoomManager").GetComponent<OptionsManager>();
		virtualCamera = base.transform.Find("Virtual Camera").GetComponent<Camera>();
		hudCamera = base.transform.Find("HUD Camera").GetComponent<Camera>();
		standardhud = GetComponentInChildren<HudController>().gameObject;
		AudioMixer[] array = audmix;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetFloat("allPitch", 1f);
		}
		CheckPixelization();
		CheckColorCompression();
		CheckTilt();
	}

	private void Update()
	{
		if (asscon.majorEnabled && timeScale != asscon.gameSpeed)
		{
			timeScale = asscon.gameSpeed;
			Time.timeScale = timeScale * timeScaleModifier;
		}
		else if (!asscon.majorEnabled && timeScale != 1f)
		{
			timeScale = 1f;
			Time.timeScale = timeScale * timeScaleModifier;
		}
		if ((!pixeled && cam.aspect != aspectRatio) || (pixeled && virtualCamera.aspect != aspectRatio))
		{
			CheckAspectRatio();
		}
		if (player == null)
		{
			player = GameObject.FindWithTag("Player");
		}
		scroll = Input.GetAxis("Mouse ScrollWheel");
		if (activated)
		{
			float num = 1f;
			if (zooming)
			{
				num = cam.fieldOfView / defaultFov;
			}
			if (!reverseY)
			{
				rotationX += Input.GetAxis("Mouse Y") * (opm.mouseSensitivity / 10f) * num;
			}
			else
			{
				rotationX -= Input.GetAxis("Mouse Y") * (opm.mouseSensitivity / 10f) * num;
			}
			if (!reverseX)
			{
				rotationY += Input.GetAxis("Mouse X") * (opm.mouseSensitivity / 10f) * num;
			}
			else
			{
				rotationY -= Input.GetAxis("Mouse X") * (opm.mouseSensitivity / 10f) * num;
			}
		}
		if (rotationY > 180f)
		{
			rotationY -= 360f;
		}
		else if (rotationY < -180f)
		{
			rotationY += 360f;
		}
		rotationX = Mathf.Clamp(rotationX, minimumX + additionalRotationX, maximumX + additionalRotationX);
		if (zooming)
		{
			cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, zoomTarget, Time.deltaTime * 300f);
		}
		else if (pm.boost)
		{
			if (dodgeDirection == 0)
			{
				cam.fieldOfView = defaultFov - defaultFov / 20f;
			}
			else if (dodgeDirection == 1)
			{
				cam.fieldOfView = defaultFov + defaultFov / 10f;
			}
		}
		else
		{
			cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, defaultFov, Time.deltaTime * 300f);
		}
		if (zooming)
		{
			hudCamera.fieldOfView = Mathf.MoveTowards(hudCamera.fieldOfView, zoomTarget, Time.deltaTime * 300f);
		}
		else if (hudCamera.fieldOfView != 90f)
		{
			hudCamera.fieldOfView = Mathf.MoveTowards(hudCamera.fieldOfView, 90f, Time.deltaTime * 300f);
		}
		player.transform.localEulerAngles = new Vector3(0f, rotationY + additionalRotationY, 0f);
		float num2 = 0f;
		float num3 = movementHor * -1f;
		float num4 = base.transform.localEulerAngles.z;
		if (num4 > 180f)
		{
			num4 -= 360f;
		}
		num2 = ((!tilt) ? Mathf.MoveTowards(num4, 0f, Time.deltaTime * 25f * (Mathf.Abs(num4) + 0.01f)) : (pm.boost ? Mathf.MoveTowards(num4, num3 * 5f, Time.deltaTime * 100f * (Mathf.Abs(num4 - num3 * 5f) + 0.01f)) : Mathf.MoveTowards(num4, num3, Time.deltaTime * 25f * (Mathf.Abs(num4 - num3) + 0.01f))));
		base.transform.localEulerAngles = new Vector3(0f - rotationX + additionalRotationX, 0f, num2);
		if (cameraShaking > 0f)
		{
			if (cameraShaking > 1f)
			{
				base.transform.localPosition = new Vector3(defaultPos.x + (float)Random.Range(-1, 2), defaultPos.y + (float)Random.Range(-1, 2), defaultPos.z);
			}
			else
			{
				base.transform.localPosition = new Vector3(defaultPos.x + cameraShaking * Random.Range(-1f, 1f), defaultPos.y + cameraShaking * Random.Range(-1f, 1f), defaultPos.z);
			}
			cameraShaking -= Time.deltaTime * 3f;
		}
		else if (pm.walking && pm.standing)
		{
			base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, targetPos, Time.deltaTime * 0.5f);
			if (base.transform.localPosition == targetPos && targetPos != defaultPos)
			{
				targetPos = defaultPos;
			}
			else if (base.transform.localPosition == targetPos && targetPos == defaultPos)
			{
				targetPos = new Vector3(defaultPos.x, defaultPos.y - 0.1f, defaultPos.z);
			}
		}
		else
		{
			base.transform.localPosition = defaultPos;
			targetPos = new Vector3(defaultPos.x, defaultPos.y - 0.1f, defaultPos.z);
		}
	}

	private void FixedUpdate()
	{
		if (slowDown < timeScale * timeScaleModifier)
		{
			slowDown = Mathf.MoveTowards(slowDown, timeScale * timeScaleModifier, 0.02f);
			Time.timeScale = slowDown;
			AudioMixer[] array = audmix;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetFloat("allPitch", slowDown);
			}
		}
	}

	public void CameraShake(float shakeAmount)
	{
		float num = PlayerPrefs.GetFloat("ScrSha", 100f) / 100f;
		if (num != 0f && cameraShaking < shakeAmount * num)
		{
			cameraShaking = shakeAmount * num;
		}
	}

	public void StopShake()
	{
		cameraShaking = 0f;
	}

	public void HitStop(float length)
	{
		if (length > currentStop)
		{
			currentStop = length;
			Time.timeScale = 0f;
			StartCoroutine(TimeIsStopped(length, trueStop: false));
		}
	}

	public void TrueStop(float length)
	{
		if (length > currentStop)
		{
			currentStop = length;
			AudioMixer[] array = audmix;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetFloat("allPitch", 0f);
			}
			Time.timeScale = 0f;
			StartCoroutine(TimeIsStopped(length, trueStop: true));
		}
	}

	private IEnumerator TimeIsStopped(float length, bool trueStop)
	{
		yield return new WaitForSecondsRealtime(length);
		ContinueTime(length, trueStop);
	}

	private void ContinueTime(float length, bool trueStop)
	{
		if (!(length >= currentStop))
		{
			return;
		}
		Time.timeScale = timeScale * timeScaleModifier;
		if (trueStop)
		{
			AudioMixer[] array = audmix;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetFloat("allPitch", 1f);
			}
		}
		currentStop = 0f;
	}

	public void ResetCamera(float degrees, bool instant)
	{
		if (instant)
		{
			rotationY = degrees;
			rotationX = 0f;
		}
	}

	public void Zoom(float amount)
	{
		zooming = true;
		zoomTarget = amount;
	}

	public void StopZoom()
	{
		zooming = false;
	}

	public void ResetToDefaultPos()
	{
		base.transform.localPosition = defaultPos;
		targetPos = new Vector3(defaultPos.x, defaultPos.y - 0.1f, defaultPos.z);
	}

	public void SlowDown(float amount)
	{
		if (amount <= 0f)
		{
			amount = 0.01f;
		}
		slowDown = amount;
	}

	public void CheckPixelization()
	{
		if (virtualCamera == null)
		{
			cam = GetComponent<Camera>();
			virtualCamera = base.transform.Find("Virtual Camera").GetComponent<Camera>();
			hudCamera = base.transform.Find("HUD Camera").GetComponent<Camera>();
		}
		if (PlayerPrefs.GetInt("Pix", 1) == 0)
		{
			pixeled = false;
			virtualCamera.enabled = false;
			cam.targetTexture = null;
			cam.allowHDR = true;
			hudCamera.targetTexture = null;
			hudCamera.clearFlags = CameraClearFlags.Depth;
			return;
		}
		pixeled = true;
		virtualCamera.enabled = true;
		if (aspectRatio > 1.78f)
		{
			cam.targetTexture = ultraWideTargetMaterial;
			hudCamera.targetTexture = ultraWideHudTargetMaterial;
		}
		else
		{
			cam.targetTexture = mainTargetMaterial;
			hudCamera.targetTexture = hudTargetMaterial;
		}
		hudCamera.clearFlags = CameraClearFlags.Color;
	}

	public void CheckColorCompression()
	{
		switch (PlayerPrefs.GetInt("ColCom", 2))
		{
		case 0:
			Shader.SetGlobalInt("_ColorPrecision", 2048);
			break;
		case 1:
			Shader.SetGlobalInt("_ColorPrecision", 64);
			break;
		case 2:
			Shader.SetGlobalInt("_ColorPrecision", 32);
			break;
		case 3:
			Shader.SetGlobalInt("_ColorPrecision", 16);
			break;
		case 4:
			Shader.SetGlobalInt("_ColorPrecision", 8);
			break;
		case 5:
			Shader.SetGlobalInt("_ColorPrecision", 3);
			break;
		}
	}

	public void CheckAspectRatio()
	{
		if (virtualCamera == null)
		{
			cam = GetComponent<Camera>();
			virtualCamera = base.transform.Find("Virtual Camera").GetComponent<Camera>();
			hudCamera = base.transform.Find("HUD Camera").GetComponent<Camera>();
			standardhud = GetComponentInChildren<HudController>().gameObject;
		}
		if (!pixeled)
		{
			aspectRatio = cam.aspect;
		}
		else
		{
			aspectRatio = virtualCamera.aspect;
			if (aspectRatio > 1.78f)
			{
				cam.targetTexture = ultraWideTargetMaterial;
				hudCamera.targetTexture = ultraWideHudTargetMaterial;
				virtualCamera.transform.Find("Standard").gameObject.SetActive(value: false);
				virtualCamera.transform.Find("Ultrawide").gameObject.SetActive(value: true);
				virtualCamera.transform.Find("StandardHud").gameObject.SetActive(value: false);
				virtualCamera.transform.Find("UltrawideHud").gameObject.SetActive(value: true);
			}
			else
			{
				cam.targetTexture = mainTargetMaterial;
				hudCamera.targetTexture = hudTargetMaterial;
				virtualCamera.transform.Find("Standard").gameObject.SetActive(value: true);
				virtualCamera.transform.Find("Ultrawide").gameObject.SetActive(value: false);
				virtualCamera.transform.Find("StandardHud").gameObject.SetActive(value: true);
				virtualCamera.transform.Find("UltrawideHud").gameObject.SetActive(value: false);
			}
		}
		if (aspectRatio < 1.25f)
		{
			standardhud.transform.localScale = new Vector3(0.5f, 1f, 1f);
		}
		else if (aspectRatio < 1.45f)
		{
			standardhud.transform.localScale = new Vector3(0.75f, 1f, 1f);
		}
		else if (aspectRatio < 1.6f)
		{
			standardhud.transform.localScale = new Vector3(0.825f, 1f, 1f);
		}
		else if (aspectRatio < 1.7f)
		{
			standardhud.transform.localScale = new Vector3(0.9f, 1f, 1f);
		}
		else
		{
			standardhud.transform.localScale = Vector3.one;
		}
	}

	public void CheckTilt()
	{
		if (PlayerPrefs.GetInt("CamTil", 1) == 1)
		{
			tilt = true;
		}
		else
		{
			tilt = false;
		}
	}

	public void CheckMouseReverse()
	{
		if (PlayerPrefs.GetInt("RevMouX", 0) == 1)
		{
			reverseX = true;
		}
		else
		{
			reverseX = false;
		}
		if (PlayerPrefs.GetInt("RevMouY", 0) == 1)
		{
			reverseY = true;
		}
		else
		{
			reverseY = false;
		}
	}
}
