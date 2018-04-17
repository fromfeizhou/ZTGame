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



    private void UpdateTrrain()
    {
        _terrain.transform.SetParent(transform);
       // _terrain.transform.localPosition = Vector3.zero;
       // _terrain.transform.localEulerAngles = Vector3.zero;
      //  _terrain.transform.localScale = Vector3.one;
        _terrain.AddComponent<MeshCollider>();
    }

    private string mapKey;
    public string MapKey
    {
        get { return mapKey; }
        set { mapKey = value; }
    }

    public void SetTileObj(GameObject go)
    {
        _terrain = go;
        _terrain.layer = LayerMask.NameToLayer("Terrain");
        UpdateTrrain();
    }


}
