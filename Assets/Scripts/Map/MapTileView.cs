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
        string loadTerrainPath = "MapItem/" + string.Format("Terrain_{0}_{1}", _mapTileData.Row, _mapTileData.Column);
        GameObject go = Resources.Load<GameObject>(loadTerrainPath);
        if (go == null)
            Debug.Log("loadTerrainPath:" + loadTerrainPath);

        Transform terrain = GameObject.Instantiate(go).transform;
        terrain.SetParent(transform);
        terrain.localPosition = Vector3.zero;
        terrain.localEulerAngles = Vector3.zero;
        terrain.localScale = Vector3.one;

        //string path = MapDefine.MapTexturePath + string.Format("{0:D2}", _mapId) + ".png";
        //AssetManager.LoadAsset(path, MapTextureCom);

    }

    private void MapTextureCom(Object target, string path)
    {
        Renderer render = gameObject.GetComponent<Renderer>();
        render.material.mainTexture =target as Texture;

    }
}
