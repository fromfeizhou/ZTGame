using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;


public class MapBlockLoadData
{
    public Transform root;
    public MapAsset mapAsset;
    public int disableMaxNum;
    public int interval;
    public bool IsCache;
    public int cacheNum;
    public Vector3 pos;
}

public class MapBlockLoader
{
    private static int index;
    private PiecewiseLoader elementLoader = new PiecewiseLoader(1f);
    private PiecewiseLoader elementUnLoader = new PiecewiseLoader(1f);

    private MapAsset mapAsset;
    private Dictionary<string, Dictionary<string, MapElement>> AllMapElementDic =
        new Dictionary<string, Dictionary<string, MapElement>>();
    //预设网格资源列表
    private Dictionary<string, Dictionary<string, MapElementGrid>> AllElemtGridDic =
        new Dictionary<string, Dictionary<string, MapElementGrid>>();
    //视野内预设列表
    private Dictionary<string, MapElement> visionElementDic = new Dictionary<string, MapElement>();
    private List<string> createElementList = new List<string>();    //创建列表
    private List<string> disableElementList = new List<string>();      //移除列表
    private Dictionary<string, GameObject> activeObj = new Dictionary<string, GameObject>();//激活的elementObj
    private List<GameObject> cacheObjs= new List<GameObject>();//二级缓存 不影藏，一般为高复用元素
    private List<GameObject> disabelObj = new List<GameObject>();//Element隐藏列表

    private Transform elementRoot;
    private int cacheNum;
    private int buffNum;
    private int interval;
    private IEnumerator _OnUpdateElementHandler;
    private Vector2 curGrid = new Vector2(-1, -1);

    private string mapKey;
    private bool IsNeedUpdate
    {
        get { return createElementList.Count > 0 || disableElementList.Count > 0; }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data">配置文件</param>
    /// <param interval="interval">九宫格大小</param>
    public MapBlockLoader(MapBlockLoadData data)
    {
        if (data != null)
        {
            elementRoot = data.root;
            mapAsset = data.mapAsset;
            interval = data.interval;
            buffNum = data.disableMaxNum;
            cacheNum = data.cacheNum;
            InitMapAsset(data.pos);
        }
        else
        {
            Debug.LogError("MapData is Error!!!");
        }
    }

    private void InitMapAsset(Vector3 pos)
    {
        mapKey = (int)pos.x / MapDefine.MAPITEMTOTALSIZE + "" + (int)pos.z / MapDefine.MAPITEMTOTALSIZE;
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
            AllElemtGridDic[mapKey] = tempElementGridDic;
        }
        else
        {
            Debug.LogError("MapAsset is null!!");
        }
    }

    public void SetBlockPos(Vector3 pos)
    {
       
        int gridX = Mathf.FloorToInt(pos.x / interval);
        int gridY = Mathf.FloorToInt(pos.z / interval);


        if (Mathf.Abs(curGrid.x - gridX) >= 1 || Mathf.Abs(curGrid.y - gridY) >= 1)
        {
            curGrid.x = gridX;
            curGrid.y = gridY;

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
                    if (AllElemtGridDic.TryGetValue(bigMapKey, out tempGridDataDic))
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

            if (IsNeedUpdate)
            {
                if (null == _OnUpdateElementHandler)
                {
                    _OnUpdateElementHandler = OnUpdateElement();
                    ZTSceneManager.GetInstance().StartCoroutine(_OnUpdateElementHandler);
                }
            }
            if (disabelObj.Count > buffNum)
            {
                List<GameObject> tempDisableList = disabelObj.GetRange(0, disabelObj.Count / 2);
                disabelObj.RemoveRange(0, disabelObj.Count / 2);
                ZTSceneManager.GetInstance().StartCoroutine(DestroyElementList(tempDisableList));
            }
        }
    }

    IEnumerator OnUpdateElement()
    {
        while (IsNeedUpdate)
        {
            Update();
            yield return null;
        }
        _OnUpdateElementHandler = null;
    }

    public void Update()
    {
        ClearElementInList(); //active转disable
        CreateElementInList();
    }

    //清理地图元素（放入未激活列表）
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
            activeObj.Remove(key);
            if (tempObj==null) return;
            if ( cacheNum>0)
            {
                cacheObjs.Add(tempObj);
                if (cacheObjs.Count > cacheNum)
                {
                    tempObj = cacheObjs[0];
                    cacheObjs.RemoveAt(0);
                }
            }          
            else
            {
                
                elementUnLoader.PushCallBack(() =>
                {
                    if (tempObj != null)
                    {
                        tempObj.SetActive(false);
                        disabelObj.Add(tempObj);
                        return true;
                    }
                    return false;
                });
            }
            
        }
    }

    public void CreateElementInList()
    {
        if (createElementList == null || createElementList.Count == 0)
            return;
        string key = createElementList[0];
        createElementList.RemoveAt(0);
        MapElement elementData = visionElementDic[key];
        MapElementInfo elementInfo = elementData.elementInfo;
        activeObj[elementData.elementKey] = null;

        GameObject tempObj = GetObjByCache(elementData.elementType);
        if (tempObj != null)
        {
            if (tempObj.activeSelf)
            {
                SetElementTransform(tempObj.transform, elementInfo);
                activeObj[elementData.elementKey] = tempObj;
                return;
            }
            elementLoader.PushCallBack(() =>
            {
                if (tempObj == null) return false;
                SetElementTransform(tempObj.transform, elementInfo);
                tempObj.SetActive(true);
                activeObj[elementData.elementKey] = tempObj;
                return true;
            });
        }
        else
        {
            string elementAssetPath = string.Format(MapDefine.MapElementPath, elementData.elementType);
            AssetManager.LoadAsset(elementAssetPath, (obj, str) =>
            {
                elementLoader.PushCallBack(() =>
                {
                    if (obj != null)//&& visionElementDic.ContainsKey(key))
                    {
                        if (activeObj.ContainsKey(elementData.elementKey))
                        {
                            GameObject assetTree = obj as GameObject;
                            Transform element = GameObject.Instantiate(assetTree).transform;
                            element.SetParent(elementRoot);
                            SetElementTransform(element, elementInfo);
                            activeObj[elementData.elementKey] = element.gameObject;
                            return true;
                        }
                        return false;
                    }
                    return false;
                });
            });
        }
    }

    private void SetElementTransform(Transform element, MapElementInfo info)
    {
        element.position = info.Pos;
        element.eulerAngles = info.Angle;
        element.localScale = info.Scale;
    }

    //从隐藏列表获取
    private GameObject GetObjByCache(string elementType)
    {
        GameObject tempObj = null;
        string key = elementType + "(Clone)";
        for (int index = 0; index < cacheObjs.Count; index++)
        {
            if (cacheObjs[index].name.Equals(key))
            {
                tempObj = cacheObjs[index];
                cacheObjs.RemoveAt(index);
                return tempObj;
            }
        }
        for (int index = 0; index < disabelObj.Count; index++)
        {
            if (disabelObj[index].name.Equals(key))
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
            elementUnLoader.PushCallBack(() =>
            {
                if (destroyObj == null) return false;
                GameObject.Destroy(destroyObj);
                return true;
            });
            yield return null;
        }
        destroyList.Clear();
    }



}