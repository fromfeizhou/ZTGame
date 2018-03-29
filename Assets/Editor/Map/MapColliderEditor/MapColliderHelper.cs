using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;


public class MapColliderHelper
{
    public enum eMapEditHelper
    {
        None,
        SaveMapBlockFile,
    }

    public static void SaveMapBytesFile(byte[] clolliderdata, Dictionary<string, MapBlockData> hideData, Dictionary<string, MapBlockData> heightData)
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        if (File.Exists(MapDefine.MapDataSavePath))
            File.Delete(MapDefine.MapDataSavePath);
        File.WriteAllBytes(MapDefine.MapDataSavePath, clolliderdata);

        //  获取当前实例测量得出的总时间
        System.TimeSpan timespan = stopwatch.Elapsed;
        //   double hours = timespan.TotalHours; // 总小时
        //    double minutes = timespan.TotalMinutes;  // 总分钟
        //    double seconds = timespan.TotalSeconds;  //  总秒数
        double milliseconds = timespan.TotalMilliseconds;  //  总毫秒数
        //打印代码执行时间
        Debug.Log("生成碰撞文件 用时：" + milliseconds + ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

        stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        if (File.Exists(MapDefine.MapHideBlockDataSavePath))
            File.Delete(MapDefine.MapHideBlockDataSavePath);
        byte[] hideTytes = new byte[hideData.Count * MapDefine.MapByteInterval];
        int index = 0;
        foreach (KeyValuePair<string, MapBlockData> item in hideData)
        {
            Array.Copy(item.Value.GetBytes(), 0, hideTytes, index++ * MapDefine.MapByteInterval, MapDefine.MapByteInterval);
        }
        File.WriteAllBytes(MapDefine.MapHideBlockDataSavePath, hideTytes);

        stopwatch.Stop();
        timespan = stopwatch.Elapsed;
        Debug.Log("生成草地隐藏文件 用时：" + timespan.TotalMilliseconds + ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
      
        stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        if (File.Exists(MapDefine.MapHeightBlockDataSavePath))
            File.Delete(MapDefine.MapHeightBlockDataSavePath);

        hideTytes = new byte[heightData.Count * MapDefine.MapByteInterval];
        index = 0;
        foreach (KeyValuePair<string, MapBlockData> item in heightData)
        {
            Array.Copy(item.Value.GetBytes(), 0, hideTytes, index++ * MapDefine.MapByteInterval, MapDefine.MapByteInterval);
        }
        File.WriteAllBytes(MapDefine.MapHeightBlockDataSavePath, hideTytes);
        stopwatch.Stop();
        timespan = stopwatch.Elapsed;
        Debug.Log("生成高度隐藏文件 用时：" + timespan.TotalMilliseconds + ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
    }

    public static void SaveMapBlockFile(List<MapBlockData> _mapBlockData)
    {
        if (_mapBlockData != null)
        {
            byte[] tempbytes = new byte[_mapBlockData.Count * 6];
            //string mapData = string.Empty;
            StringBuilder mapdata = new StringBuilder();
            string mapRoleCreatePoint = string.Empty;
            for (int i = 0; i < _mapBlockData.Count; i++)
            {
                if (_mapBlockData[i].type == eMapBlockType.None)
                    continue;
                //  mapdata.Append(_mapBlockData[i] + "\n");
                byte[] aa = _mapBlockData[i].GetBytes();
                Array.Copy(aa, 0, tempbytes, i * 6, 6);
            }
            if (File.Exists(MapDefine.MapDataSavePath))
                File.Delete(MapDefine.MapDataSavePath);


            File.WriteAllBytes(MapDefine.MapDataSavePath, tempbytes);//(MapDefine.MapDataSavePath, mapData.Trim());


            byte[] contents = File.ReadAllBytes(MapDefine.MapDataSavePath);

            byte temp = (byte)0;
            //temp |= 1;
            //temp |= 2;
            //temp |= 4;
            //temp |= 8;
            //temp |= 32;
            //temp |= 64;
            //temp |= 128;


            //FileStream fs = File.Create(MapDefine.MapDataSavePath);

            //byte[] info = new UTF8Encoding(true).GetBytes(mapdata.ToString());
            //fs.Write(info, 0, info.Length);
            //fs.Close();





            //  File.WriteAllText(MapDefine.MapDataSavePath, mapData.Trim());



            AssetDatabase.Refresh();
        }
        else
        {
            Debug.Log("没有数据");
        }
    }
}