using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

public enum eMapBlockType
{
    None,   //无
    Collect,//碰撞区域
    Hide,   //隐藏区域
    Height,
    Event,  //事件
    PlayerPoint,

    //Count,  //总数
}


[System.Serializable]
public class MapBlockData
{
    public int row;
    public int col;
    public eMapBlockType type;
    public int paramValue;
    private ByteBuffer btyeBuffer;

   
    public override string ToString()
    {
        if (type == eMapBlockType.Event || type == eMapBlockType.Hide)
            return string.Format("{0}:{1}:{2}:{3}", row, col, (int)type, paramValue);
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

    public void SetByteBuffer()
    {
        if (btyeBuffer == null)
        {
            btyeBuffer = new ByteBuffer();
            Int16 x = (Int16)row;
            Int16 y = (Int16)col;
            Int16 paramData = (Int16)paramValue;
            btyeBuffer.WriteInt16(x,false);
            btyeBuffer.WriteInt16(y, false);
            btyeBuffer.WriteInt16(paramData, false);
        }
    }
    public byte[] GetBytes()
    {
        SetByteBuffer();
        return btyeBuffer.ToBytes();
    }
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
        RoleTilePos roleData = null;
        string[] datas = data.Split(':');
        if (datas.Length > 1)
        {
            if (int.Parse(datas[2]) == (int)eMapBlockType.PlayerPoint)
            {
                roleData = new RoleTilePos();
                roleData.row = int.Parse(datas[0]);
                roleData.col = int.Parse(datas[1]);
                float offest = MapDefine.GetMinInterval * 0.5f;
                roleData.pos = new Vector3(roleData.row * MapDefine.GetMinInterval + offest, 0, roleData.col * MapDefine.GetMinInterval + offest);
            }
        }
        else
            Debug.LogError("data is error!");

        return roleData;

    }

}



public class MapManager : Singleton<MapManager>
{
    private MapTilePos _mapTilePosCenter;  //地图中心
    private bool _isInit = false;
    private List<MapBlockData> _mapBlockData = new List<MapBlockData>();
    private string currentBigMapIndex = "00";
    private Vector2 currElementGrid = new Vector2(-100f, -100f);

    private Action<string, float, float> _mapUpdateProces;
    private List<string> assetPaths;
    private List<UnityAction<Object, string>> assetCallbackList;

    private MapElementView mapView;
    private MapTileViewMgr mapTilesViewMgr;
    private byte[] ColliderDatas;
    private byte[] hideDatas;
    private byte[] heightDatas;
    private Dictionary<string, MapBlockData> mapBlockDataDic;
    private Dictionary<string, MapBlockData> mapHeightBlockData;

    private List<string> LoadTipsText = new List<string>()
    {
        "加载地图碰撞数据...",
        "加载地图草地数据...",
        "加载地图高度数据...",
        "加载地图预设数据...",
    };

    private List<string> LoadFinishTipsText = new List<string>()
    {
        "加载地图碰撞数据完成",
        "加载地图草地数据完成",
        "加载地图高度数据完成",
        "加载地图预设数据完成",
    };



    public void InitMap(Action<string, float, float> mapUpdateProcess, Vector3 pos = default(Vector3))
    {
        _isInit = false;
        mapBlockDataDic = new Dictionary<string, MapBlockData>();
        mapHeightBlockData=new Dictionary<string, MapBlockData>();
        Debug.Log("MapManager:Init");
        mapView = new MapElementView();
        mapView.SetBigMapKey(pos);
        mapTilesViewMgr =new MapTileViewMgr();
        mapTilesViewMgr.SetBigMapKey(pos);
        _mapUpdateProces = mapUpdateProcess;
        loadIndex = 0;
        assetPaths = new List<string>();
        int index = 0;
        assetPaths.Add(MapDefine.MapDataSavePath);
        assetPaths.Add(MapDefine.MapHideBlockDataSavePath);
        assetPaths.Add(MapDefine.MapHeightBlockDataSavePath);
        assetPaths.Add(String.Format(MapDefine.MapTilesAssetFilePath, currentBigMapIndex, currentBigMapIndex));
        assetPaths.Add(String.Format(MapDefine.MapAssetFilePath, currentBigMapIndex, currentBigMapIndex));

        assetCallbackList = new List<UnityAction<Object, string>>();
        assetCallbackList.Add(LoadColliderBlockData);
        assetCallbackList.Add(LoadHideBlockData);
        assetCallbackList.Add(LoadHeightBlockData);
        assetCallbackList.Add(LoadMapTilesAsset);
        assetCallbackList.Add(LoadMapAsset);
        assetCallbackList.Add((x, y) => { SetMapCenterPos(pos); });
        LoadNextAsset();

    }


