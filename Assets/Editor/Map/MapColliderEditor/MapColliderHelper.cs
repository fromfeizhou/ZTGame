using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class MapColliderHelper
{
    public enum eMapEditHelper
    {
        None,
        SaveMapBlockFile,
    }

    public static void SaveMapBlockFile(List<MapBlockData> _mapBlockData)
    {
        if (_mapBlockData != null)
        {
            string mapData = string.Empty;
            for (int i = 0; i < _mapBlockData.Count; i++)
            {
                if (_mapBlockData[i].type == eMapBlockType.None)
                    continue;

                mapData += _mapBlockData[i] + "\n";
            }
            if (File.Exists(MapDefine.MapDataSavePath))
                File.Delete(MapDefine.MapDataSavePath);
            File.WriteAllText(MapDefine.MapDataSavePath, mapData.Trim());
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.Log("没有数据");
        }
    }
}