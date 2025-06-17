using UnityEngine;

public class ThrownSword : MonoBehaviour
{
	public Vector3 targetPos;

	public Transform returnTransform;

	public bool active;

	public float speed;

	private bool returning;

	private bool calledReturn;

	public int type;

	public bool friendly;

	public bool deflected;

	private void Start()
	{
	}

	private void Update()
	{
		if (!active)
		{
			return;
		}
		if (type == 0)
		{
			if (!returning && base.transform.position != targetPos)
			{
				base.transform.position = Vector3.MoveTowards(base.transform.position, targetPos, Time.deltaTime * speed);
				if (base.transform.position == targetPos && !calledReturn)
				{
					calledReturn = true;
					CancelInvoke("Return");
					Invoke("Return", 1f);
				}
			}
			else if (returnTransform != null)
			{
				base.transform.position = Vector3.MoveTowards(base.transform.position, returnTransform.position, Time.deltaTime * speed);
				if (!(base.transform.position == returnTransform.position))
				{
					return;
				}
				SwordsMachine componentInParent = returnTransform.GetComponentInParent<SwordsMachine>();
				if (componentInParent != null)
				{
					if (!friendly)
					{
						componentInParent.SwordCatch();
					}
					else if (friendly)
					{
						componentInParent.Knockdown();
					}
				}
				Object.Destroy(base.gameObject);
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}
		else
		{
			if (type != 1)
			{
				return;
			}
			if (!returning)
			{
				base.transform.position += base.transform.parent.forward * 30f * Time.deltaTime;
				return;
			}
			if (base.transform.parent != null)
			{
				base.transform.parent = null;
			}
			if (returnTransform == null)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			base.transform.position = Vector3.MoveTowards(base.transform.position, returnTransform.position, Time.deltaTime * speed * 3f);
			if (!(base.transform.position == returnTransform.position))
			{
				return;
			}
			SwordsMachine componentInParent2 = returnTransform.GetComponentInParent<SwordsMachine>();
			if (componentInParent2 != null)
			{
				if (!friendly)
				{
					componentInParent2.SwordCatch();
				}
				else if (friendly)
				{
					componentInParent2.Knockdown();
				}
			}
			Object.Destroy(base.gameObject);
		}
	}

	public void SetPoints(Vector3 target, Transform origin)
	{
		targetPos = target;
		returnTransform = origin;
		active = true;
		if (type == 1)
		{
			Invoke("Return", 1f);
		}
		else
		{
			Invoke("Return", 2f);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			GetComponent<Collider>().enabled = false;
			other.gameObject.GetComponent<NewMovement>().GetHurt(30, invincible: true);
		}
		if (deflected && other.gameObject.layer == 8)
		{
			targetPos = other.ClosestPoint(base.transform.position);
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			if (base.transform.position == targetPos && !calledReturn)
			{
				calledReturn = true;
				CancelInvoke("Return");
				Invoke("Return", 1f);
			}
		}
	}

	private void Return()
	{
		if (!returning)
		{
			returning = true;
			if (type == 1)
			{
				GetComponent<Collider>().enabled = false;
			}
			else
			{
				base.transform.LookAt(returnTransform);
			}
		}
	}

	public void GetParried()
	{
		friendly = true;
		GetComponent<Collider>().enabled = false;
		Return();
	}
}
