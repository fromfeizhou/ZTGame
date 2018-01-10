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
       
        string path = MapDefine.MapTexturePath + string.Format("{0:D2}", _mapId) + ".png";
        AssetManager.LoadAsset(path, MapTextureCom);

    }

    private void MapTextureCom(Object target, string path)
    {
        Renderer render = gameObject.GetComponent<Renderer>();
        render.material.mainTexture =target as Texture;

    }
}
