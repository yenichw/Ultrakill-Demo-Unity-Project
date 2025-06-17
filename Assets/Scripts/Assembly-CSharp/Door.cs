using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
	public bool bigDoorController;

	private BigDoor[] bdoors;

	public bool open;

	public bool gotPos;

	public Vector3 closedPos;

	public Vector3 openPos;

	[HideInInspector]
	public Vector3 openPosRelative;

	public bool startOpen;

	private Vector3 targetPos;

	public float speed;

	private bool inPos = true;

	public bool reverseDirection;

	public int requests;

	private AudioSource aud;

	public AudioClip openSound;

	public AudioClip closeSound;

	private AudioSource aud2;

	private MaterialPropertyBlock block;

	public bool locked;

	public GameObject noPass;

	private NavMeshObstacle nmo;

	public GameObject[] activatedRooms;

	public GameObject[] deactivatedRooms;

	public Light openLight;

	private Door[] allDoors;

	public bool screenShake;

	private CameraController cc;

	private DoorLock thisLock;

	private List<DoorLock> locks = new List<DoorLock>();

	private int openLocks;

	[HideInInspector]
	public DoorController[] docons;

	[HideInInspector]
	public List<bool> origDoconStates = new List<bool>();

	public MeshRenderer lightsMeshRenderer;

	private Color lightsColor;

	private OcclusionPortal occpor;

	private void Awake()
	{
		block = new MaterialPropertyBlock();
	}

	private void Start()
	{
		nmo = GetComponent<NavMeshObstacle>();
		occpor = GetComponentInChildren<OcclusionPortal>();
		if (nmo != null)
		{
			nmo.enabled = false;
		}
		if (!bigDoorController)
		{
			aud = GetComponent<AudioSource>();
			if (!gotPos)
			{
				gotPos = true;
				closedPos = base.transform.localPosition;
				openPosRelative = base.transform.localPosition + openPos;
				if (startOpen)
				{
					base.transform.localPosition = openPosRelative;
				}
			}
			else if (openPosRelative == Vector3.zero)
			{
				openPosRelative = base.transform.localPosition + openPos;
			}
		}
		else
		{
			bdoors = GetComponentsInChildren<BigDoor>();
		}
		if (noPass != null)
		{
			aud2 = base.transform.GetChild(0).GetComponent<AudioSource>();
		}
		if (openLight != null && !startOpen)
		{
			openLight.enabled = false;
		}
		DoorLock[] componentsInChildren = GetComponentsInChildren<DoorLock>();
		if (componentsInChildren.Length != 0)
		{
			DoorLock[] array = componentsInChildren;
			foreach (DoorLock doorLock in array)
			{
				if (doorLock.gameObject == base.gameObject)
				{
					thisLock = doorLock;
					continue;
				}
				locks.Add(doorLock);
				doorLock.parentDoor = this;
			}
		}
		if (bigDoorController)
		{
			docons = GetComponentsInChildren<DoorController>();
		}
		else if (base.transform.parent != null)
		{
			docons = base.transform.parent.GetComponentsInChildren<DoorController>();
		}
		if (((componentsInChildren.Length != 0 && thisLock == null) || componentsInChildren.Length > 1) && docons.Length != 0)
		{
			for (int j = 0; j < docons.Length; j++)
			{
				if (docons[j].gameObject.activeInHierarchy)
				{
					origDoconStates.Add(item: true);
				}
				else
				{
					origDoconStates.Add(item: false);
				}
				docons[j].gameObject.SetActive(value: false);
			}
		}
		if ((bool)lightsMeshRenderer && lightsMeshRenderer.sharedMaterial.HasProperty(UKShaderProperties.EmissiveColor))
		{
			lightsMeshRenderer.GetPropertyBlock(block);
			lightsColor = lightsMeshRenderer.sharedMaterial.GetColor(UKShaderProperties.EmissiveColor);
			if (noPass != null && noPass.activeInHierarchy)
			{
				block.SetColor(UKShaderProperties.EmissiveColor, Color.red);
			}
			lightsMeshRenderer.SetPropertyBlock(block);
		}
		if (startOpen)
		{
			open = true;
			if ((bool)occpor)
			{
				occpor.open = true;
			}
		}
	}

	private void Update()
	{
		if (bigDoorController || inPos)
		{
			return;
		}
		base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, targetPos, Time.deltaTime * speed);
		if (screenShake)
		{
			if (cc == null)
			{
				cc = Object.FindObjectOfType<CameraController>();
			}
			cc.CameraShake(0.05f);
		}
		if (!(Vector3.Distance(base.transform.localPosition, targetPos) < 0.1f))
		{
			return;
		}
		base.transform.localPosition = targetPos;
		inPos = true;
		if (closeSound != null)
		{
			aud.clip = closeSound;
			aud.loop = false;
			aud.Play();
		}
		if (nmo != null)
		{
			if (base.transform.localPosition == closedPos)
			{
				nmo.enabled = true;
				if (openLight != null)
				{
					openLight.enabled = true;
				}
			}
			else
			{
				nmo.enabled = false;
			}
		}
		if ((bool)occpor && base.transform.localPosition == closedPos)
		{
			occpor.open = false;
		}
	}

	public void Open(bool enemy = false, bool skull = false)
	{
		if (!skull)
		{
			requests++;
		}
		else if (skull && docons.Length != 0)
		{
			bool flag = false;
			DoorController[] array = docons;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].gameObject.activeInHierarchy)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				requests++;
			}
		}
		if (!(requests == 1 || skull))
		{
			return;
		}
		open = true;
		if ((bool)occpor)
		{
			occpor.open = true;
		}
		if (!enemy)
		{
			allDoors = Object.FindObjectsOfType<Door>();
			Door[] array2 = allDoors;
			foreach (Door door in array2)
			{
				if (door != null && door != this && door.transform.localPosition != door.closedPos && !door.startOpen)
				{
					DoorController doorController = null;
					if (door.bigDoorController)
					{
						doorController = door.GetComponentInChildren<DoorController>();
					}
					else if (base.transform.parent != null)
					{
						doorController = door.transform.parent.GetComponentInChildren<DoorController>();
					}
					if (doorController != null && doorController.type == 0 && !doorController.enemyIn)
					{
						door.Close();
					}
				}
			}
		}
		if (!bigDoorController)
		{
			if (aud == null)
			{
				aud = GetComponent<AudioSource>();
			}
			aud.clip = openSound;
			if (closeSound != null)
			{
				aud.loop = true;
			}
			aud.Play();
			targetPos = openPosRelative;
			inPos = false;
		}
		else
		{
			BigDoor[] array3 = bdoors;
			foreach (BigDoor bigDoor in array3)
			{
				if (reverseDirection)
				{
					bigDoor.reverseDirection = true;
				}
				else
				{
					bigDoor.reverseDirection = false;
				}
				bigDoor.Open();
			}
		}
		if (activatedRooms.Length != 0)
		{
			GameObject[] array4 = activatedRooms;
			for (int i = 0; i < array4.Length; i++)
			{
				array4[i].SetActive(value: true);
			}
		}
		if (openLight != null)
		{
			openLight.enabled = true;
		}
		if (thisLock != null)
		{
			thisLock.Open();
		}
	}

	public void Optimize()
	{
		if (deactivatedRooms.Length != 0)
		{
			GameObject[] array = deactivatedRooms;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
		}
	}

	public void Close(bool force = false)
	{
		if (!gotPos && !bigDoorController)
		{
			gotPos = true;
			closedPos = base.transform.localPosition;
			openPosRelative = base.transform.localPosition + openPos;
			if (startOpen)
			{
				base.transform.localPosition = openPosRelative;
			}
		}
		if (requests > 1 && !force)
		{
			requests--;
			return;
		}
		open = false;
		if (requests > 0 && !force)
		{
			requests--;
		}
		else if (force)
		{
			requests = 0;
		}
		if (startOpen)
		{
			startOpen = false;
		}
		if (!bigDoorController)
		{
			if (aud == null)
			{
				aud = GetComponent<AudioSource>();
			}
			if (aud != null)
			{
				aud.clip = openSound;
				aud.Play();
			}
			targetPos = closedPos;
			inPos = false;
		}
		else if (bdoors != null)
		{
			BigDoor[] array = bdoors;
			foreach (BigDoor bigDoor in array)
			{
				bigDoor.Close();
				if (openLight != null)
				{
					bigDoor.openLight = openLight;
				}
			}
		}
		if (thisLock != null)
		{
			thisLock.Close();
		}
	}

	public void Lock()
	{
		if (locked)
		{
			return;
		}
		locked = true;
		noPass.SetActive(value: true);
		if (!bigDoorController)
		{
			if (base.transform.localPosition != closedPos)
			{
				Close();
			}
		}
		else
		{
			BigDoor[] array = bdoors;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].open)
				{
					Close();
					break;
				}
			}
		}
		aud2.pitch = 0.2f;
		aud2.Play();
		if ((bool)lightsMeshRenderer && lightsMeshRenderer.sharedMaterial.HasProperty(UKShaderProperties.EmissiveColor))
		{
			lightsMeshRenderer.GetPropertyBlock(block);
			block.SetColor(UKShaderProperties.EmissiveColor, Color.red);
			lightsMeshRenderer.SetPropertyBlock(block);
		}
	}

	public void Unlock()
	{
		if (locked)
		{
			aud2.pitch = 0.5f;
			aud2.Play();
		}
		locked = false;
		noPass.SetActive(value: false);
		if ((bool)lightsMeshRenderer && lightsMeshRenderer.sharedMaterial.HasProperty(UKShaderProperties.EmissiveColor))
		{
			lightsMeshRenderer.GetPropertyBlock(block);
			block.SetColor(UKShaderProperties.EmissiveColor, lightsColor);
			lightsMeshRenderer.SetPropertyBlock(block);
		}
	}

	public void LockOpen()
	{
		openLocks++;
		if (openLocks != locks.Count)
		{
			return;
		}
		if (docons.Length != 0)
		{
			for (int i = 0; i < docons.Length; i++)
			{
				if (origDoconStates[i])
				{
					docons[i].gameObject.SetActive(value: true);
				}
			}
		}
		Open(enemy: false, skull: true);
	}

	public void LockClose()
	{
		openLocks--;
		if (openLocks == locks.Count - 1)
		{
			Close(force: true);
		}
	}
}
