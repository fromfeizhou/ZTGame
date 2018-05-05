using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapElementLoaderData
{
    public Transform root;
    public int disableMaxNum;
    public int cacheNum;
}

public class MapElementLoader
{
    private MapElementLoaderData data;

    private PiecewiseLoader elementLoader = new PiecewiseLoader(1f);
    private PiecewiseLoader elementUnLoader = new PiecewiseLoader(1f);

    private Dictionary<string, MapElement> visionElementDic = new Dictionary<string, MapElement>();
    private List<string> createElementList = new List<string>();    //创建列表
    private List<string> disableElementList = new List<string>();      //移除列表
    private Dictionary<string, GameObject> activeObj = new Dictionary<string, GameObject>();//激活的elementObj
    private List<GameObject> cacheObjs = new List<GameObject>();//二级缓存 不影藏，一般为高复用元素
    private List<GameObject> disabelObj = new List<GameObject>();//Element隐藏列表
    private List<GameObject> disabelCacheObjs =new List<GameObject>();

    private IEnumerator _OnUpdateElementHandler;


    private bool IsNeedUpdate
    {
        get { return createElementList.Count > 0 || disableElementList.Count > 0; }
    }

    public MapElementLoader(MapElementLoaderData data)
    {
        if (data == null)
            Debug.LogError("MapElementLoaderData is null!!!");
        this.data = data;
    }

    public void LoadElement(Dictionary<string, MapElement> elementDic)
    {
        if (elementDic == null) return;
        visionElementDic = elementDic;

        var needClearElementDic = activeObj.Keys.Except(elementDic.Keys);
        var needLoadElementDic = elementDic.Keys.Except(activeObj.Keys);
        createElementList.Clear();
        disableElementList.Clear();
        foreach (var key in needLoadElementDic)
            createElementList.Add(key);

        foreach (var key in needClearElementDic)
            disableElementList.Add(key);

        visionElementDic = elementDic;

        ClearElementInList();
        CreateElementInList();
        //if (IsNeedUpdate)
        //{
        //    if (null == _OnUpdateElementHandler)
        //    {
        //        _OnUpdateElementHandler = OnUpdateElement();
        //        ZTSceneManager.GetInstance().StartCoroutine(_OnUpdateElementHandler);
        //    }
        //}
        if (cacheObjs.Count > data.cacheNum)
        {
            int clearNum = cacheObjs.Count - (int) (data.cacheNum * 0.5);
            disabelCacheObjs.AddRange(cacheObjs.GetRange(0, clearNum));
            cacheObjs.RemoveRange(0, clearNum);
        }
        if (disabelObj.Count > data.disableMaxNum)
        {
            int clearNum = disabelObj.Count - (int)(data.disableMaxNum * 0.5);
            List<GameObject> tempDisableList = disabelObj.GetRange(0, clearNum);
            disabelObj.RemoveRange(0, clearNum);
            DestroyElementList(tempDisableList);
        }
    }

    //IEnumerator OnUpdateElement()
    //{
    //    while (IsNeedUpdate)
    //    {
    //        Update();
    //        yield return null;
    //    }
    //    _OnUpdateElementHandler = null;
    //}

    //private void Update()
    //{
    //    ClearElementInList(); //active转disable
    //    CreateElementInList();
    //}

    //清理地图元素（放入未激活列表）
    public void ClearElementInList()
    {
        while(disabelCacheObjs.Count > 0)
        {
            GameObject tempObj = disabelCacheObjs[0];
            disabelCacheObjs.RemoveAt(0);
            tempObj.SetActive(false);
            disabelObj.Add(tempObj);
            Debug.LogError("cacheObj" + tempObj.name);
        }
        while ( disableElementList.Count > 0)
        {
            string key = disableElementList[0];
            disableElementList.RemoveAt(0);
            if (activeObj.ContainsKey(key))
            {
                GameObject tempObj = activeObj[key];
                activeObj.Remove(key);
                if (tempObj == null) return;
                if (data.cacheNum > 0)
                {
                    cacheObjs.Add(tempObj);
                }
                else
                {
                    tempObj.SetActive(false);
                    disabelObj.Add(tempObj);
                }

            }
        }
       
    }

    public void CreateElementInList()
    {
        while (createElementList.Count>0)
        {
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
                    if (!visionElementDic.ContainsKey(elementData.elementKey))
                    {
                        activeObj.Remove(elementData.elementKey);
                        return false;
                    }
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
                        if (obj != null) //&& visionElementDic.ContainsKey(key))
                        {
                            if (activeObj.ContainsKey(elementData.elementKey))
                            {
                                if (!visionElementDic.ContainsKey(elementData.elementKey))
                                    return false;
                                GameObject assetTree = obj as GameObject;
                                Transform element = GameObject.Instantiate(assetTree).transform;
                                element.SetParent(data.root);
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
    private void DestroyElementList(List<GameObject> destroyList)
    {
        if (destroyList == null)
            return;
        for (int index = 0; index < destroyList.Count; index++)
        {
            GameObject destroyObj = destroyList[index];
            elementUnLoader.PushCallBack(() =>
            {
                if (destroyObj == null) return false;
                GameObject.Destroy(destroyObj);
                return true;
            });
        }
        destroyList.Clear();
    }

}
