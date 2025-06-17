using UnityEngine;

public class FakeMassActivator : MonoBehaviour
{
	private void OnEnable()
	{
		StatueIntroChecker statueIntroChecker = Object.FindObjectOfType<StatueIntroChecker>();
		base.transform.parent.GetComponentInChildren<MassAnimationReceiver>().GetComponent<Animator>().speed = 1f;
		if (statueIntroChecker != null && statueIntroChecker.beenSeen)
		{
			base.transform.parent.GetComponentInChildren<MassAnimationReceiver>().GetComponent<Animator>().Play("Intro", 0, 0.715f);
		}
		if (statueIntroChecker != null)
		{
			statueIntroChecker.beenSeen = true;
		}
	}
}
