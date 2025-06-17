using UnityEngine;
using UnityEngine.UI;

public class GraphicsOptions : MonoBehaviour
{
	public Dropdown pixelization;

	public Slider textureWarping;

	public Dropdown vertexWarping;

	public Dropdown colorCompression;

	public Toggle vSync;

	private CameraController cc;

	private TextureController texcon;

	public RenderTexture[] virtualMaterials;

	public RenderTexture[] virtualMaterials2;

	public RenderTexture[] widescreenVirtualMaterials;

	public RenderTexture[] widescreenVirtualMaterials2;

	public Material[] targetMats;

	private void Start()
	{
		pixelization.value = PlayerPrefs.GetInt("Pix", 3);
		pixelization.RefreshShownValue();
		textureWarping.value = PlayerPrefs.GetFloat("TexWar", 100f);
		vertexWarping.value = PlayerPrefs.GetInt("VerWar", 2);
		vertexWarping.RefreshShownValue();
		colorCompression.value = PlayerPrefs.GetInt("ColCom", 2);
		colorCompression.RefreshShownValue();
		if (PlayerPrefs.GetInt("VSync", 1) == 1)
		{
			QualitySettings.vSyncCount = 1;
			vSync.isOn = true;
		}
		else
		{
			QualitySettings.vSyncCount = 0;
			vSync.isOn = false;
		}
	}

	public void PCPreset()
	{
		pixelization.value = 0;
		pixelization.RefreshShownValue();
		textureWarping.value = 0f;
		vertexWarping.value = 0;
		vertexWarping.RefreshShownValue();
		colorCompression.value = 2;
		colorCompression.RefreshShownValue();
	}

	public void PSXPreset()
	{
		pixelization.value = 3;
		pixelization.RefreshShownValue();
		textureWarping.value = 100f;
		vertexWarping.value = 2;
		vertexWarping.RefreshShownValue();
		colorCompression.value = 2;
		colorCompression.RefreshShownValue();
	}

	public void Pixelization(int stuff)
	{
		PlayerPrefs.SetInt("Pix", stuff);
		if (cc == null)
		{
			cc = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
		}
		targetMats[0].mainTexture = virtualMaterials[stuff];
		targetMats[1].mainTexture = virtualMaterials2[stuff];
		targetMats[2].mainTexture = widescreenVirtualMaterials[stuff];
		targetMats[3].mainTexture = widescreenVirtualMaterials2[stuff];
		cc.mainTargetMaterial = virtualMaterials[stuff];
		cc.hudTargetMaterial = virtualMaterials2[stuff];
		cc.ultraWideTargetMaterial = widescreenVirtualMaterials[stuff];
		cc.ultraWideHudTargetMaterial = widescreenVirtualMaterials2[stuff];
		cc.CheckPixelization();
		DownscaleChangeSprite[] array = Object.FindObjectsOfType<DownscaleChangeSprite>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].CheckScale();
		}
	}

	public void TextureWarping(float stuff)
	{
		PlayerPrefs.SetFloat("TexWar", stuff);
		if (texcon == null)
		{
			texcon = Object.FindObjectOfType<TextureController>();
		}
		texcon.CheckTextureWarping();
	}

	public void VertexWarping(int stuff)
	{
		PlayerPrefs.SetInt("VerWar", stuff);
		if (texcon == null)
		{
			texcon = Object.FindObjectOfType<TextureController>();
		}
		texcon.CheckVertexWarping();
	}

	public void ColorCompression(int stuff)
	{
		PlayerPrefs.SetInt("ColCom", stuff);
		if (cc == null)
		{
			cc = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
		}
		cc.CheckColorCompression();
	}

	public void VSync(bool stuff)
	{
		if (stuff)
		{
			QualitySettings.vSyncCount = 1;
			PlayerPrefs.SetInt("VSync", 1);
		}
		else
		{
			QualitySettings.vSyncCount = 0;
			PlayerPrefs.SetInt("VSync", 0);
		}
	}
}
