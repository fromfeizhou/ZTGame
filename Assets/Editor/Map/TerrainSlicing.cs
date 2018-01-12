using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TerrainSlicing : Editor
{
    public const string TerrainSavePath = MapDefine.TERRAIN_ASSET_PATH;
	//分割数量

	//开始分割地形
	[MenuItem("Terrain/Slicing")]
	private static void Slicing()
	{
		EditorUtility.ClearProgressBar();

		Dictionary<string,Transform> mapDic = new Dictionary<string, Transform>();

		Terrain terrain = GameObject.FindGameObjectWithTag ("MainTerrain").GetComponent<Terrain>();

		if (terrain.terrainData.size.x % MapDefine.MAPITEMSIZE != 0) {
			Debug.LogErrorFormat("[TerrainSlicing]terrainSize:{0}, MapItemSize:{1}",terrain.terrainData.size,MapDefine.MAPITEMSIZE);
			return;
		}

		Transform mapRoot = (new GameObject ("MapRoot")).transform;
		mapRoot.position = Vector3.zero;
		mapRoot.eulerAngles = Vector3.zero;
		mapRoot.localScale = Vector3.one;

		if (terrain == null)
		{
			Debug.LogError("找不到地形!");
			return;
		}

		if (Directory.Exists(TerrainSavePath))
			Directory.Delete(TerrainSavePath, true);
		Directory.CreateDirectory(TerrainSavePath);

		TerrainData terrainData = terrain.terrainData;

		int SlicingBlockCnt = (int)terrainData.size.x / MapDefine.MAPITEMSIZE;

		Debug.Log ("SLICING_SIZE:" + SlicingBlockCnt);
		//Debug.LogFormat ("TerrainData.size.x{0}, TerrainData.size.y{1}, TerrainData.size.z{2}",terrainData.size.x,terrainData.size.y,terrainData.size.z);
		//得到新地图分辨率

		int heightmapResolution = (terrainData.heightmapResolution - 1) / SlicingBlockCnt;
		int alphamapResolution = terrainData.alphamapResolution / SlicingBlockCnt;
		int baseMapResolution = terrainData.baseMapResolution / SlicingBlockCnt;
		Vector3 size = new Vector3(terrainData.size.x / SlicingBlockCnt, terrainData.size.y,  terrainData.size.z / SlicingBlockCnt);

		SplatPrototype[] splatProtos = terrainData.splatPrototypes;
		for (int x = 0; x < SlicingBlockCnt; ++x)
		{
			for (int y = 0; y < SlicingBlockCnt; ++y)
			{
				TerrainData newData = new TerrainData();
				string terrainName = string.Format (MapDefine.TERRAIN_NAME, x, y);
				AssetDatabase.CreateAsset(newData, TerrainSavePath + terrainName + MapDefine.EXTENSION);

				EditorUtility.DisplayProgressBar("正在分割地形", terrainName, (float)(x * SlicingBlockCnt + y) / (float)(SlicingBlockCnt * SlicingBlockCnt));

				newData.heightmapResolution = heightmapResolution;
				newData.alphamapResolution = alphamapResolution;
				newData.baseMapResolution = baseMapResolution;
				newData.size = size;

				SplatPrototype[] newSplats = new SplatPrototype[splatProtos.Length];
				for (int i = 0; i < splatProtos.Length;  ++i)
				{
					newSplats[i] = new SplatPrototype();
					newSplats[i].texture = splatProtos[i].texture;
					newSplats[i].tileSize = splatProtos[i].tileSize;

					float offsetX = (newData.size.x * x) % splatProtos[i].tileSize.x + splatProtos[i].tileOffset.x;
					float offsetY = (newData.size.z * y) % splatProtos[i].tileSize.y + splatProtos[i].tileOffset.y;
					newSplats[i].tileOffset = new Vector2(offsetX, offsetY);
				}
				newData.splatPrototypes = newSplats;

				float[,,] alphamap = new float[alphamapResolution, alphamapResolution, splatProtos.Length];
				alphamap = terrainData.GetAlphamaps(x * newData.alphamapWidth, y * newData.alphamapHeight, newData.alphamapWidth, newData.alphamapHeight);
				newData.SetAlphamaps(0, 0, alphamap);

				int xBase = terrainData.heightmapWidth / SlicingBlockCnt;
				int yBase = terrainData.heightmapHeight / SlicingBlockCnt;

				float[,] height = terrainData.GetHeights(xBase * x, yBase * y, xBase + 1, yBase + 1);
				newData.SetHeights(0, 0, height);

				GameObject terrainBase = new GameObject (terrainName,new System.Type[]{typeof(Terrain),typeof(TerrainCollider)});
				terrainBase.transform.SetParent (mapRoot);

				Terrain terrainBlock = terrainBase.GetComponent<Terrain> ();
				terrainBlock.terrainData = newData;
				terrainBlock.materialType = terrain.materialType;
				terrainBlock.materialTemplate = terrain.materialTemplate;

				TerrainCollider terrainCollider = terrainBase.GetComponent<TerrainCollider> ();
				terrainCollider.terrainData = newData;
				terrainBase.transform.localPosition = new Vector3 (x * MapDefine.MAPITEMSIZE, 0, y * MapDefine.MAPITEMSIZE);

				mapDic [terrainName] = terrainBase.transform; 
			}
		}

		/** Collect Tree */
		Transform TreeRoot = GameObject.Find ("[Trees]").transform;
		for (int i = 0; i < TreeRoot.childCount; i++) {
			EditorUtility.DisplayProgressBar("树木归档",i.ToString() + "/" + TreeRoot.childCount.ToString(),i/ (float)TreeRoot.childCount);
			Transform tree = TreeRoot.GetChild (i);
			int posX = (int)tree.localPosition.x / MapDefine.MAPITEMSIZE;
			int posZ = (int)tree.localPosition.z / MapDefine.MAPITEMSIZE;
			string terrainName = string.Format (MapDefine.TERRAIN_NAME, posX, posZ);
			Transform treeClone = Instantiate (tree);
			if (!mapDic.ContainsKey (terrainName)) {
				Debug.LogErrorFormat ("[TerrainSlicing]found out TerrainItem. terrainName:{0}, treePos:{1}", terrainName,tree.localPosition);
				return;
			}

			treeClone.SetParent (mapDic [terrainName]);
			treeClone.localPosition = tree.localPosition - new Vector3(posX * MapDefine.MAPITEMSIZE,0,posZ * MapDefine.MAPITEMSIZE);
			treeClone.localEulerAngles = tree.localEulerAngles;
			treeClone.localScale = tree.localScale;
			treeClone.name = tree + posX.ToString() + "_" + posZ.ToString();
		}


		/** CreateMapBlock Prefab */
        string prefabPath = MapDefine.TERRAIN_PREFAB_PATH;
		if (Directory.Exists(prefabPath))
			Directory.Delete(prefabPath, true);
		Directory.CreateDirectory(prefabPath);

		Dictionary<string,Transform>.Enumerator enumerator = mapDic.GetEnumerator();

		int mapItemCreateCnt = 0;
		while (enumerator.MoveNext ()) {
			enumerator.Current.Value.position = Vector3.zero;
			++mapItemCreateCnt;
			EditorUtility.DisplayProgressBar("地图预置物创建",mapItemCreateCnt.ToString() + "/" + mapDic.Count.ToString(),mapItemCreateCnt/ (float)mapDic.Count);
			PrefabUtility.CreatePrefab (prefabPath + enumerator.Current.Key + ".prefab",enumerator.Current.Value.gameObject);
		}
		GameObject.DestroyImmediate (mapRoot.gameObject);
		EditorUtility.ClearProgressBar();
	}
}