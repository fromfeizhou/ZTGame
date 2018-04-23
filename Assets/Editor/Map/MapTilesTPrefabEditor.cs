using System;
using System.Collections.Generic;
using System.IO;
using Boo.Lang;
using NPOI.SS.Formula.Functions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MapTilesData
{
    public string tilesKey;
    public Vector3 goPos;
    public int goRow;
    public int goCol;
    public GameObject tilesGo;
}


public class MapTilesTPrefabEditor
{
    private static int tilesIndex = 0;
    private const string TilesMapKey = "Map";
    private const string TilesTextureKey = "TilesTexture_";

    private const string MapTilesPrefabSavePath = "Assets/Map/Prefabs/MapItem/{0}";//{1}.prefab";

    private const int FileGridInterval = 256;
    private const int TilesGridInterval = 16;
    public static Dictionary<string, GameObject> mapTilesGos =
        new Dictionary<string, GameObject>();

    public static Dictionary<string, MapTilesData> mapTilesDataDic =
        new Dictionary<string, MapTilesData>();

    public static Dictionary<string, System.Collections.Generic.List<GameObject>> mapTextureTilesGos = new Dictionary<string, System.Collections.Generic.List<GameObject>>();

    #region  把每块tiles加上tilesTexture组合成一个prefab 创建时间成本大（巨大）

    [MenuItem("ZTTool/MapTool/地图面片转prefab")]
    public static void MapTilesTPrefab()
    {
        mapTextureTilesGos.Clear();
        mapTilesDataDic.Clear();
        //地图瓦片
        GameObject mapTilesRoot = GameObject.Find("MapTilesRoot/MapTiles");
        if (mapTilesRoot == null) return;
        Renderer[] mapTilesTransform = mapTilesRoot.GetComponentsInChildren<Renderer>();
        Transform go = null;
        int row = 0;
        int col = 0;
        for (int index = 0; index < mapTilesTransform.Length; index++)
        {
            go = mapTilesTransform[index].transform;
            row = Mathf.FloorToInt(go.position.x / TilesGridInterval);
            col = Mathf.FloorToInt(go.position.z / TilesGridInterval);
            string key = row + "_" + col;
            if (!mapTilesDataDic.ContainsKey(key))
                mapTilesDataDic[key] = new MapTilesData()
                {
                    tilesKey = key,
                    goCol = col,
                    goRow = row,
                    tilesGo = go.gameObject,
                    goPos = go.position,
                };
            else
            {
                Debug.LogError("地图瓦片重复Key:" + key);
            }
        }

        //地图纹理瓦片（草地、沙地...）
        GameObject mapTextureTilesRoot = GameObject.Find("MapTilesRoot/MapTextureTiles");
        if (mapTextureTilesRoot == null) return;
        mapTilesTransform = mapTextureTilesRoot.GetComponentsInChildren<Renderer>();
        for (int index = 0; index < mapTilesTransform.Length; index++)
        {
            go = mapTilesTransform[index].transform;
            row = Mathf.FloorToInt(go.position.x / TilesGridInterval);
            col = Mathf.FloorToInt(go.position.z / TilesGridInterval);
            string key = row + "_" + col;
            if (!mapTextureTilesGos.ContainsKey(key))
                mapTextureTilesGos[key] = new System.Collections.Generic.List<GameObject>();
            GameObject tempGo = go.gameObject;// as GameObject;
            mapTextureTilesGos[key].Add(tempGo);
        }
        //生成预设
        CreateTilesPrefab();
        EditorUtility.ClearProgressBar();


        //销毁备份GO
        DestroyTempGo();
        AssetDatabase.Refresh();
        EditorSceneManager.OpenScene("Assets/Scences/SceneEditor.unity");
    }

   
    private static void CreateTilesPrefab()
    {
        foreach (KeyValuePair<string, MapTilesData> item in mapTilesDataDic)
        {
            MapTilesData tileData = item.Value;
            System.Collections.Generic.List<GameObject> textureGos = null;
            if (mapTextureTilesGos.TryGetValue(item.Key, out textureGos))
            {
                for (int index = 0; index < textureGos.Count; index++)
                {
                    textureGos[index].transform.SetParent(tileData.tilesGo.transform);
                }
            }

            int row = Mathf.FloorToInt(tileData.goPos.x / FileGridInterval);
            int col = Mathf.FloorToInt(tileData.goPos.z / FileGridInterval);
            tileData.tilesGo.transform.SetParent(null);
            string fileName = row + "_" + col;
            string filePath = string.Format(MapTilesPrefabSavePath, fileName);
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            //  EditorUtility.DisplayProgressBar("创建prefab", fileName + tileData.tilesKey, 0);
            PrefabUtility.CreatePrefab(filePath + "/" + tileData.tilesKey + ".prefab",
                    tileData.tilesGo);
        }
    }

