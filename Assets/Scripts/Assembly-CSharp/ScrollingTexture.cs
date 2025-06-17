using UnityEngine;

public class ScrollingTexture : MonoBehaviour
{
	public float scrollSpeedX;

	public float scrollSpeedY;

	private MeshRenderer mr;

	private void Start()
	{
		mr = GetComponent<MeshRenderer>();
	}

	private void Update()
	{
		Material[] materials = mr.materials;
		for (int i = 0; i < materials.Length; i++)
		{
			materials[i].mainTextureOffset += new Vector2(scrollSpeedX * Time.deltaTime, scrollSpeedY * Time.deltaTime);
		}
	}
}