    #region 数据加载
    private int loadIndex = 0;
    private void LoadNextAsset()
    {
        if (loadIndex >= assetPaths.Count)
        {
            _isInit = true;
            if (loadIndex < assetCallbackList.Count)
                assetCallbackList[loadIndex](null, null);
            Debug.Log("All MapAsset Load Finish!!! ");
            return;
        }
        if (_mapUpdateProces != null)
            _mapUpdateProces(LoadTipsText[loadIndex], 0f, 1f);
        AssetManager.LoadAsset(assetPaths[loadIndex], OnLoadFinish);
    }

    private void OnLoadFinish(Object target, string path)
    {
        assetCallbackList[loadIndex](target, path);
        if (_mapUpdateProces != null)
            _mapUpdateProces(LoadFinishTipsText[loadIndex], 1f, 1f);
        loadIndex++;
        LoadNextAsset();
    }
    //加载地图预设数据资源列表
    private void LoadMapAsset(Object target, string path)
    {
        mapView.InitMapElementView(target as MapAsset);
    }

    //加载地图瓦片预设数据资源列表
    private void LoadMapTilesAsset(Object target, string path)
    {
        mapTilesViewMgr.InitMapTilesView(target as MapAsset);
    }
    private void LoadColliderBlockData(Object target, string path)
    {
        TextAsset txt = target as TextAsset;
        if (txt != null)
            ColliderDatas = txt.bytes;
    }
    private void LoadHideBlockData(Object target, string path)
    {
        TextAsset txt = target as TextAsset;
        if (txt != null)
            SetMapBlockDic(txt.bytes, eMapBlockType.Hide, mapBlockDataDic);
    }
    private void LoadHeightBlockData(Object target, string path)
    {
        TextAsset txt = target as TextAsset;
        if (txt != null)
            SetMapBlockDic(txt.bytes, eMapBlockType.Height, mapHeightBlockData);
    }

    private void SetMapBlockDic(byte[] datas, eMapBlockType type,Dictionary<string,MapBlockData> dic)
    {
        int count = datas.Length / MapDefine.MapByteInterval;
        for (int index = 0; index < count; index++)
        {
            MapBlockData tempData = new MapBlockData();
            byte[] temp = new byte[MapDefine.MapByteInterval];
            Array.Copy(datas, index * MapDefine.MapByteInterval, temp, 0, MapDefine.MapByteInterval);
            ByteBuffer tempBuffer = new ByteBuffer(temp);
            tempData.row = tempBuffer.ReadInt16();
            tempData.col = tempBuffer.ReadInt16();
            int tempInt = tempBuffer.ReadInt16();
            if (type == eMapBlockType.Hide)
            {
                float tempFloat = tempInt * 0.01f;
                tempInt = (int)tempFloat;
                tempData.paramValue = tempInt;
            }
            else//高度float，取数据时候转
                tempData.paramValue = tempInt;
            tempData.type = type;
            dic[tempData.row + "_" + tempData.col] = tempData;
        }
    }
    #endregion
    public List<MapBlockData> GetMapBlock(float minRow, float maxRow, float minCol, float maxCol)
    {
        return _mapBlockData.FindAll(a => a.row >= minRow && a.row < maxRow && a.col >= minCol && a.col < maxCol);
    }
    public eMapBlockType GetFloorColl(Vector3 pos)
    {
        int row = Mathf.RoundToInt(pos.x / MapDefine.MapMinGridSize);
        int col = Mathf.RoundToInt(pos.z / MapDefine.MapMinGridSize);//1280 * 5120);
        if (_mapBlockData != null && _mapBlockData.Count > 0)
        {
            int index = _mapBlockData.FindIndex(a => a.row == row && a.col == col);
            if (index >= 0)
                return _mapBlockData[index].type;
        }
        return eMapBlockType.None;
    }

