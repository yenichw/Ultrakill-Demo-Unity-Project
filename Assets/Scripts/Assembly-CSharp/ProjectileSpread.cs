using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpread : MonoBehaviour
{
	private GameObject projectile;

	public float spreadAmount;

	public int projectileAmount;

	public bool parried;

	[HideInInspector]
	public List<EnemyIdentifier> hitEnemies = new List<EnemyIdentifier>();

	private void Start()
	{
		projectile = GetComponentInChildren<Projectile>().gameObject;
		GameObject gameObject = new GameObject();
		gameObject.transform.position = base.transform.position;
		gameObject.transform.rotation = base.transform.rotation;
		for (int i = 0; i <= projectileAmount; i++)
		{
			GameObject obj = Object.Instantiate(projectile, gameObject.transform.position + gameObject.transform.up * 0.1f, gameObject.transform.rotation);
			obj.transform.Rotate(Vector3.right * spreadAmount);
			obj.transform.SetParent(base.transform, worldPositionStays: true);
			gameObject.transform.Rotate(Vector3.forward * (360 / projectileAmount));
		}
		Object.Destroy(gameObject);
		Invoke("Remove", 5f);
	}

	private void Remove()
	{
		Object.Destroy(base.gameObject);
	}
}
