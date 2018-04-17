using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Xml;
using UnityEditor.Animations;

class AssetLuaZip
{
    //创建Prefab  
    [MenuItem("CYH_Tools/AssetLuaZip")]
    static void CreateLuaZip()
    {
        string path = Application.dataPath + "/LuaScript";
        string outPath = "D:/IISRoot/LuaScript.zip";
        string error;
        float process;
        FileHelper.DeleteFile(outPath,"");
        CompressHelper.ZipFile(path, outPath, out error,out process,"*.txt");
    }


}