using UnityEngine;
using cakeslice;

public class EnemySimplifier : MonoBehaviour
{
	private SkinnedMeshRenderer smr;

	private MeshRenderer mr;

	[HideInInspector]
	public Material originalMaterial;

	public Material simplifiedMaterial;

	[HideInInspector]
	public Outline oline;

	private OptionsManager oman;

	private GameObject player;

	[HideInInspector]
	public LayerMask lmask;

	private bool active = true;

	private void Start()
	{
		lmask = (int)lmask | 0x100;
		lmask = (int)lmask | 0x1000000;
		oline = GetComponentInChildren<Outline>();
		oline.enabled = false;
		smr = oline.GetComponent<SkinnedMeshRenderer>();
		if (smr == null)
		{
			mr = oline.GetComponentInChildren<MeshRenderer>();
			if (originalMaterial == null)
			{
				originalMaterial = mr.material;
			}
		}
		else if (originalMaterial == null)
		{
			originalMaterial = smr.material;
		}
		oman = GameObject.FindWithTag("RoomManager").GetComponent<OptionsManager>();
		player = GameObject.FindWithTag("Player");
	}

	private void Update()
	{
		if (active)
		{
			if (smr != null)
			{
				SkinnedVer();
			}
			else
			{
				NormalVer();
			}
		}
	}

	public void SkinnedVer()
	{
		if (!oman.outlinesOnly && simplifiedMaterial != null && oline != null)
		{
			if (smr.sharedMaterial != simplifiedMaterial && oman.simplifyEnemies && Vector3.Distance(base.transform.position, player.transform.position) > oman.simplifiedDistance)
			{
				smr.sharedMaterial = simplifiedMaterial;
			}
			else if (smr.sharedMaterial == simplifiedMaterial && oman.simplifyEnemies && Vector3.Distance(base.transform.position, player.transform.position) > oman.simplifiedDistance)
			{
				Vector3 vector = new Vector3(base.transform.position.x, base.transform.position.y + 3f, base.transform.position.z);
				Vector3 vector2 = new Vector3(player.transform.position.x, player.transform.position.y + 0.6f, player.transform.position.z);
				if (!Physics.Raycast(vector, vector2 - vector, Vector3.Distance(vector, vector2), lmask))
				{
					oline.enabled = true;
				}
				else
				{
					oline.enabled = false;
				}
			}
			else if (smr.sharedMaterial == simplifiedMaterial && (!oman.simplifyEnemies || Vector3.Distance(base.transform.position, player.transform.position) < oman.simplifiedDistance))
			{
				smr.sharedMaterial = originalMaterial;
				oline.enabled = false;
			}
		}
		else
		{
			if (!(oline != null))
			{
				return;
			}
			if (smr.sharedMaterial == simplifiedMaterial)
			{
				smr.sharedMaterial = originalMaterial;
			}
			if (oman.simplifyEnemies && Vector3.Distance(base.transform.position, player.transform.position) > oman.simplifiedDistance)
			{
				Vector3 vector3 = new Vector3(base.transform.position.x, base.transform.position.y + 3f, base.transform.position.z);
				Vector3 vector4 = new Vector3(player.transform.position.x, player.transform.position.y + 0.6f, player.transform.position.z);
				if (!Physics.Raycast(vector3, vector4 - vector3, Vector3.Distance(vector3, vector4), lmask))
				{
					oline.enabled = true;
				}
				else
				{
					oline.enabled = false;
				}
			}
			else if (oline.enabled && (!oman.simplifyEnemies || Vector3.Distance(base.transform.position, player.transform.position) < oman.simplifiedDistance))
			{
				oline.enabled = false;
			}
		}
	}

	public void NormalVer()
	{
		if (!oman.outlinesOnly && simplifiedMaterial != null && oline != null)
		{
			if (mr.sharedMaterial != simplifiedMaterial && oman.simplifyEnemies && Vector3.Distance(base.transform.position, player.transform.position) > oman.simplifiedDistance)
			{
				mr.sharedMaterial = simplifiedMaterial;
			}
			else if (mr.sharedMaterial == simplifiedMaterial && oman.simplifyEnemies && Vector3.Distance(base.transform.position, player.transform.position) > oman.simplifiedDistance)
			{
				Vector3 vector = new Vector3(base.transform.position.x, base.transform.position.y + 3f, base.transform.position.z);
				Vector3 vector2 = new Vector3(player.transform.position.x, player.transform.position.y + 0.6f, player.transform.position.z);
				if (!Physics.Raycast(vector, vector2 - vector, Vector3.Distance(vector, vector2), lmask))
				{
					oline.enabled = true;
				}
				else
				{
					oline.enabled = false;
				}
			}
			else if (mr.sharedMaterial == simplifiedMaterial && (!oman.simplifyEnemies || Vector3.Distance(base.transform.position, player.transform.position) < oman.simplifiedDistance))
			{
				mr.sharedMaterial = originalMaterial;
				oline.enabled = false;
			}
		}
		else
		{
			if (!(oline != null))
			{
				return;
			}
			if (mr.sharedMaterial == simplifiedMaterial)
			{
				mr.sharedMaterial = originalMaterial;
			}
			if (oman.simplifyEnemies && Vector3.Distance(base.transform.position, player.transform.position) > oman.simplifiedDistance)
			{
				Vector3 vector3 = new Vector3(base.transform.position.x, base.transform.position.y + 3f, base.transform.position.z);
				Vector3 vector4 = new Vector3(player.transform.position.x, player.transform.position.y + 0.6f, player.transform.position.z);
				if (!Physics.Raycast(vector3, vector4 - vector3, Vector3.Distance(vector3, vector4), lmask))
				{
					oline.enabled = true;
				}
				else
				{
					oline.enabled = false;
				}
			}
			else if (oline.enabled && (!oman.simplifyEnemies || Vector3.Distance(base.transform.position, player.transform.position) < oman.simplifiedDistance))
			{
				oline.enabled = false;
			}
		}
	}

	public void Begone()
	{
		active = false;
		oline.enabled = false;
		if (smr != null)
		{
			smr.sharedMaterial = originalMaterial;
		}
		else
		{
			mr.sharedMaterial = originalMaterial;
		}
		Object.Destroy(this);
	}
}
