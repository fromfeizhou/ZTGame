using System;
using System.Collections.Generic;
using System.IO;
using NPOI.SS.Formula.Functions;
using UnityEditor;
using UnityEngine;


public class MapByteFileCreateEditor
{
    enum MapByteDataType
    {
        Collider=1,
        Hide=2,
        Height=3,
        All=4,
    }
    private static Dictionary<string, MapBlockData> MapHeightBlockDataDic = new Dictionary<string, MapBlockData>();
    private static Dictionary<string, MapBlockData> MapHideBlockDataDic = new Dictionary<string, MapBlockData>();
    private static byte[] blockBytesData;
    private static MapByteDataType fileData = MapByteDataType.All;

    [MenuItem("ZTTool/MapTool/生成地图所需文件")]
    public static void CreateAllFile()
    {
        fileData = MapByteDataType.All;
        Create();
    }

    [MenuItem("ZTTool/MapTool/生成草地隐身文件")]
    public static void CreateHideFile()
    {
        fileData = MapByteDataType.Hide;
        Create();
    }

    [MenuItem("ZTTool/MapTool/生成建筑高度文件")]
    public static void CreateHeightFile()
    {
        fileData = MapByteDataType.Height;
        Create();
    }


    [MenuItem("ZTTool/MapTool/生成碰撞文件")]
    public static void CreateColliderFile()
    {
        fileData = MapByteDataType.Collider;

        Create();
    }

