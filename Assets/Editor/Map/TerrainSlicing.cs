using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TreeEditor;
using UnityEditor.SceneManagement;

public class TerrainSlicing : Editor
{
    public const string TerrainSavePath = MapDefine.TERRAIN_ASSET_PATH;
    
    /*
    [MenuItem("Terrain/CollectMapItem")]
    private static void CollectMapItem()
    {
        EditorUtility.ClearProgressBar();

        MapAsset mapAsset = new MapAsset();
        mapAsset.MapList = new List<MapInfo>();

        CollectTrees(mapAsset.MapList);
      
        AssetDatabase.CreateAsset(mapAsset,"Assets/MapAsset.asset");
        AssetDatabase.Refresh();
    }


    private static void CollectTrees(List<MapInfo> mapList)
    {
        //Collect Tree
        Transform TreeRoot = GameObject.Find("[Trees]").transform;
        for (int i = 0; i < TreeRoot.childCount; i++)
        {
            EditorUtility.DisplayProgressBar("树木归档", i + "/" + TreeRoot.childCount, i / (float)TreeRoot.childCount);
            Transform tree = TreeRoot.GetChild(i);
            int posX = (int)(tree.localPosition.x / MapDefine.MAPITEMSIZE);
            int posZ = (int)(tree.localPosition.z / MapDefine.MAPITEMSIZE);

            MapInfo mapInfo;
            string tmpMapKey = posX + "_" + posZ;
            int indexMapInfo = mapList.FindIndex(a => a.mapKey.Equals(tmpMapKey));
            if (indexMapInfo < 0)
            {
                mapInfo = new MapInfo(){mapKey = tmpMapKey };
                mapList.Add(mapInfo);
            }
            else
            {
                mapInfo = mapList[indexMapInfo];
            }

            MapItemInfo mapItemInfo;
            int indexMapItemInfo = mapInfo.MapItemList.FindIndex(a => a.MapItemType == eMapItemType.Tree);
            if (indexMapItemInfo < 0)
            {
                mapItemInfo = new MapItemInfo(){MapItemType = eMapItemType.Tree};
                mapInfo.MapItemList.Add(mapItemInfo);
            }
            else
            {
                mapItemInfo = mapInfo.MapItemList[indexMapItemInfo];
            }

            MapItemInfoBase mapItemInfoBase = new MapItemInfoBase();
            mapItemInfoBase.Pos   = tree.localPosition - new Vector3(posX * MapDefine.MAPITEMSIZE,3, posZ * MapDefine.MAPITEMSIZE);
            mapItemInfoBase.Angle = Vector3.zero;
            mapItemInfoBase.Scale = Vector3.one;
            mapItemInfo.MapItemInfoList.Add(mapItemInfoBase);
        }
        EditorUtility.ClearProgressBar();
    }
    */
    
    
	//开始分割地形
	[MenuItem("Terrain/Slicing")]
	private static void Slicing()
	{
		EditorUtility.ClearProgressBar();
	    MapAsset mapAsset = new MapAsset();

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


	    if (Directory.Exists(MapDefine.TERRAIN_PREFAB_PATH))
	        Directory.Delete(MapDefine.TERRAIN_PREFAB_PATH, true);
	    Directory.CreateDirectory(MapDefine.TERRAIN_PREFAB_PATH);

        //Debug.LogFormat ("TerrainData.size.x{0}, TerrainData.size.y{1}, TerrainData.size.z{2}",terrainData.size.x,terrainData.size.y,terrainData.size.z);
        //得到新地图分辨率

        int heightmapResolution = (terrainData.heightmapResolution - 1) / SlicingBlockCnt;
		int alphamapResolution = terrainData.alphamapResolution / SlicingBlockCnt;
		int baseMapResolution = terrainData.baseMapResolution / SlicingBlockCnt;
		Vector3 size = new Vector3(terrainData.size.x / SlicingBlockCnt, terrainData.size.y,  terrainData.size.z / SlicingBlockCnt);

		SplatPrototype[] splatProtos = terrainData.splatPrototypes;

	    for (int row = 0; row < SlicingBlockCnt; ++row)
	    {
            for (int col = 0; col < SlicingBlockCnt; ++col)
			{
				TerrainData newData = new TerrainData();
				string terrainName = string.Format (MapDefine.MAPKEYNAME, row, col);
				AssetDatabase.CreateAsset(newData, TerrainSavePath + terrainName + MapDefine.EXTENSION);

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

					float offsetX = (newData.size.x * col) % splatProtos[i].tileSize.x + splatProtos[i].tileOffset.x;
					float offsetY = (newData.size.z * row) % splatProtos[i].tileSize.y + splatProtos[i].tileOffset.y;
					newSplats[i].tileOffset = new Vector2(offsetX, offsetY);
				}
				newData.splatPrototypes = newSplats;

				float[,,] alphamap = new float[alphamapResolution, alphamapResolution, splatProtos.Length];
				alphamap = terrainData.GetAlphamaps(col * newData.alphamapWidth, row * newData.alphamapHeight, newData.alphamapWidth, newData.alphamapHeight);
				newData.SetAlphamaps(0, 0, alphamap);

				int xBase = terrainData.heightmapWidth / SlicingBlockCnt;
				int yBase = terrainData.heightmapHeight / SlicingBlockCnt;

				float[,] height = terrainData.GetHeights(xBase * col, yBase * row, xBase + 1, yBase + 1);
				newData.SetHeights(0, 0, height);

				GameObject terrainBase = new GameObject (terrainName,new System.Type[]{typeof(Terrain),typeof(TerrainCollider)});
				terrainBase.transform.SetParent (mapRoot);
			    
                GameObject treeRoot = new GameObject("[Tree]");
                treeRoot.transform.SetParent(terrainBase.transform);
			    treeRoot.transform.localPosition = Vector3.zero;
			    treeRoot.transform.localEulerAngles = Vector3.zero;
			    treeRoot.transform.localScale = Vector3.one;

                Terrain terrainBlock = terrainBase.GetComponent<Terrain> ();
				terrainBlock.terrainData = newData;
				terrainBlock.materialType = terrain.materialType;
				terrainBlock.materialTemplate = terrain.materialTemplate;

				TerrainCollider terrainCollider = terrainBase.GetComponent<TerrainCollider> ();
				terrainCollider.terrainData = newData;
				terrainBase.transform.localPosition = new Vector3 (col * MapDefine.MAPITEMSIZE, 0, row * MapDefine.MAPITEMSIZE);

                PrefabUtility.CreatePrefab(MapDefine.TERRAIN_PREFAB_PATH + terrainName + ".prefab", terrainBase);
			}
		}
        
