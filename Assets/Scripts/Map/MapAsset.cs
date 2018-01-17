using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum eMapItemType
{
    Tree,
}

[System.Serializable]
public class MapItemInfoBase
{
    public Vector3 Pos;
    public Vector3 Angle;
    public Vector3 Scale;
}

[System.Serializable]
public class MapItemInfo
{
    public eMapItemType MapItemType;
    public List<MapItemInfoBase> MapItemInfoList = new List<MapItemInfoBase>();
}

[System.Serializable]
public class MapInfo
{
    public string MapKey;
    public List<MapItemInfo> MapItemList = new List<MapItemInfo>();
}


[CreateAssetMenu]
[System.Serializable]
public class MapAsset : ScriptableObject
{
    public List<MapInfo> MapList = new List<MapInfo>();
    public void AddMapItem<T>(string mapKey, eMapItemType mapType, T mapItem) where T: MapItemInfoBase
    {
        MapInfo mapInfo;
        int indexMapInfo = MapList.FindIndex(a => a.MapKey.Equals(mapKey));
        if (indexMapInfo < 0)
        {
            mapInfo = new MapInfo { MapKey = mapKey};
            MapList.Add(mapInfo);
        }
        else
        {
            mapInfo = MapList[indexMapInfo];
        }

        MapItemInfo mapItemInfo;
        int indexMapItemInfo = mapInfo.MapItemList.FindIndex(a => a.MapItemType == eMapItemType.Tree);
        if (indexMapItemInfo < 0)
        {
            mapItemInfo = new MapItemInfo() { MapItemType = eMapItemType.Tree };
            mapInfo.MapItemList.Add(mapItemInfo);
        }
        else
        {
            mapItemInfo = mapInfo.MapItemList[indexMapItemInfo];
        }

        mapItemInfo.MapItemInfoList.Add(mapItem);
    }
}
