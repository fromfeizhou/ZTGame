using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTileViewMgr
{
    private int smallInterval = MapDefine.MAPITEMSIZE / 4;
    private GameObject mapTileRoot;

    private List<string> createMapTileViewList = new List<string>();
    private List<string> clearMapTileViewList = new List<string>();

    private Dictionary<string, MapTileView> activeMapTileViewDic = new Dictionary<string, MapTileView>();
    private List<MapTileView> disableMapTileViewList = new List<MapTileView>();

    private IEnumerator OnUpdateView;


    public MapTileViewMgr()
    {
        mapTileRoot = new GameObject("SceneMapMgr");
    }

    public void UpdateViewByPos(Vector3 pos, int row, int col)
    {
        List<Vector2> posList = new List<Vector2>();

        int starX = Mathf.FloorToInt(pos.x - smallInterval);
        int starY = Mathf.FloorToInt(pos.z - smallInterval);
        int endX = Mathf.FloorToInt(pos.x + smallInterval);
        int endY = Mathf.FloorToInt(pos.z + smallInterval);

        for (int i = starX; i <= endX; i += smallInterval)
        {
            if (i < 0) continue;
            for (int j = starY; j <= endY; j += smallInterval)
            {
                if (j < 0) continue;
                posList.Add(new Vector2(i, j));
            }
        }
        createMapTileViewList.Clear();
        clearMapTileViewList.Clear();
        List<string> allData = new List<string>();
        for (int index = 0; index < posList.Count; index++)
        {
            int tempRow = Mathf.FloorToInt(posList[index].y / MapDefine.MAPITEMSIZE);
            int tempcol = Mathf.FloorToInt(posList[index].x / MapDefine.MAPITEMSIZE);
            string tempKey = tempRow + "_" + tempcol;
            if (!activeMapTileViewDic.ContainsKey(tempKey) && !createMapTileViewList.Contains(tempKey))
            {
                createMapTileViewList.Add(tempKey);
            }
            allData.Add(tempKey);


        }
        foreach (var item in activeMapTileViewDic)
        {
            if (!allData.Contains(item.Key))
            {
                clearMapTileViewList.Add(item.Key);
            }
        }
        if (createMapTileViewList.Count > 0||clearMapTileViewList.Count>0)
        {
            if (OnUpdateView == null)
            {
                OnUpdateView = OnUpdateViewHandler();
                ZTSceneManager.GetInstance().StartCoroutine(OnUpdateView);
            }
        }
        if (disableMapTileViewList.Count > 8)
        {
            List<MapTileView> tempDisableList = disableMapTileViewList.GetRange(0, disableMapTileViewList.Count / 2);
            disableMapTileViewList.RemoveRange(0, disableMapTileViewList.Count / 2);
            ZTSceneManager.GetInstance().StartCoroutine(DestroyElementList(tempDisableList));
        }
    }

    public MapTileView GetMapView(string key)
    {
        MapTileView mapTileView = null;
        for (int index = 0; index < disableMapTileViewList.Count; index++)
        {
            mapTileView = disableMapTileViewList[index];
            if (mapTileView.MapKey == key)
            {
                disableMapTileViewList.RemoveAt(index);
                return mapTileView;
            }
        }
        return null;
    }

    IEnumerator DestroyElementList(List<MapTileView> clearList)
    {
        if (clearList == null)
            yield break;
        for (int index = 0; index < clearList.Count; index++)
        {
            GameObject destroyObj = clearList[index].gameObject;
            GameObject.Destroy(destroyObj);
            yield return null;
        }
        clearList.Clear();
    }

    IEnumerator OnUpdateViewHandler()
    {

        while (createMapTileViewList.Count > 0 || clearMapTileViewList.Count > 0)
        {
            Update();
            yield return null;
        }
        OnUpdateView = null;
    }

    private void Update()
    {
        ClearTile();
        CreateTile();
    }

    private void ClearTile()
    {
        if (clearMapTileViewList.Count <= 0) return;
        string name = clearMapTileViewList[0];
        clearMapTileViewList.RemoveAt(0);
        MapTileView view = null;
        if (activeMapTileViewDic.TryGetValue(name, out view))
        {
            view.gameObject.SetActive(false);
            if (view.IsLoad)
            {
                disableMapTileViewList.Add(view);
            }
            activeMapTileViewDic.Remove(name);
        }
    }

    public void CreateTile()
    {
        if (createMapTileViewList.Count <= 0) return;
        string name = createMapTileViewList[0];
        string[] data = name.Split('_');
        createMapTileViewList.RemoveAt(0);

        if (data.Length != 2) return;
        int row = int.Parse(data[0]);
        int col = int.Parse(data[1]);
        MapTileView tempTileView = GetMapView(name);
        if (tempTileView != null)
        {
            tempTileView.gameObject.SetActive(true);
            activeMapTileViewDic[name] = tempTileView;
            Debug.LogError("Get  :" + name);


        }
        else
        {
            //Debug.LogError("Load Begin:" + name);
            GameObject gameObject = new GameObject();
            gameObject.name = name;
            gameObject.transform.localPosition = new Vector3(col * MapDefine.MapWidth, 0, row * MapDefine.MapHeight);
            gameObject.transform.parent = mapTileRoot.transform;
            tempTileView = gameObject.AddComponent<MapTileView>();
            tempTileView.MapKey = name;
            activeMapTileViewDic[name] = tempTileView;
            string mapKey = string.Format("{0}_{1}", row, col);
            string assetPath = MapDefine.TERRAIN_PREFAB_PATH + mapKey + ".prefab";
            AssetManager.LoadAsset(assetPath, (obj, str) =>
            {
                GameObject mapGo = obj as GameObject;
                if (activeMapTileViewDic.ContainsKey(name))
                {
                    activeMapTileViewDic[name].SetTileObj(GameObject.Instantiate(mapGo));
                }
            });
        }
    }
}
