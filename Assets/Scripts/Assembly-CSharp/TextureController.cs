using UnityEngine;

public class TextureController : MonoBehaviour
{
	private int vertexWarping = 2;

	private static readonly int VertexWarping = Shader.PropertyToID("_VertexWarping");

	private static readonly int TextureWarping = Shader.PropertyToID("_TextureWarping");

	private void Awake()
	{
		CheckTextureWarping();
		CheckVertexWarping();
	}

	public void CheckTextureWarping()
	{
		float value = PlayerPrefs.GetFloat("TexWar", 100f) * 0.003f;
		Material[] array = Resources.FindObjectsOfTypeAll<Material>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetFloat(TextureWarping, value);
		}
	}

	public void CheckVertexWarping()
	{
		vertexWarping = PlayerPrefs.GetInt("VerWar", 2);
		float value = 0f;
		switch (vertexWarping)
		{
		case 0:
			value = 9999f;
			break;
		case 1:
			value = 400f;
			break;
		case 2:
			value = 160f;
			break;
		case 3:
			value = 80f;
			break;
		case 4:
			value = 40f;
			break;
		case 5:
			value = 16f;
			break;
		}
		Material[] array = Resources.FindObjectsOfTypeAll<Material>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetFloat(VertexWarping, value);
		}
	}
}
