using UnityEngine;

public class Turn : MonoBehaviour
{
	public Vector3 angle;

	public float speed;

	public Vector3 target;

	private Vector3 tempRot;

	private AudioSource aud;

	public bool turning;

	public float spinSpeed;

	private bool spinning;

	public Vector3 totalRot;

	private Vector3 origRot;

	private void Start()
	{
		origRot = base.transform.localRotation.eulerAngles;
		target = base.transform.localRotation.eulerAngles;
		tempRot = base.transform.localRotation.eulerAngles;
		totalRot = base.transform.localRotation.eulerAngles;
		aud = GetComponent<AudioSource>();
	}

	private void LateUpdate()
	{
		if (turning && spinSpeed > 10f)
		{
			spinSpeed = Mathf.MoveTowards(spinSpeed, 10f, Time.deltaTime * 150f);
		}
		else
		{
			turning = false;
		}
		if (spinSpeed > 20f)
		{
			spinning = true;
			base.transform.localRotation = Quaternion.Euler(totalRot);
			base.transform.Rotate(angle, spinSpeed * 10f * Time.deltaTime, Space.Self);
			totalRot = base.transform.localRotation.eulerAngles;
		}
		else if (spinning && (!turning || spinSpeed <= 10f))
		{
			if (Quaternion.Angle(base.transform.localRotation, Quaternion.Euler(tempRot)) > 5f)
			{
				base.transform.localRotation = Quaternion.RotateTowards(base.transform.localRotation, Quaternion.Euler(tempRot), Quaternion.Angle(base.transform.localRotation, Quaternion.Euler(tempRot)) * 0.1f * speed * Time.deltaTime);
			}
			else
			{
				aud.Play();
				spinning = false;
			}
			totalRot = base.transform.localRotation.eulerAngles;
		}
	}

	public void DoTurn()
	{
		turning = true;
		spinSpeed = 100f;
	}
}