    private static void DestroyTempGo()
    {
        foreach (KeyValuePair<string, System.Collections.Generic.List<GameObject>> item in mapTextureTilesGos)
        {
            for (int index = 0; index < item.Value.Count; index++)
            {
                GameObject.DestroyImmediate(item.Value[index]);
            }
        }
        mapTextureTilesGos.Clear();
    }

    #endregion



    #region 生成tiles预设并记录各个tiles三维数据（按类型创建）
    //瓦片类型：TilesMap_XXX
    //瓦片纹理类型：TilesTexture_XXX
    [MenuItem("ZTTool/MapTool/生成瓦片数据")]
    public static void CreateTilesPrefabAndData()
    {
        string tilesFilePath = "Assets" + MapDefine.TilesMapFilePath;
       // string directoryPath = tilesFilePath.Substring(0, tilesFilePath.LastIndexOf("/"));
        if (Directory.Exists(tilesFilePath))
            Directory.Delete(tilesFilePath,true);
        Directory.CreateDirectory(tilesFilePath);
        string[] bigMapIndexs = ("0_0").Split('_');
        int bigMapX = int.Parse(bigMapIndexs[0]);
        int bigMapY = int.Parse(bigMapIndexs[1]);
        //基于大地图偏移
        int offsetX = bigMapX * MapDefine.MAPITEMTOTALSIZE;
        int offsetY = bigMapY * MapDefine.MAPITEMTOTALSIZE;
        string bigMapKey = bigMapX + "" + bigMapY;
        MapAsset mapAsset = new MapAsset();
        tilesIndex = 0;
        EditorUtility.DisplayProgressBar("遍历瓦片数据", "地图瓦片遍历ing...", 0);
        CreateTilesMap(bigMapKey, offsetX, offsetY, mapAsset);
       // EditorUtility.DisplayProgressBar("遍历瓦片数据", "地图纹理瓦片遍历ing...", 0);
       // GameObject root = GameObject.Find("MapTilesRoot/MapTextureTiles");
       // CreateTileTexture(root.transform,bigMapKey, offsetX, offsetY, mapAsset);
        CreateTileDataFile(mapAsset, bigMapKey);
        EditorUtility.ClearProgressBar();
    }
    //地形瓦片
    private static void CreateTilesMap(string bigMapKey,int offsetX,int offsetY,MapAsset mapAsset)
    {
        GameObject root = GameObject.Find("MapTilesRoot/MapTiles");
        if (root == null) return;
       

        //美术还没弄完先，目前只有一种类型，先特殊处理
        Renderer[] gos = root.transform.GetComponentsInChildren<Renderer>();


        for (int index = 0; index < gos.Length; index++)
        {
            Transform element = gos[index].transform;
            element.gameObject.name = gos[index].sharedMaterial.name;
            if (true)//element.gameObject.name.Contains(TilesMapKey))
            {
                string prefabPath = MapDefine.TilesMapFilePath + element.gameObject.name + ".prefab";
                if (!System.IO.File.Exists(Application.dataPath + prefabPath))
                {
                    PrefabUtility.CreatePrefab("Assets" + prefabPath, element.gameObject);
                }
                int col = (int)(element.position.x + offsetX) / MapDefine.MapElementSize;
                int row = (int)(element.position.z + offsetY) / MapDefine.MapElementSize;
                string elementGridKey = col + "" + row;
                string elementKey = tilesIndex + "";
                tilesIndex++;
                string gridKey = col + "_" + row;
                MapElementInfo elementInfo = new MapElementInfo()
                {
                    Pos = element.position,
                    Angle = element.eulerAngles,
                    Scale = element.localScale
                };
                MapElement mapElement = new MapElement();
                mapElement.elementKey = elementKey;
                mapElement.elementType = element.name;
                mapElement.elementInfo = elementInfo;
                mapAsset.AddMapElement(mapElement);
                mapAsset.AddMapElementGridItem(gridKey, elementKey);
            }
        }
    }
    //纹理瓦片
    private static void CreateTileTexture(Transform go,string bigMapKey, int offsetX, int offsetY, MapAsset mapAsset)
    {
        if (go == null) return;
        for (int index = 0; index < go.childCount; index++)
        {
            Transform element = go.GetChild(index);
            if (element.name.Contains(TilesTextureKey))
            {
                string prefabPath = MapDefine.TilesMapFilePath + element.name + ".prefab";
                if (!System.IO.File.Exists(Application.dataPath + prefabPath))
                {
                    PrefabUtility.CreatePrefab("Assets" + prefabPath, element.gameObject);
                }
                int tempTilesindex = tilesIndex++;

                Vector3 postion = element.position;
                Vector3 scale = element.localScale;
                element.position = Vector3.zero;
                element.localScale = Vector3.one;
                Vector3 center = Vector3.zero;
                Renderer[] renders = element.GetComponentsInChildren<Renderer>();
                foreach (Renderer child in renders)
                    center += child.bounds.center;
                center /= renders.Length;
                Bounds bounds = new Bounds(center, Vector3.zero);
                foreach (Renderer child in renders)
                    bounds.Encapsulate(child.bounds);
                Vector3 centralPoint = bounds.center;
                element.position = postion;
                element.localScale = scale;

                centralPoint += element.position;
                int starX = (int)(centralPoint.x - bounds.size.x * 0.5f + offsetX) / MapDefine.TilesGridInterval;
                int endX = (int)(centralPoint.x + bounds.size.x * 0.5f + offsetX) / MapDefine.TilesGridInterval;
                int starZ = (int)(centralPoint.z - bounds.size.z * 0.5f + offsetY) / MapDefine.TilesGridInterval;
                int endZ = (int)(centralPoint.z + bounds.size.z * 0.5f + offsetY) / MapDefine.TilesGridInterval;
                for (int k = starX; k <= endX; k++)
                {
                    for (int j = starZ; j <= endZ; j++)
                    {
                        string gridKey = k + "_" + j;
                        MapElementInfo elementInfo = new MapElementInfo()
                        {
                            Pos = element.position,
                            Angle = element.eulerAngles,
                            Scale = element.localScale
                        };
                        MapElement mapElement = new MapElement();
                        mapElement.elementKey = tempTilesindex + "";
                        mapElement.elementType = element.name;
                        mapElement.elementInfo = elementInfo;
                        mapAsset.AddMapElement(mapElement);
                        mapAsset.AddMapElementGridItem(gridKey, mapElement.elementKey);
                    }
                }
            }
            else
            {
                CreateTileTexture(element, bigMapKey, offsetX, offsetY, mapAsset);
            }
        }
    }
    //生成数据文件
    private static void CreateTileDataFile(MapAsset mapAsset,string bigMapKey)
    {
        string tempPth = string.Format(MapDefine.MapTilesAssetFolderPath, bigMapKey);
        if (!Directory.Exists(tempPth))
            Directory.CreateDirectory(tempPth);
        tempPth += string.Format(MapDefine.MapTilesAssetFileName, bigMapKey);
        AssetDatabase.CreateAsset(mapAsset, tempPth);
        AssetDatabase.Refresh();
    }

    #endregion


}
