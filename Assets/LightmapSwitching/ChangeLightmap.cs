using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.IO;
using System.Collections.Generic;

public class ChangeLightmap : MonoBehaviour
{
	[System.Serializable]
	private class SphericalHarmonics
	{
		public float[] coefficients = new float[27];
	}

	[System.Serializable]
	private class RendererInfo
	{
		public Renderer renderer;
		public int lightmapIndex;
		public Vector4 lightmapOffsetScale;
	}

	[System.Serializable]
	private class LightingScenarioData
	{
		public string name;
		public RendererInfo[] rendererInfos;
		public Texture2D[] lightmaps;
		public Texture2D[] lightmapsDir;
		public Texture2D[] lightmapsShadow;
		public LightmapsMode lightmapsMode;
		public SphericalHarmonics[] lightProbes;
	}

	[SerializeField] private List<LightingScenarioData> lightingScenariosData = new List<LightingScenarioData>();
	private LightingScenarioData currentLightingScenarioData;
	[SerializeField] private bool loadOnAwake = false; // Load the selected lighmap when this script wakes up (aka when game starts).
	[SerializeField] private bool verbose = false;  //TODO : enable logs only when verbose enabled
	private string jsonFileName = "mapconfig.txt"; // Name of the json data file.
	[SerializeField] private string m_currentLightScenario = "LightMapData_1";
	private string absoluteName;


	public static ChangeLightmap instance;
	public string resourceFolder { get { return m_currentLightScenario; } }
	public string GetResourcesDirectory(string dir) { return Application.dataPath + "/Resources/" + dir + "/"; }// The directory where the lightmap data resides.	
	public bool CheckResourcesDirectoryExists(string dir) { return Directory.Exists(GetResourcesDirectory(dir)); }


