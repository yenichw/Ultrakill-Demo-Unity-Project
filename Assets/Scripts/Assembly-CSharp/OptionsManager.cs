using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
	public bool mainMenu;

	public bool paused;

	public bool inIntro;

	private float originalTimeScale;

	public GameObject pauseMenu;

	public GameObject optionsMenu;

	public GameObject progressChecker;

	public GameObject difficultySelect;

	public GameObject levelSelect;

	private CameraController cc;

	private NewMovement nm;

	private GunControl gc;

	private FistControl fc;

	public AudioMixer allAudio;

	public float mouseSensitivity;

	public float simplifiedDistance;

	public bool simplifyEnemies;

	public bool outlinesOnly;

	public Dropdown resolutionDropdown;

	private int screenWidth;

	private int screenHeight;

	public Toggle fullScreen;

	public float bloodstainChance;

	public float maxGore;

	public GameObject playerPosInfo;

	public bool dontUnpause;

	private CameraController camcon;

	private void Awake()
	{
		if (GameObject.FindWithTag("OptionsManager") == null)
		{
			Object.Instantiate(progressChecker);
		}
		camcon = Object.FindObjectOfType<CameraController>();
	}

	private void Start()
	{
		resolutionDropdown.captionText.text = Screen.width + "x" + Screen.height;
		fullScreen.isOn = Screen.fullScreen;
	}

	private void Update()
	{
		if (Input.GetButtonDown("Cancel") && !inIntro && !mainMenu)
		{
			if (!paused)
			{
				Pause();
			}
			else if (!dontUnpause)
			{
				CloseOptions();
				UnPause();
			}
		}
		if (mainMenu && !paused)
		{
			Pause();
		}
		if (paused)
		{
			if (mainMenu)
			{
				Time.timeScale = 1f;
			}
			else
			{
				Time.timeScale = 0f;
			}
		}
	}

	public void Pause()
	{
		if (cc == null)
		{
			cc = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
			nm = cc.pm;
			gc = nm.GetComponentInChildren<GunControl>();
			fc = nm.GetComponentInChildren<FistControl>();
		}
		if (!mainMenu)
		{
			nm.enabled = false;
			allAudio.SetFloat("allPitch", 0f);
		}
		cc.enabled = false;
		gc.activated = false;
		if (fc != null)
		{
			fc.activated = false;
		}
		originalTimeScale = camcon.timeScale * camcon.timeScaleModifier;
		paused = true;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		pauseMenu.SetActive(value: true);
	}

	public void UnPause()
	{
		CloseOptions();
		Cursor.lockState = CursorLockMode.Locked;
		paused = false;
		Time.timeScale = originalTimeScale;
		allAudio.SetFloat("allPitch", 1f);
		Cursor.visible = false;
		if (!nm.dead)
		{
			nm.enabled = true;
			cc.enabled = true;
			gc.activated = true;
			if (fc != null)
			{
				fc.activated = true;
			}
		}
		pauseMenu.SetActive(value: false);
	}

	public void RestartCheckpoint()
	{
		StatsManager component = GetComponent<StatsManager>();
		if (!component.infoSent)
		{
			component.Restart();
			UnPause();
		}
	}

	public void RestartMission()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		Object.Destroy(base.gameObject);
	}

	public void OpenOptions()
	{
		pauseMenu.SetActive(value: false);
		optionsMenu.SetActive(value: true);
	}

	public void CloseOptions()
	{
		pauseMenu.SetActive(value: true);
		optionsMenu.SetActive(value: false);
	}

	public void QuitMission()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene("Main Menu");
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	public void OpenDifficultySelect()
	{
		difficultySelect.SetActive(value: true);
		pauseMenu.SetActive(value: false);
	}

	public void CloseDifficultySelect()
	{
		difficultySelect.SetActive(value: false);
		pauseMenu.SetActive(value: true);
	}

	public void OpenLevelSelect()
	{
		levelSelect.SetActive(value: true);
		difficultySelect.SetActive(value: false);
	}

	public void CloseLevelSelect()
	{
		levelSelect.SetActive(value: false);
		difficultySelect.SetActive(value: true);
	}

	public void ResolutionChange()
	{
		Debug.Log("test " + resolutionDropdown.value);
		if (resolutionDropdown.value == 0)
		{
			screenWidth = 640;
			screenHeight = 480;
		}
		else if (resolutionDropdown.value == 1)
		{
			screenWidth = 1280;
			screenHeight = 720;
		}
		else if (resolutionDropdown.value == 2)
		{
			screenWidth = 1920;
			screenHeight = 1080;
		}
		Screen.SetResolution(screenWidth, screenHeight, fullScreen.isOn);
	}

	public void ChangeLevel(string levelname)
	{
		if (nm == null)
		{
			nm = Object.FindObjectOfType<NewMovement>();
		}
		PlayerPosInfo component = Object.Instantiate(playerPosInfo).GetComponent<PlayerPosInfo>();
		component.velocity = nm.GetComponent<Rigidbody>().velocity;
		component.wooshTime = nm.GetComponentInChildren<WallCheck>().GetComponent<AudioSource>().time;
		component.noPosition = true;
		SceneManager.LoadScene(levelname);
	}

	public void ChangeLevelAbrupt(string levelname)
	{
		SceneManager.LoadScene(levelname);
	}

	public void ChangeLevelWithPosition(string levelname)
	{
		if (nm == null)
		{
			nm = Object.FindObjectOfType<NewMovement>();
		}
		PlayerPosInfo component = Object.Instantiate(playerPosInfo).GetComponent<PlayerPosInfo>();
		component.velocity = nm.GetComponent<Rigidbody>().velocity;
		component.wooshTime = nm.GetComponentInChildren<WallCheck>().GetComponent<AudioSource>().time;
		component.noPosition = false;
		SceneManager.LoadScene(levelname);
	}
}