    private static void Create()
    {
        MapHeightBlockDataDic.Clear();
        MapHideBlockDataDic.Clear();
        blockBytesData = new byte[13107200];//2048*5*2048*5
        string bigMapIndex = "0_0";// 大地图坐标 后续通过读取场景名字获取
        string[] bigMapIndexs = bigMapIndex.Split('_');
        if (bigMapIndexs.Length != 2)
        {
            Debug.LogError("Map Data is Error!!!");
            return;
        }
        int bigMapX = int.Parse(bigMapIndexs[0]);
        int bigMapY = int.Parse(bigMapIndexs[1]);

        //基于大地图偏移
        int offsetX = bigMapX * MapDefine.MAPITEMTOTALSIZE;
        int offsetY = bigMapY * MapDefine.MAPITEMTOTALSIZE;

        GameObject mapElementRoot = GameObject.Find("MapElement");
        if (mapElementRoot == null) return;
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        CreateElement(mapElementRoot.transform, offsetX, offsetY);
        stopwatch.Stop();
        System.TimeSpan timespan = stopwatch.Elapsed;
        double milliseconds = timespan.TotalMilliseconds;  //  总毫秒数
        Debug.Log("遍历场景 用时：" + milliseconds + ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
        SaveMapBytesFile(blockBytesData, MapHideBlockDataDic, MapHeightBlockDataDic);
        AssetDatabase.Refresh();
    }

    private static void CreateElement(Transform go, int offsetX, int offsetY)
    {
        if (go == null) return;
        for (int index = 0; index < go.childCount; index++)
        {
            Transform element = go.GetChild(index);
            if (element.name.Contains("Ele_"))
            {
                Transform colliderRoots = element.Find("ColliderRoot");
                if (colliderRoots == null) continue;
                List<KeyValuePair<string, CollRectange>> goInCludeRects = GetGoIncludeBlocks(element, offsetX, offsetY);
                for (int rootIndex = 0; rootIndex < colliderRoots.childCount; rootIndex++)
                {
                    Transform root = colliderRoots.GetChild(rootIndex);
                    string[] datas = root.name.Split('_');
                    if (!Enum.IsDefined(typeof(eMapBlockType), datas[0])) continue;
                    eMapBlockType blockType = (eMapBlockType)Enum.Parse(typeof(eMapBlockType), datas[0]);
                    if ((int) fileData != (int) blockType && fileData != MapByteDataType.All) continue;

                    string blockParam = datas.Length > 1 ? datas[1] : "";
                    Renderer[] colliderRenderers = root.GetComponentsInChildren<Renderer>(true);
                    List<CollRectange> colliderRectList = new List<CollRectange>();
                    for (int colliderIndex = 0; colliderIndex < colliderRenderers.Length; colliderIndex++)
                    {
                        Renderer tempRenderer = colliderRenderers[colliderIndex];
                        CollRectange tempColl = new CollRectange(tempRenderer.transform.position.x,
                            tempRenderer.transform.position.z, tempRenderer.transform.eulerAngles.y,
                            Mathf.Abs(tempRenderer.transform.lossyScale.x),
                            Mathf.Abs(tempRenderer.transform.lossyScale.z));
                        colliderRectList.Add(tempColl);
                    }
                    for (int blockIndex = goInCludeRects.Count - 1; blockIndex >= 0; blockIndex--)
                    {
                        for (int colliderIndex = 0; colliderIndex < colliderRectList.Count; colliderIndex++)
                        {
                            if (ZTCollider.CheckCollision(goInCludeRects[blockIndex].Value, colliderRectList[colliderIndex]))
                            {
                                if (blockType == eMapBlockType.Height)
                                    blockParam = Mathf.Abs(colliderRenderers[colliderIndex].bounds.size.y) + "";
                                AddColliderToDic(goInCludeRects[blockIndex].Key, blockType, blockParam);
                                //goInCludeRects.RemoveAt(blockIndex);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                CreateElement(element, offsetX, offsetY);
            }
        }
    }
    //获取go包含的小格子（0.2米平方大小）
    private static List<KeyValuePair<string, CollRectange>> GetGoIncludeBlocks(Transform element, int offsetX, int offsetY)
    {
        int col = (int)((element.position.x + offsetX) / MapDefine.MapBlockSize);
        int row = (int)((element.position.z + offsetY) / MapDefine.MapBlockSize);
        string elementGridKey = col + "" + row;

        Vector3 postion = element.position;
        Vector3 scale = element.localScale;
        element.position = Vector3.zero;
        Vector3 center = Vector3.zero;
        Renderer[] renders = element.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer child in renders)
            center += child.bounds.center;
        center /= renders.Length;
        Bounds bounds = new Bounds(center, Vector3.zero);
        foreach (Renderer child in renders)
            bounds.Encapsulate(child.bounds);
        Vector3 centralPoint = bounds.center;
        element.position = postion;

        centralPoint += element.position;
        int starX = (int)((centralPoint.x - bounds.size.x * 0.5f + offsetX) / MapDefine.MapBlockSize);
        int endX = (int)((centralPoint.x + bounds.size.x * 0.5f + offsetX) / MapDefine.MapBlockSize);
        int starZ = (int)((centralPoint.z - bounds.size.z * 0.5f + offsetY) / MapDefine.MapBlockSize);
        int endZ = (int)((centralPoint.z + bounds.size.z * 0.5f + offsetY) / MapDefine.MapBlockSize);
        // Debug.LogError("centralPoint"+ centralPoint+"    "+starX + " "+ endX + "  "+starZ + "  "+ endZ);
        List<KeyValuePair<string, CollRectange>> curGoInCludeBlockList = new List<KeyValuePair<string, CollRectange>>();


        float radius = MapDefine.MapBlockSize * 0.5f;
        for (int k = starX; k <= endX; k++)
        {
            for (int j = starZ; j <= endZ; j++)
            {
                string key = k + "_" + j;
                KeyValuePair<string, CollRectange> item = new KeyValuePair<string, CollRectange>(key,
                    new CollRectange(MapDefine.MapBlockSize * k + radius,
                        MapDefine.MapBlockSize * j + radius, 0, MapDefine.MapBlockSize, MapDefine.MapBlockSize));
                curGoInCludeBlockList.Add(item);
            }
        }
        return curGoInCludeBlockList;
    }

    private static MapBlockData tempBlock;
    private static void AddColliderToDic(string key, eMapBlockType mapBlockType, string param)
    {
        if (mapBlockType == eMapBlockType.Collect)
        {
            string[] datas = key.Split('_');
            int row = (int)(int.Parse(datas[0]));
            int col = (int)(int.Parse(datas[1]));
            int index = row + col * 10240;
            int byteRow = index / 8;
            int byteCol = index % 8;

            if (byteRow >= blockBytesData.Length)
            {
                Debug.LogError("byteRow:" + byteRow + "  " + key);
                return;
            }
            byte curByte = blockBytesData[byteRow];
            byte temp = (byte)Mathf.Pow(2, byteCol);
            curByte |= temp;
            blockBytesData[byteRow] = curByte;
            // Debug.LogError(row+"    "+col+"  "+"  ");
        }
        else
        {
            string[] datas = key.Split('_');
            int paramValue = string.IsNullOrEmpty(param) ? 0 : (int)(float.Parse(param) * 100);
            tempBlock = new MapBlockData
            {
                row = int.Parse(datas[0]),
                col = int.Parse(datas[1]),
                type = mapBlockType,
                paramValue = paramValue
            };
            if (mapBlockType == eMapBlockType.Hide)
            {
                if (!MapHideBlockDataDic.ContainsKey(key))
                    MapHideBlockDataDic[key] = tempBlock;
            }
            else if (mapBlockType == eMapBlockType.Height)
            {
                if (!MapHeightBlockDataDic.ContainsKey(key))
                    MapHeightBlockDataDic[key] = tempBlock;
            }
        }


        //if (!MapHideBlockDataDic.ContainsKey(key))
        //{
        //    string[] datas = key.Split('_');
        //    int paramValue = string.IsNullOrEmpty(param) ?  0 : (int)(float.Parse(param) * 100);
        //    MapHideBlockDataDic[key] = new MapBlockData { row = int.Parse(datas[0]), col = int.Parse(datas[1]), type = mapBlockType, paramValue = paramValue };
        //}
    }

    public static void SaveMapBytesFile(byte[] clolliderdata, Dictionary<string, MapBlockData> hideData, Dictionary<string, MapBlockData> heightData)
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        System.TimeSpan timespan = TimeSpan.Zero;
        byte[] hideTytes = null;
        int index = 0;
        if (fileData == MapByteDataType.All || fileData == MapByteDataType.Collider)
        {
            if (File.Exists(MapDefine.MapDataSavePath))
                File.Delete(MapDefine.MapDataSavePath);
            File.WriteAllBytes(MapDefine.MapDataSavePath, clolliderdata);
            timespan = stopwatch.Elapsed;
            double milliseconds = timespan.TotalMilliseconds;  //  总毫秒数
            Debug.Log("生成碰撞文件 用时：" + milliseconds + ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
        }

        if (fileData == MapByteDataType.All || fileData == MapByteDataType.Hide)
        {
            stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            if (File.Exists(MapDefine.MapHideBlockDataSavePath))
                File.Delete(MapDefine.MapHideBlockDataSavePath);
            hideTytes = new byte[hideData.Count * MapDefine.MapByteInterval];
            index = 0;
            foreach (KeyValuePair<string, MapBlockData> item in hideData)
            {
                Array.Copy(item.Value.GetBytes(), 0, hideTytes, index++ * MapDefine.MapByteInterval, MapDefine.MapByteInterval);
            }
            File.WriteAllBytes(MapDefine.MapHideBlockDataSavePath, hideTytes);
            stopwatch.Stop();
            timespan = stopwatch.Elapsed;
            Debug.Log("生成草地隐藏文件 用时：" + timespan.TotalMilliseconds + ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
        }

        if (fileData == MapByteDataType.All || fileData == MapByteDataType.Height)
        {
            stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            if (File.Exists(MapDefine.MapHeightBlockDataSavePath))
                File.Delete(MapDefine.MapHeightBlockDataSavePath);
            hideTytes = new byte[heightData.Count * MapDefine.MapByteInterval];
            index = 0;
            foreach (KeyValuePair<string, MapBlockData> item in heightData)
                Array.Copy(item.Value.GetBytes(), 0, hideTytes, index++ * MapDefine.MapByteInterval, MapDefine.MapByteInterval);
            File.WriteAllBytes(MapDefine.MapHeightBlockDataSavePath, hideTytes);
            stopwatch.Stop();
            timespan = stopwatch.Elapsed;
            Debug.Log("生成高度隐藏文件 用时：" + timespan.TotalMilliseconds + ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
        }
    }
}

