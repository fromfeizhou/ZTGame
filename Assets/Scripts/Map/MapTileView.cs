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
        UpdateTileView();
    }

    private void UpdateTileView()
    {
        ClearTrrain();

        string path = MapDefine.TERRAIN_PREFAB_PATH + string.Format("Terrain_{0}_{1}.prefab", _mapTileData.Row, _mapTileData.Column);
        GameObject go = MapManager.GetInstance().GetTrrainPrefab(path);
        if (null != go)
        {
           CreateTrrain(go);
        }
        else
        {
            AssetManager.LoadAsset(path, MapTextureCom);
        }
    }

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
    }

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
