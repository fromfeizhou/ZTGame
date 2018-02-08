using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapDefine
{
    public const string MAPITEMINFOASSET = "Assets/ResourcesLib/Config/MapAsset.asset";
    public const string TERRAIN_ASSET_PATH = "Assets/ResourcesLib/Map/TerrainRes/";
    public const string TERRAIN_PREFAB_PATH = "Assets/Prefabs/Map/MapItem/";
    public const string MAPITEM_TREE = "Assets/Prefabs/Map/tree.prefab";
    public const string MAPITEM_Wall01 = "Assets/Prefabs/Map/wall01.prefab";
    public const string MapDataSavePath = "Assets/MapData.txt";
    public const string MapRoleCreatePointSavePath = "Assets/RoleCreatePosData.txt";
    public const string MapItemPath = "Assets/Prefabs/Map/{0}.prefab";

    public const string MAPKEYNAME = "{0}_{1}";
    public const string EXTENSION = ".asset";
    public const int MAPITEMTOTALSIZE = 2048;
    public const int MAPITEMSIZE = 256;

    //格子大小
    public const float GridSize_Main = 20;
    public const float GridSize_Port = 40;
    public const float GridSize_Edit = 32;

    public const float MinEditMapRange = 4f;//地图编辑的最小范围4米（编辑器MapEditView）

    public static int MapWidth = MAPITEMSIZE;
    public static int MapHeight = MAPITEMSIZE;

    public static int MapViewRow = 1; //单屏行数
    public static int MapViewColumn = 1; //单屏列数

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
}

public enum eMapBlockType
{
    None,   //无
    Collect,//碰撞区域
    Hide,   //隐藏区域
    Event,  //事件
    playerPoint
    //Count,  //总数
}


[System.Serializable]
public class MapBlockData
{
    public int row;
    public int col;
    public eMapBlockType type;
    public string param;
    public override string ToString()
    {
        if (type == eMapBlockType.Event)
            return string.Format("{0}:{1}:{2}:{3}", row, col, (int)type, param);
        return string.Format("{0}:{1}:{2}", row, col, (int)type);
    }

    public static MapBlockData Parse(string content)
    {
        string[] contents = content.Split(':');
        MapBlockData tmpData = new MapBlockData();
        tmpData.row = int.Parse(contents[0]);
        tmpData.col = int.Parse(contents[1]);
        tmpData.type = (eMapBlockType)System.Enum.Parse(typeof(eMapBlockType), contents[2]);
        return tmpData;
    }

#if UNITY_EDITOR
    public bool IsInBlock(int rand, int scrRow, int scrCol)
    {
        return row >= scrRow && row < scrRow + rand && col >= scrCol && col < scrCol + rand;
    }
#endif
}

//地图 格子位置
public class MapTilePos
{
    public int Row = 0;
    public int Column = 0;

    public MapTilePos(int row = 0, int column = 0)
    {
        Row = row;
        Column = column;
    }
}

public class RoleTilePos
{
    public int row = 0;
    public int col = 0;
    public Vector3 pos = Vector3.zero;

    public static RoleTilePos Parse(string data)
    {
        RoleTilePos roleData = new RoleTilePos();
        string[] datas = data.Split(':');
        if (datas.Length > 1)
        {
            roleData.row = int.Parse(datas[0]);
            roleData.col = int.Parse(datas[1]);
            float offest = MapDefine.GetMinInterval * 0.5f;
            roleData.pos = new Vector3(roleData.row * MapDefine.GetMinInterval + offest, 0, roleData.col * MapDefine.GetMinInterval + offest);
        }
        else
            Debug.LogError("data is error!");

        return roleData;

    }

}

public class MapManager : Singleton<MapManager>
{
    private Dictionary<int, Dictionary<int, MapTileData>> _mapDataDic = null;
    private Dictionary<string, GameObject> _mapTerrainPrefabDic = null;    //地形预设
    private List<MapTileView> _mapViewList = null;
    private MapTilePos _mapTilePosCenter;  //地图中心
    private Vector3 _mapPosCenter = Vector3.zero;   //地图中心点
    private int _maxDataRow;    //地图数据最大行数
    private int _maxDataColumn;     //地图数据最大列数
    private MapTilePos _mapTilePos;

    private bool _isInit = false;
    private GameObject _floorPrefab;
    private GameObject _sceneLayer;


