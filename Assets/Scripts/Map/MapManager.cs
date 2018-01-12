using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDefine
{

    public const string TERRAIN_ASSET_PATH = "Assets/ResourcesLib/Map/TerrainRes/";
    public const string TERRAIN_PREFAB_PATH = "Assets/Prefabs/Map/MapItem/";
    public const string TERRAIN_NAME = "Terrain_{0}_{1}";
    public const string EXTENSION = ".asset";
    public const int MAPITEMSIZE = 128;

    public static int MapWidth = MAPITEMSIZE;
    public static int MapHeight = MAPITEMSIZE;

    public static int MapViewRow = 1;       //单屏行数
    public static int MapViewColumn = 1;    //单屏列数

    public static int MaxViewRowNum = 4;     //创建最大行数
    public static int MaxViewColumnNum = 5;  //创建最大列数

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

public class MapManager
{


    private static MapManager _instance = null;
    private Dictionary<int, Dictionary<int, MapTileData>> _mapDataDic = null;
    private List<GameObject> _mapViewList = null;
    private MapTilePos _mapTilePosCenter = new MapTilePos();    //地图中心
    private int _maxDataRow;    //地图数据最大行数
    private int _maxDataColumn;     //地图数据最大列数
    private MapTilePos _mapTilePos;

    private bool _isInit = false;
    private GameObject _floorPrefab;
    private GameObject _sceneLayer;
    //Single 
    public static MapManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new MapManager();
            _instance._isInit = false;
            return _instance;
        }
        return _instance;
    }

    public void InitMap()
    {
        _sceneLayer = GameObject.Find("SceneMap");

        _maxDataRow = 5;
        _maxDataColumn = 9;

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

        AssetManager.LoadAsset("Assets/Prefabs/Map/Floor.prefab", LoadFloorCom);
    }

    private void LoadFloorCom(Object target, string path)
    {
        _floorPrefab = target as GameObject;
        //资源加载完毕
        InitMapView();
    }

    public void Destroy()
    {
        if (null != MapManager._instance)
        {

            _mapDataDic = null;
            _mapViewList = null;
            MapManager._instance = null;
        }
    }



    public void SetMapCenterPos(Vector3 pos)
    {
        if (Mathf.Abs(_mapTilePosCenter.Column - Mathf.FloorToInt(pos.x / MapDefine.MapWidth)) >= 1 || Mathf.Abs(_mapTilePosCenter.Row - Mathf.FloorToInt(pos.z / MapDefine.MapHeight)) >= 1)
        {
            _mapTilePosCenter.Column = Mathf.FloorToInt(pos.x / MapDefine.MapWidth);
            _mapTilePosCenter.Row = Mathf.FloorToInt(pos.z / MapDefine.MapHeight);
            UpdateMapView();
        }
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
            _mapViewList = new List<GameObject>();
            for (int i = 0; i < MapDefine.MaxViewRowNum * MapDefine.MaxViewColumnNum; i++)
            {
                GameObject gameObject = GameObject.Instantiate(_floorPrefab);
                gameObject.transform.localPosition = new Vector3(Mathf.Floor(i / MapDefine.MaxViewColumnNum) * MapDefine.MapHeight,0, (i % MapDefine.MaxViewColumnNum) * MapDefine.MapWidth);
                gameObject.transform.parent = _sceneLayer.transform;
                _mapViewList.Add(gameObject);
            }
        }
        _isInit = true;
        UpdateMapView();
    }

    //刷新地图
    private void UpdateMapView()
    {
        if (_isInit == false)
        {
            return;
        }
        MapTilePos tmpTilePos = GetCurMapPosData();
        if (_mapTilePos == null || Mathf.Abs(_mapTilePos.Row - tmpTilePos.Row) > MapDefine.MaxViewRowNum || Mathf.Abs(_mapTilePos.Column - tmpTilePos.Column) > MapDefine.MaxViewColumnNum)
        {
            //全部刷新
            for (int i = 0; i < _mapViewList.Count; i++)
            {
                GameObject floor = _mapViewList[i];
                int row = tmpTilePos.Row + Mathf.FloorToInt(i / MapDefine.MaxViewColumnNum);
                int column = tmpTilePos.Column + Mathf.FloorToInt(i % MapDefine.MaxViewColumnNum);
                MapTileData data = _mapDataDic[row][column];
                floor.GetComponent<MapTileView>().setMapData(data);
            }
        }
        else
        {
            //滚动刷新
            int rowNum = tmpTilePos.Row - _mapTilePos.Row;
            int column = tmpTilePos.Column - _mapTilePos.Column;
            ScrollMapView(rowNum, column);
        }
        _mapTilePos = tmpTilePos;
    }

    //滚动地图
    private void ScrollMapView(int rowNum, int columnNum)
    {
        //水平滚动  列位移
        if (columnNum > 0)
        {
            for (int i = 0; i < columnNum; i++)
            {
                //数据列数固定 当前列 + 列数
                int dataColumn = _mapTilePos.Column + MapDefine.MaxViewColumnNum;
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
                    float tx = MapDefine.MapWidth * MapDefine.MaxViewColumnNum ;
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
                int dataColumn = _mapTilePos.Column - 1;
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
            for (int i = 0; i < rowNum; i++)
            {
                //数据行数固定 当前列  + 总行数
                int dataRow = _mapTilePos.Row + MapDefine.MaxViewRowNum;
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
                    float tz =  MapDefine.MapHeight * MapDefine.MaxViewRowNum ;
                    //x旋转90度 平移改为对y处理
                    floor.transform.Translate(new Vector3(0, tz, 0));
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
                int dataRow = _mapTilePos.Row - 1;

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
                    //x旋转90度 平移改为对y处理
                    floor.transform.Translate(new Vector3(0, tz, 0));
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
        int columnMax = _maxDataColumn - MapDefine.MaxViewColumnNum;

        int row = _mapTilePosCenter.Row;
        int rowMax = _maxDataRow - MapDefine.MaxViewRowNum;

        column = column >= 0 ? column : 0;
        column = column >= columnMax ? columnMax : column;
        row = row >= 0 ? row : 0;
        row = row >= rowMax ? rowMax : row;
        return new MapTilePos(row, column);
    }
}