        //Collect Tree
		Transform TreeRoot = GameObject.Find ("[Trees]").transform;
		for (int i = 0; i < TreeRoot.childCount; i++) {
			Transform tree = TreeRoot.GetChild (i);
			int col = (int)tree.localPosition.x / MapDefine.MAPITEMSIZE;
			int row = (int)tree.localPosition.z / MapDefine.MAPITEMSIZE;
			string terrainName = string.Format (MapDefine.MAPKEYNAME, row, col);

		    MapInfo mapInfo;
		    int indexMapInfo = mapAsset.MapList.FindIndex(a => a.mapKey.Equals(terrainName));
		    if (indexMapInfo < 0)
		    {
		        mapInfo = new MapInfo() { mapKey = terrainName };
		        mapAsset.MapList.Add(mapInfo);
		    }
		    else
		    {
		        mapInfo = mapAsset.MapList[indexMapInfo];
		    }

		    MapItemInfo mapItemInfo;
		    int indexMapItemInfo = mapInfo.MapItemList.FindIndex(a => a.MapItemType == eMapItemType.Tree);
		    if (indexMapItemInfo < 0)
		    {
		        mapItemInfo = new MapItemInfo() { MapItemType = eMapItemType.Tree };
		        mapInfo.MapItemList.Add(mapItemInfo);
		    }
		    else
		    {
		        mapItemInfo = mapInfo.MapItemList[indexMapItemInfo];
		    }

		    MapItemInfoBase mapItemInfoBase = new MapItemInfoBase();
            
            Debug.Log("terrainName:" + terrainName);
		    mapItemInfoBase.Pos = tree.localPosition - new Vector3(col * MapDefine.MAPITEMSIZE, 0, row * MapDefine.MAPITEMSIZE);
            mapItemInfoBase.Angle = Vector3.zero;
		    mapItemInfoBase.Scale = tree.localScale;
		    mapItemInfo.MapItemInfoList.Add(mapItemInfoBase);
        }

	    AssetDatabase.CreateAsset(mapAsset, MapDefine.MAPITEMINFOASSET);

		GameObject.DestroyImmediate (mapRoot.gameObject);
		EditorUtility.ClearProgressBar();
	}
}