    private List<MapBlockData> _mapBlockData;
    private List<RoleTilePos> _roleTilePosList;

    #region 角色创建坐标
    private void InitRoleCreatPosData()
    {
        if (File.Exists(MapDefine.MapRoleCreatePointSavePath))
        {
            _roleTilePosList = new List<RoleTilePos>();
            string[] contents = File.ReadAllLines(MapDefine.MapRoleCreatePointSavePath);
            for (int i = 0; i < contents.Length; i++)
            {
                if (!string.IsNullOrEmpty(contents[i]))
                {
                    _roleTilePosList.Add(RoleTilePos.Parse(contents[i]));
                }
            }
        }
    }
    //获取角色创建坐标
    public Vector3 GetRoleCreatePoint(string posData)
    {
        string[] pos = posData.Split(':');
        if (pos.Length > 0)
        {
            int row = int.Parse(pos[0]);
            int col = int.Parse(pos[1]);
            int index = _roleTilePosList.FindIndex(a => a.row == row && a.col == col);
            if (index >= 0)
                return _roleTilePosList[index].pos;
        }
        return Vector3.zero;
        
    }

    #endregion


    private void InitMapData()
    {
        if (File.Exists(MapDefine.MapDataSavePath))
        {
            _roleTilePosList = new List<RoleTilePos>();
            _mapBlockData = new List<MapBlockData>();
            string[] contents = File.ReadAllLines(MapDefine.MapDataSavePath);
            for (int i = 0; i < contents.Length; i++)
            {
                if (!string.IsNullOrEmpty(contents[i]))
                {
                    _mapBlockData.Add(MapBlockData.Parse(contents[i]));

                    RoleTilePos temp = RoleTilePos.Parse(contents[i]);
                    if (temp != null)
                        _roleTilePosList.Add(temp);
                }
            }
        }
    }

    public void InitMap()
    {
        InitMapData();
        _sceneLayer = GameObject.Find("SceneMap");
        _mapTerrainPrefabDic = new Dictionary<string, GameObject>();

        _maxDataRow = 8;
        _maxDataColumn = 8;

        _mapDataDic = new Dictionary<int, Dictionary<int, MapTileData>>();
        for (int i = 0; i < _maxDataRow * _maxDataColumn; i++)
        {
            MapTileData tileData = new MapTileData();
            tileData.MapId = i + 1;
            tileData.Row = Mathf.FloorToInt(i / _maxDataColumn);
            tileData.Column = i % _maxDataColumn;
            if (!_mapDataDic.ContainsKey(tileData.Row))
            {
                _mapDataDic[tileData.Row] = new Dictionary<int, MapTileData>();
            }
            _mapDataDic[tileData.Row][tileData.Column] = tileData;
        }

        AssetManager.LoadAsset(MapDefine.MAPITEMINFOASSET, (obj, str) => mapAsset = obj as MapAsset);
        AssetManager.LoadAsset("Assets/Prefabs/Map/Floor.prefab", LoadFloorCom);
    }

    private MapAsset mapAsset;


    public List<MapBlockData> GetMapBlock(float minRow, float maxRow, float minCol, float maxCol)
    {
        return _mapBlockData.FindAll(a => a.row >= minRow && a.row < maxRow && a.col >= minCol && a.col < maxCol);
    }

    public eMapBlockType GetFloorColl(Vector3 pos)
    {
        int row = Mathf.RoundToInt(pos.x / MapDefine.GetMinInterval);
        int col = Mathf.RoundToInt(pos.z / MapDefine.GetMinInterval);//1280 * 5120);

        if (_mapBlockData != null && _mapBlockData.Count > 0)
        {
            int index = _mapBlockData.FindIndex(a => a.row == row && a.col == col);
            if (index >= 0)
                return _mapBlockData[index].type;
        }
        return eMapBlockType.None;
    }

    public MapBlockData GetCurMapBlock(Vector3 pos)
    {
        int row = Mathf.RoundToInt(pos.x / MapDefine.GetMinInterval);
        int col = Mathf.RoundToInt(pos.z / MapDefine.GetMinInterval);//1280 * 5120);

        if (_mapBlockData != null && _mapBlockData.Count > 0)
        {
            int index = _mapBlockData.FindIndex(a => a.row == row && a.col == col);
            if (index >= 0)
                return _mapBlockData[index];
        }
        return null;
    }


