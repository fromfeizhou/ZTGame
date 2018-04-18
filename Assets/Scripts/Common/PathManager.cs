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
    public static string LuaPath = "Assets/LuaScript";
    public static string HotLuaPath = "Assets/PersistentAssets/AssetBundle/LuaScript";
	public const string NetWorkErrCodeFilePath = "ecode_zh";

    private static string GetRootPath(string key = "ImgShopItem", bool isResoucePath = true)
    {
        string path = "";
        string root = "Assets";
        if (isResoucePath)
        {
            root = PathManager.ResoucePath;
        }
        if (pathDic == null)
        {
            ParsePath();
            return "";
        }
        if (pathDic.ContainsKey(key))
        {
            path = System.IO.Path.Combine(root, pathDic[key]);
        }
         
        return path;
    }
    /// <summary>
    /// resourcesLib目录下的资源
    /// </summary>
    /// <param name="key"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetResPathByName(string key = "ImgShopItem", string name = "10001.png")
    {
        string dic = GetRootPath(key);
        return CombinePath(dic, name);
    }
    /// <summary>
    /// 全路径
    /// </summary>
    /// <param name="key"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetFullPathByName(string key = "ImgShopItem", string name = "10001.png")
    {
        string dic = GetRootPath(key,false);
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
