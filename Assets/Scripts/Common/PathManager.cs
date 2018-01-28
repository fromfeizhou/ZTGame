using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class JsonPathMode
{
    //JsonArrayModel类型的列表
    public List<JsonPathArrModel> infoList;
}
[System.Serializable]
public class JsonPathArrModel
{
    //对应Json中属性 名字要一样
    public string key;
    public string path;

}

public class PathManager
{
    public static Dictionary<string, string> pathDic;
    public static string ConfigPath = "Assets/ResourcesLib/Config";
    public static string ResoucePath = "Assets/ResourcesLib";
    //获取resourceLib下的目录
    public static string GetResPath(string key = "ImgShopItem", string rootKey = "")
    {
        string path = "";
        string root = PathManager.ResoucePath;
        if (pathDic == null)
        {
            ParsePath();
        }
        if (pathDic.ContainsKey(rootKey))
         {
             root = pathDic[rootKey];
         }

        if (pathDic.ContainsKey(key))
        {
            path = System.IO.Path.Combine(root, pathDic[key]);
        }
         
        return path;
    }

    public static string GetResPathByName(string key = "ImgShopItem", string name = "10001.png", string rootKey = "")
    {
        string dic = GetResPath(key, rootKey);
        return CombinePath(dic, name);
    }

    //解析path配置
    public static void ParsePath()
    {
        if (pathDic != null) return;
        Debug.Log("PathManager ParsePath");
        //列表初始化
        pathDic = new Dictionary<string, string>();
        //资源加载
        AssetManager.LoadAsset(GetPathConfigPath(), AssetLoadCallBack);
    }

    //pathJson加载回调
    private static void AssetLoadCallBack(Object target, string path)
    {
        if (null == target)
        {
            return;
        }
        TextAsset txt = target as TextAsset;
        JsonPathMode jsonObject = JsonUtility.FromJson<JsonPathMode>(txt.text);
        foreach (var info in jsonObject.infoList)
        {
            pathDic.Add(info.key, info.path);
        }

        GameStartEvent.GetInstance().dispatchEvent(GAME_LOAD_SETP_EVENT.LOAD_PATH);
    }

    //获取地址配置文件 地址
    public static string GetPathConfigPath()
    {
        return CombinePath(PathManager.ConfigPath, "pathConfig.json");
    }

    //获取语音包配置文件 地址
    public static string GetLocalStringPath()
    {
        return CombinePath(PathManager.ConfigPath, "localString.txt");
    }


    public static string CombinePath(string root, string name)
    {
        return System.IO.Path.Combine(root, name);
    }
    //销毁
    public static void Destroy()
    {
        pathDic = null;
    }
}