    public MapInfo GetMapInfiByPos(int row, int col)
    {
        string mapKey = row + "_" + col;
        int index = mapAsset.MapList.FindIndex(a => a.MapKey.Equals(mapKey));
        if (index >= 0)
            return mapAsset.MapList[index];
        return null;
    }

    public void AddTrrainPrefab(string key, GameObject prefab)
    {
        _mapTerrainPrefabDic.Add(key, prefab);
    }

    public GameObject GetTrrainPrefab(string key)
    {
        if (_mapTerrainPrefabDic.ContainsKey(key))
        {
            return _mapTerrainPrefabDic[key];
        }
        return null;
    }

    public void SafeGetTrrainPrefab(int row, int col, System.Action<GameObject> callback)
    {
        string mapKey = string.Format("{0}_{1}", row, col);
        if (_mapTerrainPrefabDic.ContainsKey(mapKey))
        {
            if (callback != null)
                callback(_mapTerrainPrefabDic[mapKey]);
            return;
        }
        string assetPath = MapDefine.TERRAIN_PREFAB_PATH + mapKey + ".prefab";
        AssetManager.LoadAsset(assetPath, (obj, str) =>
        {
            GameObject mapGo = obj as GameObject;
            AddTrrainPrefab(mapKey, mapGo);
            if (callback != null)
                callback(_mapTerrainPrefabDic[mapKey]);
        });
    }

    private void LoadFloorCom(Object target, string path)
    {
        _floorPrefab = target as GameObject;
        //资源加载完毕
        InitMapView();
    }

    public override void Destroy()
    {
        base.Destroy();
        if (null != _mapViewList)
        {
            for (int i = 0; i < _mapViewList.Count; i++)
            {
                if (_mapViewList[i] == null) continue;
                GameObject.Destroy(_mapViewList[i].gameObject);
            }
            _mapViewList.Clear();

            _mapDataDic = null;
            _mapViewList = null;
        }
        _mapDataDic = null;
    }


    public void SetMapCenterPos(Vector3 pos)
    {
        //if (_mapTilePosCenter == null)
        //{
        //    _mapTilePosCenter = new MapTilePos();
        //    UpdateMapView();
        //    return;
        //}
        if (_mapTilePosCenter==null||Mathf.Abs(_mapTilePosCenter.Column - Mathf.FloorToInt(pos.x / MapDefine.MapWidth)) >= 1 || Mathf.Abs(_mapTilePosCenter.Row - Mathf.FloorToInt(pos.z / MapDefine.MapHeight)) >= 1)
        {
            if (_mapTilePosCenter == null)
                _mapTilePosCenter = new MapTilePos();
            _mapTilePosCenter.Column = Mathf.FloorToInt(pos.x / MapDefine.MapWidth);
            _mapTilePosCenter.Row = Mathf.FloorToInt(pos.z / MapDefine.MapHeight);
            _mapPosCenter = pos;
            UpdateMapView();
        }
    }
    //获得地图中心坐标
    public Vector3 GetCenterPos()
    {
        return _mapPosCenter;
    }

    //初始化地图块
    private void InitMapView()
    {
        if (null == _sceneLayer)
        {
            return;
        }
        if (null == _mapViewList)
        {
            _mapViewList = new List<MapTileView>();
            for (int i = 0; i < MapDefine.MaxViewRowNum * MapDefine.MaxViewColumnNum; i++)
            {
                GameObject gameObject = GameObject.Instantiate(_floorPrefab);
                gameObject.transform.localPosition = new Vector3((i % MapDefine.MaxViewColumnNum) * MapDefine.MapWidth, 0, Mathf.Floor(i / MapDefine.MaxViewColumnNum) * MapDefine.MapHeight);
                //gameObject.transform.localPosition = new Vector3(Mathf.Floor(i / MapDefine.MaxViewColumnNum) * MapDefine.MapHeight, 0, (i % MapDefine.MaxViewColumnNum) * MapDefine.MapWidth);
                gameObject.transform.parent = _sceneLayer.transform;
                MapTileView tempTileView = gameObject.GetComponent<MapTileView>();
                _mapViewList.Add(tempTileView);
            }
        }
        _isInit = true;
       // UpdateMapView();
    }

