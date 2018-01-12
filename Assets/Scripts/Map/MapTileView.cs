using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTileView : MonoBehaviour {

	private int _mapId;
    private MapTileData _mapTileData = null;
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
        string path = MapDefine.TERRAIN_PREFAB_PATH + string.Format("Terrain_{0}_{1}.prefab", _mapTileData.Row, _mapTileData.Column);
        AssetManager.LoadAsset(path, MapTextureCom);

    }

    private void MapTextureCom(Object target, string path)
    {

        GameObject go = target as GameObject;
        if (go == null)
        {
            Debug.Log("loadTerrainPath:" + path);
            return;
        }

        Transform terrain = GameObject.Instantiate(go).transform;
        terrain.SetParent(transform);
        terrain.localPosition = Vector3.zero;
        terrain.localEulerAngles = Vector3.zero;
        terrain.localScale = Vector3.one;
    }
}
