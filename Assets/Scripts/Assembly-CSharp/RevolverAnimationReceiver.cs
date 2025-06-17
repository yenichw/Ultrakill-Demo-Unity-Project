using UnityEngine;

public class RevolverAnimationReceiver : MonoBehaviour
{
	private Revolver rev;

	private void Start()
	{
		rev = GetComponentInParent<Revolver>();
	}

	public void ReadyGun()
	{
		rev.ReadyGun();
	}
}