    //刷新地图
    private void UpdateMapView()
    {
        if (_isInit == false)
        {
            return;
        }
        MapTilePos tmpTilePos = GetCurMapPosData();


        int beginX = tmpTilePos.Row - 1;
        int beginY = tmpTilePos.Column - 1;
        int endX = tmpTilePos.Row + 1;
        int endY = tmpTilePos.Column + 1;
        List<MapTileView> tempTileViews = new List<MapTileView>();
        for (int k = 0; k < _mapViewList.Count; k++)
        {
            MapTileView floor = _mapViewList[k];
            if (floor.IsNeedClear(beginX, endX, beginY, endY))
                tempTileViews.Add(floor);
        }
        int tempIndex = 0;
        for (int index = beginX; index <= endX; index++)
        {
            if (index < 0) continue;
            for (int j = beginY; j <= endY; j++)
            {
                if (j < 0 ) continue;
                bool isShow = false;
                for (int k = 0; k < _mapViewList.Count; k++)
                {
                    MapTileView tileView = _mapViewList[k];
                    MapTileData data = tileView.GetMapData();
                    if (data != null&&tileView.IsLoad && data.Column == j && data.Row == index)
                    {
                        isShow = true;
                        break;
                    }
                }
                if (isShow) continue;
                if (tempIndex < tempTileViews.Count)
                {
                    MapTileData targetTileData = _mapDataDic[index][j];
                    tempTileViews[tempIndex].setMapData(targetTileData);
                    tempTileViews[tempIndex].transform.position = new Vector3(MapDefine.MapWidth * j, 0, MapDefine.MapWidth * index);
                    tempIndex++;
                    continue;
                }
            }
        }
        _mapTilePos = tmpTilePos;
        //旧逻辑
        //if (_mapTilePos == null || Mathf.Abs(_mapTilePos.Row - tmpTilePos.Row) > MapDefine.MaxViewRowNum || Mathf.Abs(_mapTilePos.Column - tmpTilePos.Column) > MapDefine.MaxViewColumnNum)
        //{
        //    //全部刷新
        //    for (int i = 0; i < _mapViewList.Count; i++)
        //    {
        //        GameObject floor = _mapViewList[i];
        //        int row = tmpTilePos.Row + Mathf.FloorToInt(i / MapDefine.MaxViewColumnNum);
        //        int column = tmpTilePos.Column + Mathf.FloorToInt(i % MapDefine.MaxViewColumnNum);
        //        MapTileData data = _mapDataDic[row][column];
        //        floor.GetComponent<MapTileView>().setMapData(data);
        //    }
        //}
        //else
        //{
        //    //滚动刷新
        //    int rowNum = tmpTilePos.Row - _mapTilePos.Row;
        //    int column = tmpTilePos.Column - _mapTilePos.Column;
        //    ScrollMapView(rowNum, column);
        //}
        //_mapTilePos = tmpTilePos;
    }

