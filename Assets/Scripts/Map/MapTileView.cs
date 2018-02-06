using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTileView : MonoBehaviour {

	private int _mapId;
    private MapTileData _mapTileData = null;
    private GameObject _terrain = null;
    public bool isShow = false;

    public bool IsLoad
    {
        get
        {
            return _terrain != null;
        }
    }
    public void setMapData(MapTileData data)
    {
        if (data==_mapTileData&&IsLoad)
            return;
        _mapTileData = data;
        _mapId = _mapTileData.MapId;
        name = data.Row + "_" + data.Column;
        UpdateTileView();
    }

    public MapTileData GetMapData()
    {
        return _mapTileData;
    }

    private void UpdateTileView()
    {
        ClearTrrain();
        MapManager.GetInstance().SafeGetTrrainPrefab(_mapTileData.Row, _mapTileData.Column, go =>
        {
            if(_terrain == null)
                CreateTrrain(go);
            UpdateMapItem();
        });
    }

    private int _gridCnt = MapDefine.MAPITEMSIZE * 4;
    private List<MapBlockData> mapBlock;
    void OnDrawGizmos()
    {
        if (!isShow) return;
        if (mapBlock == null)
        {
            float minRow = _mapTileData.Row * _gridCnt;
            float maxRow = minRow + _gridCnt;
            float minCol = _mapTileData.Column * _gridCnt;
            float maxCol = minCol + _gridCnt;
            mapBlock = MapManager.GetInstance().GetMapBlock(minRow, maxRow, minCol, maxCol);
        }
        if (mapBlock.Count > 0)
        {
            for (int i = 0; i < mapBlock.Count; i++)
            {
                Vector3 pos =  new Vector3(mapBlock[i].row * 0.2f, 0, mapBlock[i].col * 0.2f);
                if (MapManager.GetInstance().GetFloorColl(pos) != eMapBlockType.None)
                {
                    Gizmos.DrawWireCube(pos + aaa * 0.5f, aaa);
                }
            }
        }
    }
    private Vector3 aaa = new Vector3(0.2f, 0.0f, 0.2f);
    private void UpdateMapItem()
    {
        Transform tempParent = null;
        _mapInfo = MapManager.GetInstance().GetMapInfiByPos(_mapTileData.Row, _mapTileData.Column);
        if (_mapInfo == null)
        {
            return;
        }

        for (int i = 0; i < _mapInfo.MapItemList.Count; i++)
        {
            string tempAssetPath = "";
            MapItemInfo tempItem = _mapInfo.MapItemList[i];
            //if (tempItem.MapItemType == eMapItemType.Tree){
            //    treeParent = _terrain.transform.Find("[Tree]");
            //    tempAssetPath = MapDefine.MAPITEM_TREE;
            //}
            //else if (tempItem.MapItemType == eMapItemType.Wall01)
            //{
            //    treeParent = _terrain.transform.Find("[Wall01]");
            //    tempAssetPath = MapDefine.MAPITEM_Wall01;
            //}
            string tempName=(tempItem.MapItemType ).ToString();
            tempParent = _terrain.transform.Find(string.Format("[{0}]", tempName));
            if (tempParent == null) continue;
            tempAssetPath = string.Format(MapDefine.MapItemPath, tempName);

            for (int j = 0; j < _mapInfo.MapItemList[i].MapItemInfoList.Count; j++)
            {
                AssetManager.LoadAsset(tempAssetPath, (obj, str) =>
                {
                    GameObject assetTree = obj as GameObject;
                    Transform tree = Instantiate(assetTree).transform;
                    tree.SetParent(tempParent);
                    tree.localPosition = _mapInfo.MapItemList[i].MapItemInfoList[j].Pos;
                    tree.localEulerAngles = _mapInfo.MapItemList[i].MapItemInfoList[j].Angle;
                    tree.localScale = _mapInfo.MapItemList[i].MapItemInfoList[j].Scale;
                    trees.Add(tree.gameObject);
                    BuildingZTCollider tempcollider = tree.GetComponent<BuildingZTCollider>();
                    if(tempcollider!=null){
                    ICharaBattle tempBattle=ZTSceneManager.GetInstance().GetCharaById(1) as ICharaBattle;
                    if (tempBattle != null)
                        tempcollider.SetTarget(tempBattle.Collider);
                    }
                        
                });
            }
        }
    }

    private MapInfo _mapInfo;


    private void CreateTrrain(GameObject go)
    {
        _terrain = GameObject.Instantiate(go);
        UpdateTrrain();
    }

    public bool IsNeedClear(int minRow,int maxRow,int minCol,int maxCol)
    {
        if (_mapTileData == null || !IsLoad ||  (_mapTileData.Row > maxRow || _mapTileData.Row < minRow || _mapTileData.Column > maxCol || _mapTileData.Column < minCol))
        {
            ClearTrrain();
            return true;
        }
        return false;
    }

    public void ClearTrrain()
    {
        if (null != _terrain)
        {
            GameObject.DestroyObject(_terrain);
            _terrain = null;
        }

        for(int i = 0;i< trees.Count;i++)
            GameObject.DestroyObject(trees[i]);
        trees.Clear();


    }

    private List<GameObject> trees = new List<GameObject>();

    private void MapTextureCom(Object target, string path)
    {
        GameObject go = target as GameObject;
        if (go == null)
        {
            Debug.Log("loadTerrainPath:" + path);
            return;
        }
        MapManager.GetInstance().AddTrrainPrefab(path, go);
        CreateTrrain(go);
    }

    private void UpdateTrrain()
    {
        _terrain.transform.SetParent(transform);
        _terrain.transform.localPosition = Vector3.zero;
        _terrain.transform.localEulerAngles = Vector3.zero;
        _terrain.transform.localScale = Vector3.one;
    }
}
