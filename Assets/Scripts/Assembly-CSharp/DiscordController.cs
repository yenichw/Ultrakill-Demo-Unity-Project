using System;
using Discord;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DiscordController : MonoBehaviour
{
	public static DiscordController Instance;

	[SerializeField]
	private long discordClientId;

	[Space]
	[SerializeField]
	private SerializedActivityAssets missingActivityAssets;

	[SerializeField]
	private RankIcon[] rankIcons;

	[SerializeField]
	private string[] diffNames;

	private global::Discord.Discord discord;

	private ActivityManager activityManager;

	private int lastPoints;

	private bool disabled;

	private Activity cachedActivity;

	private void Awake()
	{
		if ((bool)Instance)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		base.transform.SetParent(null);
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		bool flag = PlayerPrefs.GetInt("DisImp", 1) == 1;
		if (flag)
		{
			Enable();
		}
		disabled = !flag;
		SceneManager.sceneLoaded += SceneManager_sceneLoaded;
	}

	private void Update()
	{
		if (discord != null && !disabled)
		{
			discord.RunCallbacks();
		}
	}

	private void OnApplicationQuit()
	{
		if (discord != null)
		{
			discord.Dispose();
		}
	}

	public static void UpdateRank(int rank)
	{
		if ((bool)Instance && !Instance.disabled)
		{
			if (Instance.rankIcons.Length <= rank)
			{
				Debug.LogError("Discord Controller is missing rank names/icons!");
				return;
			}
			Instance.cachedActivity.Assets.SmallText = Instance.rankIcons[rank].Text;
			Instance.cachedActivity.Assets.SmallImage = Instance.rankIcons[rank].Image;
			Instance.SendActivity();
		}
	}

	public static void UpdateStyle(int points)
	{
		if ((bool)Instance && !Instance.disabled && Instance.lastPoints != points)
		{
			Instance.lastPoints = points;
			Instance.cachedActivity.Details = "STYLE: " + points;
			Instance.SendActivity();
		}
	}

	public static void UpdateWave(int wave)
	{
		if ((bool)Instance && !Instance.disabled && Instance.lastPoints != wave)
		{
			Instance.lastPoints = wave;
			Instance.cachedActivity.Details = "WAVE: " + wave;
			Instance.SendActivity();
		}
	}

	public static void Disable()
	{
		if ((bool)Instance && Instance.discord != null)
		{
			Instance.disabled = true;
			Instance.activityManager.ClearActivity(delegate
			{
			});
		}
	}

	public static void Enable()
	{
		if ((bool)Instance)
		{
			if (Instance.discord == null)
			{
				Instance.discord = new global::Discord.Discord(Instance.discordClientId, 1uL);
				Instance.activityManager = Instance.discord.GetActivityManager();
			}
			Instance.disabled = false;
			Instance.ResetActivityCache();
			Instance.FetchSceneActivity(SceneManager.GetActiveScene().name);
		}
	}

	private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (mode != LoadSceneMode.Additive)
		{
			FetchSceneActivity(scene.name);
		}
	}

	private void ResetActivityCache()
	{
		cachedActivity = new Activity
		{
			State = "LOADING",
			Assets = 
			{
				LargeImage = "generic",
				LargeText = "LOADING"
			},
			Instance = true
		};
	}

	private void FetchSceneActivity(string scene)
	{
		ResetActivityCache();
		LevelActivity levelActivity = UnityEngine.Object.FindObjectOfType<LevelActivity>();
		if ((bool)levelActivity)
		{
			cachedActivity.Assets = levelActivity.Assets.Deserialize();
		}
		else
		{
			cachedActivity.Assets = missingActivityAssets.Deserialize();
		}
		if (scene == "Main Menu")
		{
			cachedActivity.State = "In Main Menu";
		}
		else
		{
			cachedActivity.State = "DIFFICULTY: " + diffNames[PlayerPrefs.GetInt("Diff", 0)];
		}
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		long start = (long)(DateTime.UtcNow - dateTime).TotalSeconds;
		cachedActivity.Timestamps = new ActivityTimestamps
		{
			Start = start
		};
		SendActivity();
	}

	private void SendActivity()
	{
		if (discord != null && activityManager != null)
		{
			activityManager.UpdateActivity(cachedActivity, delegate
			{
			});
		}
	}
}
