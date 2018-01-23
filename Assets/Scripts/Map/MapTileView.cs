using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTileView : MonoBehaviour {

	private int _mapId;
    private MapTileData _mapTileData = null;
    private GameObject _terrain = null;
    public void setMapData(MapTileData data)
    {
        if (_mapId == data.MapId)
            return;
        _mapTileData = data;
        _mapId = _mapTileData.MapId;
        name = data.Row + "_" + data.Column;
        UpdateTileView();
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
    private Vector3 baseSize = new Vector3(0.25f,0,0.25f);
    private List<MapBlockData> mapBlock;
    void OnDrawGizmos()
    {
        if (mapBlock == null)
        {
            float minRow = _mapTileData.Row * _gridCnt;
            float maxRow = minRow + _gridCnt;
            float minCol = _mapTileData.Column;
            float maxCol = minCol+ _gridCnt * _gridCnt;
            mapBlock = MapManager.GetInstance().GetMapBlock(minRow, maxRow, minCol, maxCol);
        }
        if (mapBlock.Count > 0)
        {
            for (int i = 0; i < mapBlock.Count; i++)
            {
                Vector3 pos = transform.position + new Vector3(mapBlock[i].row * 0.25f, 0, mapBlock[i].col * 0.25f);
                if (MapManager.GetInstance().GetFloorColl(pos) != eMapBlockType.None)
                {
                    Gizmos.DrawWireCube(pos, baseSize);
                }
            }
        }
    }

    private void UpdateMapItem()
    {
        Transform treeParent = _terrain.transform.Find("[Tree]");
        _mapInfo = MapManager.GetInstance().GetMapInfiByPos(_mapTileData.Row, _mapTileData.Column);
        if (_mapInfo == null)
        {
            return;
        }

        for (int i = 0; i < _mapInfo.MapItemList.Count; i++)
        {
            for (int j = 0; j < _mapInfo.MapItemList[i].MapItemInfoList.Count; j++)
            {
                AssetManager.LoadAsset(MapDefine.MAPITEM_TREE, (obj, str) =>
                {
                    GameObject assetTree = obj as GameObject;
                    Transform tree = Instantiate(assetTree).transform;
                    tree.SetParent(treeParent);
                    tree.localPosition = _mapInfo.MapItemList[i].MapItemInfoList[j].Pos;
                    tree.localEulerAngles = _mapInfo.MapItemList[i].MapItemInfoList[j].Angle;
                    tree.localScale = _mapInfo.MapItemList[i].MapItemInfoList[j].Scale;
                    trees.Add(tree.gameObject);
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

    private void ClearTrrain()
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
