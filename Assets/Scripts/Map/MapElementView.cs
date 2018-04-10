using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class MapElementView
{
    private MapTileViewMgr mapTileViewMgr;

    private MapAsset mapAsset;
    private Dictionary<string, Dictionary<string, MapElement>> AllMapElementDic =
        new Dictionary<string, Dictionary<string, MapElement>>();
    //预设网格资源列表
    private Dictionary<string, Dictionary<string, MapElementGrid>> AllMapElementGridDic =
        new Dictionary<string, Dictionary<string, MapElementGrid>>();
    //视野内预设列表
    private Dictionary<string, MapElement> visionElementDic = new Dictionary<string, MapElement>();
    private List<string> createElementList = new List<string>();    //创建列表
    private List<string> disableElementList = new List<string>();      //移除列表
    private Dictionary<string, GameObject> activeObj = new Dictionary<string, GameObject>();//激活的elementObj
    private List<GameObject> disabelObj = new List<GameObject>();//Element隐藏列表
    private GameObject mapRoot;
    private GameObject elementRoot;

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
        elementRoot = new GameObject("MapElement");
        mapTileViewMgr=new MapTileViewMgr();

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


    private int smallInterval = MapDefine.MAPITEMSIZE / 4;
    public void UpdateTerrainView(Vector3 pos, int row, int col)
    {
        mapTileViewMgr.UpdateViewByPos(pos, row, col);
    }
   
    //射线检测（建筑屋顶逻辑）
    public void UpdateRoleRay(Vector3 pos)
    {

        Vector3 targetPos = pos + 100 * Vector3.up;
        Vector3 temp = pos - targetPos;
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

    private void SetRayObjEnabel(Transform obj, bool isShow)
    {
        if (obj == null) return;
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
        var needClearElementDic = activeObj.Keys.Except(elementDic.Keys);
        var needLoadElementDic = elementDic.Keys.Except(activeObj.Keys);
        createElementList.Clear();
        disableElementList.Clear();
        foreach (var key in needLoadElementDic)
            createElementList.Add(key);

        foreach (var key in needClearElementDic)
            disableElementList.Add(key);

        visionElementDic = elementDic;
       // if (disableElementList.Count > 0)
        //    ClearElementInList();
        if (createElementList.Count > 0|| disableElementList.Count > 0)
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

    IEnumerator OnUpdateElement()
    {
        while (createElementList.Count > 0 || disableElementList.Count > 0)
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

    public void ClearElementInList()
    {
        if (disableElementList == null || disableElementList.Count == 0)
        {
            return;
        }
        string key = disableElementList[0];
        disableElementList.RemoveAt(0);
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
        activeObj[elementData.elementKey] = null;

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
            string elementAssetPath = string.Format(MapDefine.MapElementPath, elementData.elementType);
            AssetManager.LoadAsset(elementAssetPath, (obj, str) =>
            {
                if (obj != null)//&& visionElementDic.ContainsKey(key))
                {
                    if (activeObj.ContainsKey(elementData.elementKey))
                    {
                        GameObject assetTree = obj as GameObject;
                        Transform element = GameObject.Instantiate(assetTree).transform;
                        element.SetParent(elementRoot.transform);
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

}
