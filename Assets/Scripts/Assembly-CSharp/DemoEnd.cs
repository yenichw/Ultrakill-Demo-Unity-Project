using UnityEngine;
using UnityEngine.UI;

public class DemoEnd : MonoBehaviour
{
	private Image img;

	private bool fade;

	public GameObject credits;

	public int timeToFade;

	public float fadeTime;

	private StatsManager sm;

	private void Start()
	{
		img = GetComponent<Image>();
		GameObject.FindWithTag("Player").GetComponent<NewMovement>().activated = false;
		GameObject.FindWithTag("Player").GetComponentInChildren<GunControl>().NoWeapon();
		GameObject.FindWithTag("Player").GetComponent<Rigidbody>().velocity = Vector3.zero;
		GameObject.FindWithTag("Player").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		Object.FindObjectOfType<CameraController>().enabled = false;
		Object.FindObjectOfType<OptionsManager>().inIntro = true;
		sm = Object.FindObjectOfType<StatsManager>();
		sm.HideShit();
		sm.StopTimer();
		sm.GetComponentInChildren<MusicManager>().off = true;
		GameProgressSaver.SaveProgress(sm.levelNumber + 1);
		Invoke("Stats", 6f);
	}

	private void Stats()
	{
		sm.fr.targetLevelName = "Credits";
		sm.SendInfo();
	}

	private void StartFade()
	{
		fade = true;
	}
}