    //获取草丛或者碰撞数据
    public MapBlockData GetCurMapBlock(Vector3 pos)
    {
        int row = Mathf.RoundToInt(pos.x / MapDefine.MapMinGridSize);
        int col = Mathf.RoundToInt(pos.z / MapDefine.MapMinGridSize);
        MapBlockData tempData = null;
        int index = row + col * 10240;
        int byteRow = index / 8;
        int byteCol = index % 8;
        if (byteRow < ColliderDatas.Length)//取碰撞
        {
            byte curByte = ColliderDatas[byteRow];
            byte temp = (byte)Mathf.Pow(2, byteCol);
            int value = curByte & temp;
            
            if (value >= 1)
            {
                tempData = new MapBlockData();
                tempData.row = row;
                tempData.col = col;
                tempData.type = eMapBlockType.Collect;
            }
        }
        
        if (tempData == null)//取草丛
        {
          //  Debug.LogError(col + "  " + row + " asdfasd");

            if (mapBlockDataDic.TryGetValue(row + "_" + col, out tempData))
            {
              //  Debug.LogError(tempData.col+"  "+tempData.row+"  >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                return tempData;
            }
        }
         return tempData;
    }

    public bool GetCurMapBlockHeight(Vector3 pos,ref float height)
    {
        int row = Mathf.RoundToInt(pos.x / MapDefine.MapMinGridSize);
        int col = Mathf.RoundToInt(pos.z / MapDefine.MapMinGridSize);
        MapBlockData tempData = null;
        if (mapHeightBlockData.TryGetValue(row + "_" + col, out tempData))
        {
            height = tempData.paramValue * 0.01f;
            return true;
        }
        return false;
    }

    public override void Destroy()
    {
        base.Destroy();
       
    }

    public void Update(Vector3 pos)
    {
        if (_isInit == false)
        {
            return;
        }
        SetMapCenterPos(pos);
    }

    private float maptileInterval = MapDefine.TilesGridInterval;//MapDefine.MapWidth / 4f;
    public void SetMapCenterPos(Vector3 pos)
    {
        if (_mapTilePosCenter == null || Mathf.Abs(_mapTilePosCenter.Column - Mathf.FloorToInt(pos.z / maptileInterval)) >= 1 || Mathf.Abs(_mapTilePosCenter.Row - Mathf.FloorToInt(pos.x / maptileInterval)) >= 1)
        {
            if (_mapTilePosCenter == null)
                _mapTilePosCenter = new MapTilePos();
            _mapTilePosCenter.Row = Mathf.FloorToInt(pos.x / maptileInterval);
            _mapTilePosCenter.Column = Mathf.FloorToInt(pos.z / maptileInterval);

            mapTilesViewMgr.UpdateTilesView(pos, _mapTilePosCenter.Row, _mapTilePosCenter.Column);
        }

        int tempX = Mathf.FloorToInt(pos.x / MapDefine.MapElementSize);
        int tempY = Mathf.FloorToInt(pos.z / MapDefine.MapElementSize);
        if ( Mathf.Abs(currElementGrid.x - tempX) >= 1 || Mathf.Abs(currElementGrid.y - tempY) >= 1)
        {
            currElementGrid.x = tempX;
            currElementGrid.y = tempY;
            mapView.UpdateElementView(pos, tempX, tempY);
        }

       
        mapView.UpdateRoleRay(pos);
    }