    //滚动地图
    private void ScrollMapView(int rowNum, int columnNum)
    {
        //水平滚动  列位移
        if (columnNum > 0)
        {
            if (_mapTilePos.Column == 0) return;
            for (int i = 0; i < columnNum; i++)
            {
                
                //数据列数固定 当前列 + 列数
                int dataColumn = _mapTilePos.Column-1 + MapDefine.MaxViewColumnNum;
                for (int k = 0; k < MapDefine.MaxViewRowNum; k++)
                {
                    int floorBegin = k * MapDefine.MaxViewColumnNum;
                    int floorEnd = (k + 1) * MapDefine.MaxViewColumnNum - 1;
                    //移动列第一个 并赋值对应的格子数据
                    GameObject floor = _sceneLayer.transform.GetChild(floorBegin).gameObject;
                    floor.transform.SetSiblingIndex(floorEnd);
                    //数据行数递增
                    int dataRow = _mapTilePos.Row + k;
                    floor.GetComponent<MapTileView>().setMapData(_mapDataDic[dataRow][dataColumn]);
                    //更新位置
                    float tx = MapDefine.MapWidth * MapDefine.MaxViewColumnNum;
                    floor.transform.Translate(new Vector3(tx, 0, 0));
                }
                _mapTilePos.Column = _mapTilePos.Column + 1;
            }
        }
        else
        {
            columnNum = Mathf.Abs(columnNum);
            for (int i = 0; i < columnNum; i++)
            {
                //数据列数固定 当前列  - 1
                int dataColumn = _mapTilePos.Column - 2;
                if (dataColumn < 0) return;
                for (int k = 0; k < MapDefine.MaxViewRowNum; k++)
                {
                    int floorBegin = k * MapDefine.MaxViewColumnNum;
                    int floorEnd = (k + 1) * MapDefine.MaxViewColumnNum - 1;
                    //移动列最后一个 并赋值对应的格子数据
                    GameObject floor = _sceneLayer.transform.GetChild(floorEnd).gameObject;
                    floor.transform.SetSiblingIndex(floorBegin);

                    //数据行数递增
                    int dataRow = _mapTilePos.Row + k;
                    floor.GetComponent<MapTileView>().setMapData(_mapDataDic[dataRow][dataColumn]);
                    //更新位置
                    float tx = -MapDefine.MapWidth * MapDefine.MaxViewColumnNum;
                    floor.transform.Translate(new Vector3(tx, 0, 0));

                }
                _mapTilePos.Column = _mapTilePos.Column - 1;
            }
        }


        //垂直滚动  行位移
        if (rowNum > 0)
        {
            if (_mapTilePos.Row == 0) return;
            for (int i = 0; i < rowNum; i++)
            {
                //数据行数固定 当前列  + 总行数
                int dataRow = _mapTilePos.Row + MapDefine.MaxViewRowNum-1;
                for (int k = 0; k < MapDefine.MaxViewColumnNum; k++)
                {
                    int floorBegin = 0;
                    int floorEnd = MapDefine.MaxViewRowNum * MapDefine.MaxViewColumnNum - 1;
                    GameObject floor = _sceneLayer.transform.GetChild(floorBegin).gameObject;
                    floor.transform.SetSiblingIndex(floorEnd);
                    //数据列数递增
                    int dataColumn = _mapTilePos.Column + k;
                    floor.GetComponent<MapTileView>().setMapData(_mapDataDic[dataRow][dataColumn]);
                    //更新位置 
                    float tz = MapDefine.MapHeight * MapDefine.MaxViewRowNum;
                    //x旋转90度 平移改为对y处理（旧版 quad）
                    floor.transform.Translate(new Vector3(0, 0, tz));
                }
                _mapTilePos.Row = _mapTilePos.Row + 1;
            }
        }
        else
        {
            rowNum = Mathf.Abs(rowNum);
            for (int i = 0; i < rowNum; i++)
            {
                //数据行数固定 当前列  + 总行数 - 1
                int dataRow = _mapTilePos.Row - 2;
                if (dataRow < 0) return;
                for (int k = 0; k < MapDefine.MaxViewColumnNum; k++)
                {
                    int floorBegin = (MapDefine.MaxViewRowNum - 1) * MapDefine.MaxViewColumnNum + k;
                    int floorEnd = k;
                    GameObject floor = _sceneLayer.transform.GetChild(floorBegin).gameObject;
                    floor.transform.SetSiblingIndex(floorEnd);
                    //数据列数递增
                    int dataColumn = _mapTilePos.Column + k;
                    floor.GetComponent<MapTileView>().setMapData(_mapDataDic[dataRow][dataColumn]);

                    //更新位置
                    float tz = -MapDefine.MapHeight * MapDefine.MaxViewRowNum;
                    //x旋转90度 平移改为对y处理(旧版 quad)
                    floor.transform.Translate(new Vector3(0, 0, tz));
                }
                _mapTilePos.Row = _mapTilePos.Row - 1;
            }
        }
    }

    //获得起始方块(自动过滤最大值 以防数据溢出)
    private MapTilePos GetCurMapPosData()
    {

        //x移动 列位移
        int column = _mapTilePosCenter.Column;
        int columnMax = _maxDataColumn - MapDefine.MaxViewColumnNum+1;

        int row = _mapTilePosCenter.Row;
        int rowMax = _maxDataRow - MapDefine.MaxViewRowNum+1;

        column = column >= 0 ? column : 0;
        column = column >= columnMax ? columnMax : column;
        row = row >= 0 ? row : 0;
        row = row >= rowMax ? rowMax : row;
        return new MapTilePos(row, column);
    }


    public void Update(Vector3 pos)
    {
        if (_isInit == false)
        {
            return;
        }
        SetMapCenterPos(pos);
    }

}
