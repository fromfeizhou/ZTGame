using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class MapElementView
{

    private MapAsset mapAsset;
    private Dictionary<string, Dictionary<string, MapElement>> AllMapElementDic =
        new Dictionary<string, Dictionary<string, MapElement>>();
    //预设网格资源列表
    private Dictionary<string, Dictionary<string, MapElementGrid>> AllMapElementGridDic =
        new Dictionary<string, Dictionary<string, MapElementGrid>>();
    //视野内预设列表
    private Dictionary<string, MapElement> visionElementDic = new Dictionary<string, MapElement>();
    private List<string> createElementList = new List<string>();    //创建列表
    private List<string> removeElementList = new List<string>();      //移除列表
    private Dictionary<string, GameObject> loadedObj = new Dictionary<string, GameObject>();


    private GameObject mapRoot;
    private GameObject elementRoot;

    private List<MapTileView> _mapViewList = null;


    private Dictionary<string, GameObject> _mapTerrainPrefabDic = null;    //地形预设
    private Dictionary<int, Dictionary<int, MapTileData>> _mapDataDic = null;

    private Transform lastRayObj;
    private Transform rayObj;

    private string mapKey;
    public void SetBigMapKey(Vector3 pos = default(Vector3))
    {
        mapKey = (int)pos.x / MapDefine.MAPITEMTOTALSIZE + "" + (int)pos.z / MapDefine.MAPITEMTOTALSIZE;
    }

    private IEnumerator _OnUpdateElementHandler;
    //初始化
    public void InitMapElementView(MapAsset data)
    {
        mapAsset = data;
        InitData();
        mapRoot = new GameObject("SceneMap");
        elementRoot = new GameObject("MapElement");
        _mapTerrainPrefabDic = new Dictionary<string, GameObject>();

        int intevar = MapDefine.MAPITEMTOTALSIZE / MapDefine.MAPITEMSIZE;
        int MapCount = intevar * intevar;
        _mapDataDic = new Dictionary<int, Dictionary<int, MapTileData>>();
        for (int i = 0; i < MapCount; i++)
        {
            MapTileData tileData = new MapTileData();
            tileData.MapId = i + 1;
            tileData.Row = Mathf.FloorToInt(i / intevar);
            tileData.Column = i % intevar;
            if (!_mapDataDic.ContainsKey(tileData.Row))
            {
                _mapDataDic[tileData.Row] = new Dictionary<int, MapTileData>();
            }
            _mapDataDic[tileData.Row][tileData.Column] = tileData;
        }


        InitTerrainView();
    }
    //地图建筑数据
    private void InitData()
    {
        if (mapAsset != null)
        {
            Dictionary<string, MapElement> tempElementDic = new Dictionary<string, MapElement>();
            for (int index = 0; index < mapAsset.elementList.Count; index++)
            {
                MapElement tempElement = mapAsset.elementList[index];
                tempElementDic[tempElement.elementKey] = tempElement;
            }
            AllMapElementDic[mapKey] = tempElementDic;
            Dictionary<string, MapElementGrid> tempElementGridDic = new Dictionary<string, MapElementGrid>();
            for (int index = 0; index < mapAsset.ElementGrids.Count; index++)
            {
                MapElementGrid tempElementGrid = mapAsset.ElementGrids[index];
                tempElementGridDic[tempElementGrid.gridKey] = tempElementGrid;
            }
            AllMapElementGridDic[mapKey] = tempElementGridDic;
        }
        else
        {
            Debug.LogError("MapAsset is null!!");
        }
    }

    private void InitTerrainView()
    {
        if (null == _mapViewList)
        {
            _mapViewList = new List<MapTileView>();
            for (int i = 0; i < MapDefine.MaxViewRowNum * MapDefine.MaxViewColumnNum; i++)
            {
                GameObject gameObject = new GameObject();
                gameObject.transform.localPosition = new Vector3((i % MapDefine.MaxViewColumnNum) * MapDefine.MapWidth, 0, Mathf.Floor(i / MapDefine.MaxViewColumnNum) * MapDefine.MapHeight);
                gameObject.transform.parent = mapRoot.transform;
                MapTileView tempTileView = gameObject.AddComponent<MapTileView>();
                _mapViewList.Add(tempTileView);
            }
        }
    }

    public void UpdateTerrainView(int row, int col)
    {

        int beginX = row - 1;
        int beginY = col - 1;
        int endX = row + 1;
        int endY = col + 1;
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
                if (j < 0) continue;
                bool isShow = false;
                for (int k = 0; k < _mapViewList.Count; k++)
                {
                    MapTileView tileView = _mapViewList[k];
                    MapTileData data = tileView.GetMapData();
                    if (data != null && tileView.IsLoad && data.Column == j && data.Row == index)
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
    }
    //射线检测（建筑屋顶逻辑）
    public void UpdateRoleRay(Vector3 pos)
    {
       
        Vector3 targetPos = pos + 100 * Vector3.up;
        Vector3 temp = pos-targetPos;
        RaycastHit[] hit;
        hit = Physics.RaycastAll(targetPos, temp.normalized, 10000f, LayerMask.GetMask("Roof"));
        if (hit.Length > 0) //建筑屋顶 
        {
            lastRayObj = rayObj;
            rayObj = hit[0].collider.transform;
        }
        else
        {
            rayObj = null;
        }

        if (lastRayObj != rayObj)
        {
            SetRayObjEnabel(lastRayObj, true);
            SetRayObjEnabel(rayObj, false);
        }

    }

    private void SetRayObjEnabel(Transform obj,bool isShow)
    {
        if(obj==null)return;
        for (int index = 0; index < obj.childCount; index++)
        {
            obj.GetChild(index).gameObject.SetActive(isShow);
        }

    }

    public void UpdateElementView(Vector3 pos, int gridX, int gridY)
    {
        Dictionary<string, MapElement> elementDic = new Dictionary<string, MapElement>();
        int bigMapX = (int)pos.x / MapDefine.MAPITEMTOTALSIZE;
        int bigMapY = (int)pos.z / MapDefine.MAPITEMTOTALSIZE;
        string bigMapKey = bigMapX + "" + bigMapY;
        int beginX = (int)gridX - 1;
        int beginY = (int)gridY - 1;
        int endX = (int)gridX + 1;
        int endY = (int)gridY + 1;
        for (int i = beginX; i <= endX; i++)
        {
            if (i < 0) continue;
            for (int j = beginY; j <= endY; j++)
            {
                if (j < 0) continue;
                string gridKey = i + "_" + j;
                Dictionary<string, MapElementGrid> tempGridDataDic;
                if (AllMapElementGridDic.TryGetValue(bigMapKey, out tempGridDataDic))
                {
                    MapElementGrid tempGridData;
                    if (tempGridDataDic.TryGetValue(gridKey, out tempGridData))
                    {
                        List<string> tempElementKeyList = tempGridData.elementKeyList;
                        for (int index = 0; index < tempElementKeyList.Count; index++)
                        {
                            MapElement element;
                            Dictionary<string, MapElement> tempElementDic;
                            if (AllMapElementDic.TryGetValue(bigMapKey, out tempElementDic))
                                if (tempElementDic.TryGetValue(tempElementKeyList[index], out element))
                                {
                                    if (!elementDic.ContainsKey(element.elementKey))
                                        elementDic[element.elementKey] = element;
                                }
                        }
                    }
                }
            }
        }
        var needClearElementDic = visionElementDic.Keys.Except(elementDic.Keys);
        var needLoadElementDic = elementDic.Keys.Except(visionElementDic.Keys);

        createElementList.Clear();
        removeElementList.Clear();

        foreach (var key in needLoadElementDic)
        {
            //MapElement elementData = elementDic[key];
            //MapElementInfo elementInfo = elementData.elementInfo;
            //string elementAssetPath = string.Format(MapDefine.MapElementPath, elementData.elementType);
            //AssetManager.LoadAsset(elementAssetPath, (obj, str) =>
            //{
            //    if (obj != null)
            //    {
            //        GameObject assetTree = obj as GameObject;
            //        Transform element = GameObject.Instantiate(assetTree).transform;
            //        element.SetParent(elementRoot.transform);
            //        element.position = elementInfo.Pos;
            //        element.eulerAngles = elementInfo.Angle;
            //        element.localScale = elementInfo.Scale;

            //        loadedObj[elementData.elementKey] = element.gameObject;
            //    }
            //});
            createElementList.Add(key);


        }
        foreach (var key in needClearElementDic)
        {
            //if (loadedObj.ContainsKey(key))
            //{
            //    GameObject tempObj = loadedObj[key];
            //    GameObject.Destroy(tempObj);
            //    loadedObj.Remove(key);
            //}
            removeElementList.Add(key);
        }
        visionElementDic = elementDic;

        if(createElementList.Count > 0 || removeElementList.Count > 0)
        {
            if (null == _OnUpdateElementHandler)
            {
                _OnUpdateElementHandler = OnUpdateElement();
                ZTSceneManager.GetInstance().StartCoroutine(_OnUpdateElementHandler);
            }
        }
    }

    IEnumerator OnUpdateElement()
    {
        while (createElementList.Count > 0 || removeElementList.Count > 0)
        {
            Update();
            yield return null;
        }
        _OnUpdateElementHandler = null;
    }

    public void Update()
    {
        ClearElementInList();
        CreateElementInList();
    }

    public void ClearElementInList()
    {
        if(removeElementList == null || removeElementList.Count == 0)
        {
            return;
        }
        string key = removeElementList[0];
        removeElementList.RemoveAt(0);

        if(loadedObj.ContainsKey(key))
        {
            GameObject tempObj = loadedObj[key];
            GameObject.Destroy(tempObj);
            loadedObj.Remove(key);
        }
    }

    public void CreateElementInList()
    {
        if (createElementList == null || createElementList.Count == 0)
        {
            return;
        }
        string key = createElementList[0];
        createElementList.RemoveAt(0);

        MapElement elementData = visionElementDic[key];
        MapElementInfo elementInfo = elementData.elementInfo;
        string elementAssetPath = string.Format(MapDefine.MapElementPath, elementData.elementType);
        AssetManager.LoadAsset(elementAssetPath, (obj, str) =>
        {
            if (obj != null )//&& visionElementDic.ContainsKey(key))
            {
                GameObject assetTree = obj as GameObject;
                Transform element = GameObject.Instantiate(assetTree).transform;
                element.SetParent(elementRoot.transform);
                element.position = elementInfo.Pos;
                element.eulerAngles = elementInfo.Angle;
                element.localScale = elementInfo.Scale;

                loadedObj[elementData.elementKey] = element.gameObject;
            }
        });
    }
}