	private void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}
		else
			instance = this;

		if (loadOnAwake)
			Load();
	}

	public void LoadFrom(string folderName)
	{
		m_currentLightScenario = folderName;
		Load();
	}

	// Loads from m_resourceFolder
	public void Load()
	{
		currentLightingScenarioData = GetLightScenarioData(m_currentLightScenario);

		var newLightmaps = new LightmapData[currentLightingScenarioData.lightmaps.Length];

		for (int i = 0; i < newLightmaps.Length; i++)
		{
			newLightmaps[i] = new LightmapData();
			newLightmaps[i].lightmapColor = Resources.Load<Texture2D>(m_currentLightScenario + "/" + currentLightingScenarioData.lightmaps[i].name);

			if (currentLightingScenarioData.lightmapsMode != LightmapsMode.NonDirectional)
			{
				newLightmaps[i].lightmapDir = Resources.Load<Texture2D>(m_currentLightScenario + "/" + currentLightingScenarioData.lightmapsDir[i].name);
				if (currentLightingScenarioData.lightmapsShadow != null && currentLightingScenarioData.lightmapsShadow.Length > i && currentLightingScenarioData.lightmapsShadow[i] != null)
				{ // If the textuer existed and was set in the data file.
					newLightmaps[i].shadowMask = Resources.Load<Texture2D>(m_currentLightScenario + "/" + currentLightingScenarioData.lightmapsShadow[i].name);
				}
			}
		}

		LoadLightProbes( currentLightingScenarioData );
		ApplyRendererInfo(currentLightingScenarioData.rendererInfos);

		LightmapSettings.lightmaps = newLightmaps;
		UnityEditor.EditorUtility.SetDirty(this.gameObject);
	}

	public void Clear()
	{
        foreach ( LightingScenarioData ld in lightingScenariosData )
		{
			ld.lightmaps = null;
			ld.lightmapsDir = null;
			ld.lightmapsShadow = null;
		}
		lightingScenariosData.Clear();
	}

	public void SaveLightMaps()
	{
		LightingScenarioData newLightingScenario = new LightingScenarioData();
		var newRendererInfos = new List<RendererInfo>();
		var newLightmapsTextures = new List<Texture2D>();
		var newLightmapsTexturesDir = new List<Texture2D>();
		var newLightmapsTexturesShadow = new List<Texture2D>();
		var newLightmapsMode = new LightmapsMode();
		var newSphericalHarmonicsList = new List<SphericalHarmonics>();

		newLightmapsMode = LightmapSettings.lightmapsMode;

		var renderers = FindObjectsOfType(typeof(MeshRenderer));
		Debug.Log("stored info for " + renderers.Length + " meshrenderers");
		foreach (MeshRenderer renderer in renderers)
		{

			if (renderer.lightmapIndex != -1)
			{
				RendererInfo info = new RendererInfo();
				info.renderer = renderer;
				info.lightmapOffsetScale = renderer.lightmapScaleOffset;

				Texture2D lightmaplight = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapColor;
				info.lightmapIndex = newLightmapsTextures.IndexOf(lightmaplight);
				if (info.lightmapIndex == -1)
				{
					info.lightmapIndex = newLightmapsTextures.Count;
					newLightmapsTextures.Add(lightmaplight);
				}

				if (newLightmapsMode != LightmapsMode.NonDirectional)
				{
					//first directional lighting
					Texture2D lightmapdir = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapDir;
					info.lightmapIndex = newLightmapsTexturesDir.IndexOf(lightmapdir);
					if (info.lightmapIndex == -1)
					{
						info.lightmapIndex = newLightmapsTexturesDir.Count;
						newLightmapsTexturesDir.Add(lightmapdir);
					}
					LightmapData lmd = LightmapSettings.lightmaps[0];
					//now the shadowmask
					Texture2D lightmapshadow = LightmapSettings.lightmaps[renderer.lightmapIndex].shadowMask;
					if (lightmapshadow != null)
					{
						info.lightmapIndex = newLightmapsTexturesShadow.IndexOf(lightmapshadow);
						if (info.lightmapIndex == -1)
						{
							info.lightmapIndex = newLightmapsTexturesShadow.Count;
							newLightmapsTexturesShadow.Add(lightmapshadow);
						}
					}

				}
				newRendererInfos.Add(info);
			}
		}


		newLightingScenario.lightmapsMode = newLightmapsMode;
		newLightingScenario.lightmaps = newLightmapsTextures.ToArray();

		if (newLightmapsMode != LightmapsMode.NonDirectional)
		{
			newLightingScenario.lightmapsDir = newLightmapsTexturesDir.ToArray();
			newLightingScenario.lightmapsShadow = newLightmapsTexturesShadow.ToArray();
		}

		newLightingScenario.rendererInfos = newRendererInfos.ToArray();

		var scene_LightProbes = new SphericalHarmonicsL2[LightmapSettings.lightProbes.bakedProbes.Length];
		scene_LightProbes = LightmapSettings.lightProbes.bakedProbes;

		for (int i = 0; i < scene_LightProbes.Length; i++)
		{
			var SHCoeff = new SphericalHarmonics();

			// j is coefficient
			for (int j = 0; j < 3; j++)
			{
				//k is channel ( r g b )
				for (int k = 0; k < 9; k++)
				{
					SHCoeff.coefficients[j * 9 + k] = scene_LightProbes[i][j, k];
				}
			}

			newSphericalHarmonicsList.Add(SHCoeff);
		}

		newLightingScenario.lightProbes = newSphericalHarmonicsList.ToArray();

		// write the files and map config data.
		CreateResourcesDirectory(m_currentLightScenario);

		string resourcesDir = GetResourcesDirectory(m_currentLightScenario);
		CopyTextureToResources(resourcesDir, newLightingScenario.lightmaps);
		CopyTextureToResources(resourcesDir, newLightingScenario.lightmapsDir);
		CopyTextureToResources(resourcesDir, newLightingScenario.lightmapsShadow);

		newLightingScenario.name = m_currentLightScenario;
		SaveLightScenarioData ( newLightingScenario );

		UnityEditor.EditorUtility.SetDirty(this.gameObject);
	}


	#region Private 
	private void CreateResourcesDirectory(string dir)
	{
		if (!CheckResourcesDirectoryExists(m_currentLightScenario))
			Directory.CreateDirectory(GetResourcesDirectory(dir));
	}
	private void LoadLightProbes( LightingScenarioData lsd )
	{
		var sphericalHarmonicsArray = new SphericalHarmonicsL2[lsd.lightProbes.Length];

		for (int i = 0; i < lsd.lightProbes.Length; i++)
		{
			var sphericalHarmonics = new SphericalHarmonicsL2();

			// j is coefficient
			for (int j = 0; j < 3; j++)
			{
				//k is channel ( r g b )
				for (int k = 0; k < 9; k++)
				{
					sphericalHarmonics[j, k] = lsd.lightProbes[i].coefficients[j * 9 + k];
				}
			}

			sphericalHarmonicsArray[i] = sphericalHarmonics;
		}

		try
		{
			LightmapSettings.lightProbes.bakedProbes = sphericalHarmonicsArray;
		} catch { Debug.LogWarning("Warning, error when trying to load lightprobes for scenario "); }
	}
	private void ApplyRendererInfo(RendererInfo[] infos)
	{
		try
		{
			for (int i = 0; i < infos.Length; i++)
			{
				RendererInfo info = infos[i];
				info.renderer.lightmapIndex = infos[i].lightmapIndex;
				if (!info.renderer.isPartOfStaticBatch)
				{
					info.renderer.lightmapScaleOffset = infos[i].lightmapOffsetScale;
				}
				if (info.renderer.isPartOfStaticBatch && verbose == true)
				{
					Debug.Log("Object " + info.renderer.gameObject.name + " is part of static batch, skipping lightmap offset and scale.");
				}
			}
		} catch (Exception e)
		{
			Debug.LogError("Error in ApplyRendererInfo:" + e.GetType().ToString());
		}
	}

	private void WriteJsonFile(string path, string json)
	{
		absoluteName = path + jsonFileName;
		File.WriteAllText(absoluteName, json); // Write all the data to the file.
	}

	private string GetJsonFile(string f)
	{
		if (!File.Exists(f))
		{
			return "";
		}
		return File.ReadAllText(f); // Write all the data to the file.
	}

	private LightingScenarioData GetLightScenarioData( string scenarioName )
	{
		if (lightingScenariosData == null)
            return null;

		foreach( LightingScenarioData ld in lightingScenariosData )
        {
			if (ld.name == scenarioName)
				return ld;
        }
		return null;
	}
	private int GetLightScenarioDataIndex( string scenarioName )
	{
		if (lightingScenariosData == null)
            return -1;

		for( int i = 0; i < lightingScenariosData.Count; i++  )
        {
			if (lightingScenariosData[i].name == scenarioName)
				return i;
        }
		return -1;
	}
	private void SaveLightScenarioData( LightingScenarioData newScenario )
	{
		if ( newScenario == null)
			Debug.LogException( new Exception("Incorrect use of the method") );

		int lightScenarioIndex = GetLightScenarioDataIndex( newScenario.name );
		if ( lightScenarioIndex == -1 )
			lightingScenariosData.Add ( newScenario);
		else 
			lightingScenariosData[lightScenarioIndex] = newScenario;
	}


	private void CopyTextureToResources(string toPath, Texture2D[] textures)
	{
		for (int i = 0; i < textures.Length; i++)
		{
			Texture2D texture = textures[i];
			if (texture != null) // Maybe the optional shadowmask didn't exist?
			{
				FileUtil.ReplaceFile(
					AssetDatabase.GetAssetPath(texture),
					toPath + Path.GetFileName(AssetDatabase.GetAssetPath(texture))
				);
				AssetDatabase.Refresh(); // Refresh so the newTexture file can be found and loaded.
				Texture2D newTexture = Resources.Load<Texture2D>(m_currentLightScenario + "/" + texture.name); // Load the new texture as an object.

				CopyTextureImporterProperties(textures[i], newTexture); // Ensure new texture takes on same properties as origional texture.

				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newTexture)); // Re-import texture file so it will be successfully compressed to desired format.
				//EditorUtility.CompressTexture(newTexture, textures[i].format, UnityEditor.TextureCompressionQuality.Best); // Now compress the texture.

				textures[i] = newTexture; // Set the new texture as the reference in the Json file.
			}
		}
	}

	private void CopyTextureImporterProperties(Texture2D fromTexture, Texture2D toTexture)
	{
		TextureImporter fromTextureImporter = GetTextureImporter(fromTexture);
		TextureImporter toTextureImporter = GetTextureImporter(toTexture);

		toTextureImporter.wrapMode = fromTextureImporter.wrapMode;
		toTextureImporter.mipmapEnabled = fromTextureImporter.mipmapEnabled;
		toTextureImporter.anisoLevel = fromTextureImporter.anisoLevel;
		toTextureImporter.sRGBTexture = fromTextureImporter.sRGBTexture;
		toTextureImporter.textureType = fromTextureImporter.textureType;
		toTextureImporter.textureCompression = fromTextureImporter.textureCompression;
	}

	private TextureImporter GetTextureImporter(Texture2D texture)
	{
		string newTexturePath = AssetDatabase.GetAssetPath(texture);
		TextureImporter importer = AssetImporter.GetAtPath(newTexturePath) as TextureImporter;
		return importer;
	}

	#endregion

}
