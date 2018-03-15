using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum eMapItemType
{
    Tree=1,
    Tree08,
    Wall01,
    Wall02,
    Wall03,
    JiTan01,
    JiTan02,
    RockGroup01,
    RockGroup02,
    Cao,
    Cao2,
    JianYu,
    JianZhu01,
    JianZhu02,
    JianZhu03
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

[System.Serializable]
public class MapElementGrid
{
    public string gridKey;
    public List<string> elementKeyList = new List<string>();
}

[System.Serializable]
public class MapElement
{
    public string elementKey;
    public string elementType;
    public MapElementInfo elementInfo;
}

[System.Serializable]
public class MapElementInfo
{
    public Vector3 Pos;
    public Vector3 Angle;
    public Vector3 Scale;
}


[CreateAssetMenu]
[System.Serializable]
public class MapAsset : ScriptableObject
{
    public List<MapInfo> MapList = new List<MapInfo>();

    public List<MapElement> elementList = new List<MapElement>();

    public List<MapElementGrid> ElementGrids = new List<MapElementGrid>();

    public void AddMapElement( MapElement data)
    {
        if (data == null) return;
        int index = elementList.FindIndex(a => a.elementKey.Equals(data.elementKey));
        if (index < 0)
            elementList.Add(data);
    }

    public void AddMapElementGridItem(string gridKey, string elementKey)
    {
        int index = ElementGrids.FindIndex(a => a.gridKey.Equals(gridKey));
        if (index < 0)
        {
            MapElementGrid grid = new MapElementGrid();
            grid.gridKey = gridKey;
            grid.elementKeyList.Add(elementKey);
            ElementGrids.Add(grid);
        }
        else
        {
            List<string> elementKeys = ElementGrids[index].elementKeyList;
            int tempIndex = elementKeys.FindIndex(a => a.Equals(elementKey));
            if (tempIndex < 0)
                elementKeys.Add(elementKey);

        }
    }

    //public void AddMapItem<T>(string mapKey, eMapItemType mapType, T mapItem) where T: MapItemInfoBase
    //{
    //    MapInfo mapInfo;
    //    int indexMapInfo = MapList.FindIndex(a => a.MapKey.Equals(mapKey));
    //    if (indexMapInfo < 0)
    //    {
    //        mapInfo = new MapInfo { MapKey = mapKey};
    //        MapList.Add(mapInfo);
    //    }
    //    else
    //    {
    //        mapInfo = MapList[indexMapInfo];
    //    }

    //    MapItemInfo mapItemInfo;
    //    int indexMapItemInfo = mapInfo.MapItemList.FindIndex(a => a.MapItemType == mapType);
    //    if (indexMapItemInfo < 0)
    //    {
    //        mapItemInfo = new MapItemInfo() { MapItemType = mapType };
    //        mapInfo.MapItemList.Add(mapItemInfo);
    //    }
    //    else
    //    {
    //        mapItemInfo = mapInfo.MapItemList[indexMapItemInfo];
    //    }

    //    mapItemInfo.MapItemInfoList.Add(mapItem);
    //}
}