    //初始化地图块
    //private void InitMapView()
    //{
    //    if (null == _sceneLayer)
    //    {
    //        return;
    //    }
    //    if (null == _mapViewList)
    //    {
    //        _mapViewList = new List<MapTileView>();
    //        for (int i = 0; i < MapDefine.MaxViewRowNum * MapDefine.MaxViewColumnNum; i++)
    //        {
    //            GameObject gameObject = GameObject.Instantiate(_floorPrefab);
    //            gameObject.transform.localPosition = new Vector3((i % MapDefine.MaxViewColumnNum) * MapDefine.MapWidth, 0, Mathf.Floor(i / MapDefine.MaxViewColumnNum) * MapDefine.MapHeight);
    //            //gameObject.transform.localPosition = new Vector3(Mathf.Floor(i / MapDefine.MaxViewColumnNum) * MapDefine.MapHeight, 0, (i % MapDefine.MaxViewColumnNum) * MapDefine.MapWidth);
    //            gameObject.transform.parent = _sceneLayer.transform;
    //            MapTileView tempTileView = gameObject.GetComponent<MapTileView>();
    //            _mapViewList.Add(tempTileView);
    //        }
    //    }
    //    _isInit = true;

    //  //  UpdateMapView();
    //}

    //private void UpdateTilesView(Vector3 pos)
    //{
    //    if (_isInit == false)
    //        return;
    //    Dictionary<string, MapElement> elementDic = new Dictionary<string, MapElement>();
    //    int bigMapX = (int)pos.x / MapDefine.MAPITEMTOTALSIZE;
    //    int bigMapY = (int)pos.z / MapDefine.MAPITEMTOTALSIZE;
    //    string bigMapKey = bigMapX + "" + bigMapY;
    //    int beginX = (int)currElementGrid.x - 1;
    //    int beginY = (int)currElementGrid.y - 1;
    //    int endX = (int)currElementGrid.x + 1;
    //    int endY = (int)currElementGrid.y + 1;
    //    for (int i = beginX; i <= endX; i++)
    //    {
    //        if (i < 0) continue;
    //        for (int j = beginY; j <= endY; j++)
    //        {
    //            if (j < 0) continue;
    //            string gridKey = i + "_" + j;
    //            Dictionary<string, MapElementGrid> tempGridDataDic;
    //            if (AllMapElementGridDic.TryGetValue(bigMapKey, out tempGridDataDic))
    //            {
    //                MapElementGrid tempGridData;
    //                if (tempGridDataDic.TryGetValue(gridKey, out tempGridData))
    //                {
    //                    List<string> tempElementKeyList = tempGridData.elementKeyList;
    //                    for (int index = 0; index < tempElementKeyList.Count; index++)
    //                    {
    //                        MapElement element;
    //                        Dictionary<string, MapElement> tempElementDic;
    //                        if (AllMapElementDic.TryGetValue(bigMapKey, out tempElementDic))
    //                            if (tempElementDic.TryGetValue(tempElementKeyList[index], out element))
    //                            {
    //                                if (!elementDic.ContainsKey(element.elementKey))
    //                                    elementDic[element.elementKey] = element;
    //                            }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    var needClearElementDic = visionElementDic.Keys.Except(elementDic.Keys);
    //    var needLoadElementDic = elementDic.Keys.Except(visionElementDic.Keys);
    //    foreach (var key in needLoadElementDic)
    //    {
    //        MapElement elementData = elementDic[key];
    //        MapElementInfo elementInfo = elementData.elementInfo;
    //        string elementAssetPath = string.Format(MapDefine.MapElementPath, elementData.elementType);
    //        AssetManager.LoadAsset(elementAssetPath, (obj, str) =>
    //        {
    //            if (obj != null)
    //            {
    //                GameObject assetTree = obj as GameObject;
    //                Transform element = GameObject.Instantiate(assetTree).transform;
    //                element.SetParent(_mapElementRoot.transform);
    //                element.position = elementInfo.Pos;
    //                element.eulerAngles = elementInfo.Angle;
    //                element.localScale = elementInfo.Scale;
    //                BuildingZTCollider tempcollider = element.GetComponent<BuildingZTCollider>();
    //                //if (tempcollider != null)
    //                //{
    //                //    ICharaBattle tempBattle = ZTBattleSceneManager.GetInstance().GetCharaById(PlayerModule.GetInstance().RoleID) as ICharaBattle;
    //                //    if (tempBattle != null)
    //                //        tempcollider.SetTarget(tempBattle.Collider);
    //                //}
    //                loadedObj[elementData.elementKey] = element.gameObject;
    //            }
    //        });
    //    }
    //    foreach (var key in needClearElementDic)
    //    {
    //        if (loadedObj.ContainsKey(key))
    //        {
    //            GameObject tempObj = loadedObj[key];
    //            GameObject.Destroy(tempObj);
    //            loadedObj.Remove(key);
    //        }
    //    }
    //    visionElementDic = elementDic;

