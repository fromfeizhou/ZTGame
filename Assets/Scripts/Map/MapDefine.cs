using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDefine
{
    public const string MapAssetFileName = "/MapAsset{0}.asset";
    public const string MapAssetFolderPath = "Assets/Map/Prefabs/MapData/MapAsset{0}";
    public const string MapTilesAssetFileName = "/MapTilesAsset{0}.asset";
    public const string MapTilesAssetFolderPath = "Assets/Map/Prefabs/MapData/MapTilesAsset{0}";
    public const string MapAssetFilePath = "Assets/Map/Prefabs/MapData/MapAsset{0}/MapAsset{1}.asset";
    public const string MapTilesAssetFilePath = "Assets/Map/Prefabs/MapData/MapTilesAsset{0}/MapTilesAsset{1}.asset";
    public const string TERRAIN_ASSET_PATH = "Assets/Map/Model/TerrainRes/";
    public const string TERRAIN_PREFAB_PATH = "Assets/Map/Prefabs/MapItem/";
    public const string MapDataSavePath = "Assets/Map/Prefabs/MapBlockData/MapData.bytes";
    public const string MapTilesDataSavePath = "Assets/Map/Prefabs/MapBlockData/MapData.bytes";

    public const string MapHideBlockDataSavePath = "Assets/Map/Prefabs/MapBlockData/HideBolckData.bytes";
    public const string MapHeightBlockDataSavePath = "Assets/Map/Prefabs/MapBlockData/HeightBolckData.bytes";
    public const string MapPropPostionDataSavePath = "Assets/Map/Prefabs/MapBlockData/MapPropPostionData.txt";
    public const string ClienMapPropPostionDataSavePath = "Assets/Map/Prefabs/MapBlockData/ClienMapPropPostionData.txt";//临时

    public const string MapHideBlockdfdfDataSavePath = "Assets/Map/Prefabs/MapBlockData/HideBolckdffData.bytes";


    public const int MapByteInterval = 6;


    public const string MapRoleCreatePointSavePath = "Assets/RoleCreatePosData.txt";
    public const string MapElementPath = "Assets/Map/Prefabs/MapElementPrefabs/{0}.prefab";
    public const string MapElementFilePath = "/Map/Prefabs/MapElementPrefabs/";//地图元素生成文件路劲
    public const string MapTilesPath = "Assets/Map/Prefabs/MapItem/TilesItem/{0}.prefab";
    public const string TilesMapFilePath = "/Map/Prefabs/MapItem/TilesItem/";//地图元素生成文件路劲

    public const string MAPKEYNAME = "{0}_{1}";
    public const string EXTENSION = ".asset";
    public const int MAPITEMTOTALSIZE = 2048;
    public const int MAPITEMSIZE = 256;//地图切割大小

    public const int MapElementSize = 16;//预设格子大小

    public const float MapBlockSize = 0.2f;

    //格子大小
    public const float GridSize_Main = 20;
    public const float GridSize_Port = 40;
    public const float GridSize_Edit = 32;

    public const float MinEditMapRange = 4f;//地图编辑的最小范围4米（编辑器MapEditView）

    public static int MapWidth = MAPITEMSIZE;
    public static int MapHeight = MAPITEMSIZE;

    public const int FileGridInterval = 256;
    public const int TilesGridInterval = 16;

    public static int MaxViewRowNum = 3; //创建最大行数
    public static int MaxViewColumnNum = 3; //创建最大列数

    public static Color[] MapBlockTypeColor = {
        new Color(0, 0, 0, 0.0f),
        new Color(0, 1, 0, 0.4f),
        new Color(1, 1, 1, 0.4f),
        new Color(0, 0, 1, 0.4f),
        new Color(0.5f, 0,0.5f , 0.6f)
    };

    //最小格子的宽
    public static float GetMinInterval
    {
        get
        {
            return MinEditMapRange / (640f / GridSize_Edit);
        }
    }
    //最小格子的宽
    public static float MapMinGridSize = 0.2f;
}

