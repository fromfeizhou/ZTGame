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
    public string mapKey;
    public List<MapItemInfo> MapItemList = new List<MapItemInfo>();
}


[CreateAssetMenu]
[System.Serializable]
public class MapAsset : ScriptableObject
{
    public List<MapInfo> MapList = new List<MapInfo>();
}
