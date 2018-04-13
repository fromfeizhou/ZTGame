using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLua;
public class MapTileViewMgr
{


    private MapAsset mapAsset;
    private Dictionary<string, Dictionary<string, MapElement>> AllMapTilesDic =
        new Dictionary<string, Dictionary<string, MapElement>>();
    //预设网格资源列表
    private Dictionary<string, Dictionary<string, MapElementGrid>> AllMapTilesGridDic =
        new Dictionary<string, Dictionary<string, MapElementGrid>>();
    //视野内预设列表
    private Dictionary<string, MapElement> visionTilesDic = new Dictionary<string, MapElement>();
    private List<string> createTilesList = new List<string>();    //创建列表
    private List<string> disableTilesList = new List<string>();      //移除列表
    private Dictionary<string, GameObject> activeObj = new Dictionary<string, GameObject>();//激活的elementObj
    private List<GameObject> disabelObj = new List<GameObject>();//Element隐藏列表
    private GameObject TilesRoot;



    private string mapKey;
    public void SetBigMapKey(Vector3 pos = default(Vector3))
    {
        mapKey = (int)pos.x / MapDefine.MAPITEMTOTALSIZE + "" + (int)pos.z / MapDefine.MAPITEMTOTALSIZE;
    }

    private IEnumerator _OnUpdateElementHandler;
    //初始化
    public void InitMapTilesView(MapAsset data)
    {
        mapAsset = data;
        InitData();
        TilesRoot = new GameObject("MapTilesRoot");
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
            AllMapTilesDic[mapKey] = tempElementDic;
            Dictionary<string, MapElementGrid> tempElementGridDic = new Dictionary<string, MapElementGrid>();
            for (int index = 0; index < mapAsset.ElementGrids.Count; index++)
            {
                MapElementGrid tempElementGrid = mapAsset.ElementGrids[index];
                tempElementGridDic[tempElementGrid.gridKey] = tempElementGrid;
            }
            AllMapTilesGridDic[mapKey] = tempElementGridDic;
        }
        else
        {
            Debug.LogError("MapAsset is null!!");
        }
    }




    //刷新地图元素
    public void UpdateTilesView(Vector3 pos, int gridX, int gridY)
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
                if (AllMapTilesGridDic.TryGetValue(bigMapKey, out tempGridDataDic))
                {
                    MapElementGrid tempGridData;
                    if (tempGridDataDic.TryGetValue(gridKey, out tempGridData))
                    {
                        List<string> tempElementKeyList = tempGridData.elementKeyList;
                        for (int index = 0; index < tempElementKeyList.Count; index++)
                        {
                            MapElement element;
                            Dictionary<string, MapElement> tempElementDic;
                            if (AllMapTilesDic.TryGetValue(bigMapKey, out tempElementDic))
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
        var needClearElementDic = activeObj.Keys.Except(elementDic.Keys);
        var needLoadElementDic = elementDic.Keys.Except(activeObj.Keys);
        createTilesList.Clear();
        disableTilesList.Clear();
        foreach (var key in needLoadElementDic)
            createTilesList.Add(key);

        foreach (var key in needClearElementDic)
            disableTilesList.Add(key);

        visionTilesDic = elementDic;
        // if (disableTilesList.Count > 0)
        //    ClearElementInList();
        if (createTilesList.Count > 0 || disableTilesList.Count > 0)
        {
            if (null == _OnUpdateElementHandler)
            {
                _OnUpdateElementHandler = OnUpdateElement();
                ZTSceneManager.GetInstance().StartCoroutine(_OnUpdateElementHandler);
            }
        }
        if (disabelObj.Count > 500)
        {
            List<GameObject> tempDisableList = disabelObj.GetRange(0, disabelObj.Count / 2);
            disabelObj.RemoveRange(0, disabelObj.Count / 2);
            ZTSceneManager.GetInstance().StartCoroutine(DestroyElementList(tempDisableList));
        }
    }
    //分帧加载或者清理地图元素
    IEnumerator OnUpdateElement()
    {
        while (createTilesList.Count > 0 || disableTilesList.Count > 0)
        {
            Update();
            yield return null;
        }
        _OnUpdateElementHandler = null;
    }

    public void Update()
    {
        ClearElementInList();//active转disable
        CreateElementInList();
    }

    //清理地图元素（放入未激活列表）
    public void ClearElementInList()
    {
        if (disableTilesList == null || disableTilesList.Count == 0)
        {
            return;
        }
        string key = disableTilesList[0];
        disableTilesList.RemoveAt(0);
        if (activeObj.ContainsKey(key))
        {
            GameObject tempObj = activeObj[key];
            if (tempObj != null)
            {
                tempObj.SetActive(false);
                disabelObj.Add(tempObj);
            }
            activeObj.Remove(key);
        }
    }
    //创建地图元素
    public void CreateElementInList()
    {
        if (createTilesList == null || createTilesList.Count == 0)
        {
            return;
        }
        string key = createTilesList[0];
        createTilesList.RemoveAt(0);
        MapElement elementData = visionTilesDic[key];
        MapElementInfo elementInfo = elementData.elementInfo;
        activeObj[elementData.elementKey] = null;
     //   if (!elementData.elementType.Contains("Map"))
        //    Debug.LogError("Load>>>>>>" + elementData.elementKey);
        GameObject tempObj = GetObjByDisable(elementData.elementType);
        if (tempObj != null)
        {
            tempObj.transform.position = elementInfo.Pos;
            tempObj.transform.eulerAngles = elementInfo.Angle;
            tempObj.transform.localScale = elementInfo.Scale;
            tempObj.SetActive(true);
            activeObj[elementData.elementKey] = tempObj;
        }
        else
        {
            string elementAssetPath = string.Format(MapDefine.MapTilesPath, elementData.elementType);
            AssetManager.LoadAsset(elementAssetPath, (obj, str) =>
            {
                if (obj != null)//&& visionTilesDic.ContainsKey(key))
                {
                    if (activeObj.ContainsKey(elementData.elementKey))
                    {
                        GameObject assetTree = obj as GameObject;
                        Transform element = GameObject.Instantiate(assetTree).transform;
                        element.SetParent(TilesRoot.transform);
                        element.position = elementInfo.Pos;
                        element.eulerAngles = elementInfo.Angle;
                        element.localScale = elementInfo.Scale;
                        activeObj[elementData.elementKey] = element.gameObject;
                    }
                }
            });
        }
    }
    //从缓存获取
    private GameObject GetObjByDisable(string elementType)
    {
        GameObject tempObj = null;
        for (int index = 0; index < disabelObj.Count; index++)
        {
            if (disabelObj[index].name.Contains(elementType))
            {
                tempObj = disabelObj[index];
                disabelObj.RemoveAt(index);
                return tempObj;
            }
        }
        return tempObj;
    }

    //销毁多余地图元素
    IEnumerator DestroyElementList(List<GameObject> destroyList)
    {
        if (destroyList == null)
            yield break;
        for (int index = 0; index < destroyList.Count; index++)
        {
            GameObject destroyObj = destroyList[index];
            GameObject.Destroy(destroyObj);
            yield return null;
        }
        destroyList.Clear();
    }






























    //旧版实现
    //private int smallInterval = MapDefine.MAPITEMSIZE / 4;
    //private GameObject mapTileRoot;

    //private List<string> createMapTileViewList = new List<string>();
    //private List<string> clearMapTileViewList = new List<string>();

    //private Dictionary<string, MapTileView> activeMapTileViewDic = new Dictionary<string, MapTileView>();
    //private List<MapTileView> disableMapTileViewList = new List<MapTileView>();

    //private IEnumerator OnUpdateView;


    //public MapTileViewMgr()
    //{
    //    mapTileRoot = new GameObject("SceneMapMgr");
    //}

    //public void UpdateViewByPos(Vector3 pos, int row, int col)
    //{
    //    List<Vector2> posList = new List<Vector2>();

    //    int starX = Mathf.FloorToInt(pos.x - smallInterval);
    //    int starY = Mathf.FloorToInt(pos.z - smallInterval);
    //    int endX = Mathf.FloorToInt(pos.x + smallInterval);
    //    int endY = Mathf.FloorToInt(pos.z + smallInterval);

    //    int starX = Mathf.FloorToInt(pos.x - MapDefine.TilesGridInterval);
    //    int starY = Mathf.FloorToInt(pos.z - MapDefine.TilesGridInterval);
    //    int endX = Mathf.FloorToInt(pos.x + MapDefine.TilesGridInterval);
    //    int endY = Mathf.FloorToInt(pos.z + MapDefine.TilesGridInterval);

    //    for (int i = starX; i <= endX; i += MapDefine.TilesGridInterval)
    //    {
    //        if (i < 0) continue;
    //        for (int j = starY; j <= endY; j += MapDefine.TilesGridInterval)
    //        {
    //            if (j < 0) continue;
    //            posList.Add(new Vector2(i, j));
    //        }
    //    }




    //    createMapTileViewList.Clear();
    //    clearMapTileViewList.Clear();
    //    List<string> allData = new List<string>();
    //    for (int index = 0; index < posList.Count; index++)
    //    {
    //        int tempRow = Mathf.FloorToInt(posList[index].y / MapDefine.MAPITEMSIZE);
    //        int tempcol = Mathf.FloorToInt(posList[index].x / MapDefine.MAPITEMSIZE);
    //        string tempKey = tempRow + "_" + tempcol;
    //        if (!activeMapTileViewDic.ContainsKey(tempKey) && !createMapTileViewList.Contains(tempKey))
    //        {
    //            createMapTileViewList.Add(tempKey);
    //        }
    //        allData.Add(tempKey);
    //    }

    //    for (int index = 0; index < posList.Count; index++)
    //    {
    //        int parentRow = Mathf.FloorToInt(posList[index].x / MapDefine.FileGridInterval);
    //        int parentcol = Mathf.FloorToInt(posList[index].y / MapDefine.FileGridInterval);

    //        int tempRow = Mathf.FloorToInt(posList[index].x / MapDefine.TilesGridInterval);
    //        int tempcol = Mathf.FloorToInt(posList[index].y / MapDefine.TilesGridInterval);
    //        string fileKey = parentRow + "_" + parentcol;
    //        string gridKey = tempRow + "_" + tempcol;
    //        string tempKey = fileKey + "/" + gridKey;
    //        Debug.LogError("ALL>>>" + gridKey);
    //        allData.Add(gridKey);
    //        if (!activeMapTileViewDic.ContainsKey(gridKey) && !createMapTileViewList.Contains(tempKey))
    //        {
    //            createMapTileViewList.Add(tempKey);
    //            Debug.LogError("Add>>>" + tempKey);
    //        }
    //    }


    //    foreach (var item in activeMapTileViewDic)
    //    {
    //        if (!allData.Contains(item.Key))
    //        {
    //            clearMapTileViewList.Add(item.Key);
    //        }
    //    }
    //    if (createMapTileViewList.Count > 0 || clearMapTileViewList.Count > 0)
    //    {
    //        if (OnUpdateView == null)
    //        {
    //            OnUpdateView = OnUpdateViewHandler();
    //            ZTSceneManager.GetInstance().StartCoroutine(OnUpdateView);
    //        }
    //    }
    //    if (disableMapTileViewList.Count > 8)
    //    {
    //        List<MapTileView> tempDisableList = disableMapTileViewList.GetRange(0, disableMapTileViewList.Count / 2);
    //        disableMapTileViewList.RemoveRange(0, disableMapTileViewList.Count / 2);
    //        ZTSceneManager.GetInstance().StartCoroutine(DestroyElementList(tempDisableList));
    //    }
    //}

    //public MapTileView GetMapView(string key)
    //{
    //    MapTileView mapTileView = null;
    //    for (int index = 0; index < disableMapTileViewList.Count; index++)
    //    {
    //        mapTileView = disableMapTileViewList[index];
    //        if (mapTileView.MapKey == key)
    //        {
    //            disableMapTileViewList.RemoveAt(index);
    //            return mapTileView;
    //        }
    //    }
    //    return null;
    //}

    //IEnumerator DestroyElementList(List<MapTileView> clearList)
    //{
    //    if (clearList == null)
    //        yield break;
    //    for (int index = 0; index < clearList.Count; index++)
    //    {
    //        GameObject destroyObj = clearList[index].gameObject;
    //        GameObject.Destroy(destroyObj);
    //        yield return null;
    //    }
    //    clearList.Clear();
    //}

    //IEnumerator OnUpdateViewHandler()
    //{

    //    while (createMapTileViewList.Count > 0 || clearMapTileViewList.Count > 0)
    //    {
    //        Update();
    //        yield return null;
    //    }
    //    OnUpdateView = null;
    //}

    //private void Update()
    //{
    //    ClearTile();
    //    CreateTile();
    //}

    //private void ClearTile()
    //{
    //    if (clearMapTileViewList.Count <= 0) return;
    //    string name = clearMapTileViewList[0];
    //    clearMapTileViewList.RemoveAt(0);
    //    MapTileView view = null;
    //    if (activeMapTileViewDic.TryGetValue(name, out view))
    //    {
    //        view.gameObject.SetActive(false);
    //        if (view.IsLoad)
    //        {
    //            disableMapTileViewList.Add(view);
    //        }
    //        activeMapTileViewDic.Remove(name);
    //    }
    //}

    //public void CreateTile()
    //{
    //    if (createMapTileViewList.Count <= 0) return;
    //    string namePath = createMapTileViewList[0];
    //    string[] keys = namePath.Split('/');
    //    string[] data = keys[1].Split('_');
    //    createMapTileViewList.RemoveAt(0);
    //    string name = keys[1];

    //    if (data.Length != 2) return;
    //    int row = int.Parse(data[0]);
    //    int col = int.Parse(data[1]);
    //    MapTileView tempTileView = GetMapView(name);
    //    if (tempTileView != null)
    //    {
    //        tempTileView.gameObject.SetActive(true);
    //        activeMapTileViewDic[name] = tempTileView;
    //        Debug.LogError("Get  :" + name);


    //    }
    //    else
    //    {
    //        Debug.LogError("Load Begin:" + name);
    //        GameObject gameObject = new GameObject();
    //        gameObject.name = name;
    //        gameObject.transform.localPosition = new Vector3(col * MapDefine.TilesGridInterval, 0, row * MapDefine.TilesGridInterval);
    //        gameObject.transform.parent = mapTileRoot.transform;
    //        tempTileView = gameObject.AddComponent<MapTileView>();
    //        tempTileView.MapKey = name;
    //        activeMapTileViewDic[name] = tempTileView;
    //        string mapKey = string.Format("{0}_{1}", row, col);
    //        string assetPath = MapDefine.TERRAIN_PREFAB_PATH + namePath + ".prefab";
    //        AssetManager.LoadAsset(assetPath, (obj, str) =>
    //        {
    //            GameObject mapGo = obj as GameObject;
    //            if (activeMapTileViewDic.ContainsKey(name))
    //            {
    //                activeMapTileViewDic[name].SetTileObj(GameObject.Instantiate(mapGo));
    //            }
    //        });
    //    }
    //}
}