    //}

    //刷新地图
    //private void UpdateMapView()
    //{
    //    if (_isInit == false)
    //    {
    //        return;
    //    }
    //    MapTilePos tmpTilePos = GetCurMapPosData();


    //    int beginX = tmpTilePos.Row - 1;
    //    int beginY = tmpTilePos.Column - 1;
    //    int endX = tmpTilePos.Row + 1;
    //    int endY = tmpTilePos.Column + 1;
    //    List<MapTileView> tempTileViews = new List<MapTileView>();
    //    for (int k = 0; k < _mapViewList.Count; k++)
    //    {
    //        MapTileView floor = _mapViewList[k];
    //        if (floor.IsNeedClear(beginX, endX, beginY, endY))
    //            tempTileViews.Add(floor);
    //    }
    //    int tempIndex = 0;
    //    for (int index = beginX; index <= endX; index++)
    //    {
    //        if (index < 0) continue;
    //        for (int j = beginY; j <= endY; j++)
    //        {
    //            if (j < 0) continue;
    //            bool isShow = false;
    //            for (int k = 0; k < _mapViewList.Count; k++)
    //            {
    //                MapTileView tileView = _mapViewList[k];
    //                MapTileData data = tileView.GetMapData();
    //                if (data != null && tileView.IsLoad && data.Column == j && data.Row == index)
    //                {
    //                    isShow = true;
    //                    break;
    //                }
    //            }
    //            if (isShow) continue;
    //            if (tempIndex < tempTileViews.Count)
    //            {
    //                MapTileData targetTileData = _mapDataDic[index][j];
    //                tempTileViews[tempIndex].setMapData(targetTileData);
    //                tempTileViews[tempIndex].transform.position = new Vector3(MapDefine.MapWidth * j, 0, MapDefine.MapWidth * index);
    //                tempIndex++;
    //                continue;
    //            }
    //        }
    //    }
    //    _mapTilePos = tmpTilePos;

    //}

    //获得起始方块(自动过滤最大值 以防数据溢出)
    //private MapTilePos GetCurMapPosData()
    //{

    //    //x移动 列位移
    //    int column = _mapTilePosCenter.Column;
    //    int columnMax = _maxDataColumn - MapDefine.MaxViewColumnNum + 1;

    //    int row = _mapTilePosCenter.Row;
    //    int rowMax = _maxDataRow - MapDefine.MaxViewRowNum + 1;

    //    column = column >= 0 ? column : 0;
    //    column = column >= columnMax ? columnMax : column;
    //    row = row >= 0 ? row : 0;
    //    row = row >= rowMax ? rowMax : row;
    //    return new MapTilePos(row, column);
    //}




}
