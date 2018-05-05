using System;
using System.Collections.Generic;
using UnityEngine;

public class MapElementViewMgr
{

    private MapAsset mapAsset;
    //配置文件
    private Dictionary<string, Dictionary<string, MapElement>> AllMapElementDic =
        new Dictionary<string, Dictionary<string, MapElement>>();
    //大地图元素数据
    private Dictionary<string, Dictionary<string, MapElementGrid>> bigElementGridDic =
        new Dictionary<string, Dictionary<string, MapElementGrid>>();

    private Dictionary<string, MapElement> curBigGridElementDic=new Dictionary<string, MapElement>();

    //小地图元素数据
    private Dictionary<string, Dictionary<string, MapElementGrid>> littleElementGridDic =
        new Dictionary<string, Dictionary<string, MapElementGrid>>();
    private Dictionary<string, MapElement> curLittleGridElementDic = new Dictionary<string, MapElement>();

    private string mapKey;
    private Vector2 curBigGrid = new Vector2(-1f, -1f);
    private Vector2 curLittleGrid = new Vector2(-1f, -1f);

    private MapElementView bigElementView;
    private MapElementView littleElementView;
    private GameObject elementRoot;
    private GameObject littleElementRoot;
    private Transform lastRayObj;
    private Transform rayObj;

    public MapElementViewMgr()
    {
        elementRoot = new GameObject("MapElement");
        bigElementView = new MapElementView(new MapElementLoaderData()
        {
            disableMaxNum = 30,
            root = elementRoot.transform,
        });
        littleElementRoot=new GameObject("LittleMapElement");
        littleElementView = new MapElementView(new MapElementLoaderData()
        {
            disableMaxNum = 30,
            root = littleElementRoot.transform,
            cacheNum = 100,
        });
    }

    public void InitData(MapAsset mapData,Vector3 pos)
    {
        if (mapData == null)
        {
            Debug.LogError("MapAsset is null!!");
        }
        mapAsset = mapData;
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
            bigElementGridDic[mapKey] = tempElementGridDic;
        }
        else
        {
            Debug.LogError("MapAsset is null!!");
        }
    }

    public void SetPos(Vector3 pos)
    {
        UpdateMapAsset(pos);
        UpdateRoleRay(pos);
        UpdateBigGridData(pos);
        UpdateLittleGridData(pos);
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
    //处理被射线照射的go
    private void SetRayObjEnabel(Transform obj, bool isShow)
    {
        if (obj == null) return;
        for (int index = 0; index < obj.childCount; index++)
        {
            obj.GetChild(index).gameObject.SetActive(isShow);
        }

    }



    //更新配置数据
    private void UpdateMapAsset(Vector3 pos)
    {
        
    }

    private void UpdateBigGridData(Vector3 pos)
    {
        int gridX = Mathf.FloorToInt(pos.x / MapDefine.MapElementSize);
        int gridY = Mathf.FloorToInt(pos.z / MapDefine.MapElementSize);
        if (Mathf.Abs(curBigGrid.x - gridX) >= 1 || Mathf.Abs(curBigGrid.y - gridY) >= 1)
        {
            curBigGrid.x = gridX;
            curBigGrid.y = gridY;
            curBigGridElementDic.Clear();
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
                    if (bigElementGridDic.TryGetValue(bigMapKey, out tempGridDataDic))
                    {
                        MapElementGrid tempGridData;
                        if (tempGridDataDic.TryGetValue(gridKey, out tempGridData))
                            ParseMapElementGrid(tempGridData, bigMapKey);
                    }
                }
            }
            bigElementView.LoaderElement(curBigGridElementDic);

        }
    }

    private List<string> ParsedGrid = new List<string>();//已解析grid
    //获取大地图元素网格数据并生成对应小地图元素
    private void ParseMapElementGrid(MapElementGrid gridData, string bigMapKey)
    {
        bool isNeedParse = !ParsedGrid.Contains(gridData.gridKey);
        if(isNeedParse)
            ParsedGrid.Add(gridData.gridKey);
        List<string> tempElementKeyList = gridData.elementKeyList;
        for (int index = 0; index < tempElementKeyList.Count; index++)
        {
            MapElement element;
            Dictionary<string, MapElement> tempElementDic;
            if (AllMapElementDic.TryGetValue(bigMapKey, out tempElementDic))
            {
                if (tempElementDic.TryGetValue(tempElementKeyList[index], out element))
                {
                    bool isLittleElement = element.elementType.Contains("_Little_");
                    if (!isLittleElement&&!curBigGridElementDic.ContainsKey(element.elementKey))
                        curBigGridElementDic[element.elementKey] = element;
                    if (isLittleElement&&isNeedParse)
                    {
                        if(!littleElementGridDic.ContainsKey(bigMapKey))//小地图元素数据根据大地图元素数据生成
                            littleElementGridDic[bigMapKey] =new Dictionary<string, MapElementGrid>();
                        string littleGridKey = Mathf.FloorToInt(element.elementInfo.Pos.x / MapDefine.MapLittleElementSize) + "_" + Mathf.FloorToInt(element.elementInfo.Pos.z / MapDefine.MapLittleElementSize);
                        if (!littleElementGridDic[bigMapKey].ContainsKey(littleGridKey))
                        {
                            MapElementGrid tempGrid=new MapElementGrid();
                            tempGrid.gridKey = littleGridKey;
                            littleElementGridDic[bigMapKey][littleGridKey] = tempGrid;
                        }
                        littleElementGridDic[bigMapKey][littleGridKey].elementKeyList.Add(tempElementKeyList[index]);
                    }
                }
            }
        }
    }

    private void UpdateLittleGridData(Vector3 pos)
    {
        int gridX = Mathf.FloorToInt(pos.x / MapDefine.MapLittleElementSize);
        int gridY = Mathf.FloorToInt(pos.z / MapDefine.MapLittleElementSize);
        if (Mathf.Abs(curLittleGrid.x - gridX) >= 1 || Mathf.Abs(curLittleGrid.y - gridY) >= 1)
        {
            curLittleGrid.x = gridX;
            curLittleGrid.y = gridY;
            curLittleGridElementDic.Clear();
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
                    if (littleElementGridDic.TryGetValue(bigMapKey, out tempGridDataDic))
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
                                        if (!curLittleGridElementDic.ContainsKey(element.elementKey))
                                            curLittleGridElementDic[element.elementKey] = element;
                                    }
                            }
                        }
                    }
                }
            }
            littleElementView.LoaderElement(curLittleGridElementDic);
        }
    }

}