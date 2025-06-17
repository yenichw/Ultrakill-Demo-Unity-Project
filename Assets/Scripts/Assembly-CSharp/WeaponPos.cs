using UnityEngine;

public class WeaponPos : MonoBehaviour
{
	private bool ready;

	public Vector3 currentDefault;

	private Vector3 defaultPos;

	private Vector3 defaultRot;

	private Vector3 defaultScale;

	public Vector3 middlePos;

	public Vector3 middleRot;

	public Vector3 middleScale;

	private void Start()
	{
		CheckPosition();
	}

	private void OnEnable()
	{
		CheckPosition();
	}

	public void CheckPosition()
	{
		if (!ready)
		{
			ready = true;
			defaultPos = base.transform.localPosition;
			defaultRot = base.transform.localRotation.eulerAngles;
			defaultScale = base.transform.localScale;
			if (middleScale == Vector3.zero)
			{
				middleScale = defaultScale;
			}
			if (middleRot == Vector3.zero)
			{
				middleRot = defaultRot;
			}
		}
		if (PlayerPrefs.GetInt("HoldPos", 0) == 1)
		{
			base.transform.localPosition = middlePos;
			base.transform.localRotation = Quaternion.Euler(middleRot);
			base.transform.localScale = middleScale;
		}
		else
		{
			base.transform.localPosition = defaultPos;
			base.transform.localRotation = Quaternion.Euler(defaultRot);
			base.transform.localScale = defaultScale;
		}
		currentDefault = base.transform.localPosition;
	}
}
