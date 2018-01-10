
using System.Collections;
/// <summary>
///   平台相关
/// </summary>
using UnityEngine;
public static class Platform
{
#if UNITY_EDITOR
    public static string STREAMING_ASSETS_PATH = Application.streamingAssetsPath;
    public static string PERSISTENT_DATA_PATH = Application.dataPath + "/PersistentAssets";
#elif UNITY_STANDALONE_WIN
        public static string STREAMING_ASSETS_PATH = Application.streamingAssetsPath;
        public static string PERSISTENT_DATA_PATH = Application.dataPath + "/PersistentAssets";
#elif UNITY_IPHONE
        public static string STREAMING_ASSETS_PATH = Application.streamingAssetsPath;
        public static string PERSISTENT_DATA_PATH = Application.persistentDataPath;
#elif UNITY_ANDROID
        public static string STREAMING_ASSETS_PATH = Application.streamingAssetsPath;
        public static string PERSISTENT_DATA_PATH = Application.persistentDataPath;
#endif
}

public static class DownLoadCommon
{
    /// <summary>
    ///   AssetBundle后缀名
    /// </summary>
    public const string EXTENSION = "_ab";

    /// <summary>
    /// ab资源描述后缀
    /// </summary>
    public const string NATIVE_MANIFEST_EXTENSION = ".manifest";

    /// <summary>
    /// 资源所在根文件夹名
    /// </summary>
    public const string ROOT_FOLDER_NAME = "AssetBundle";

    /// <summary>
    ///   常驻根路径
    ///   此路径主要存放所有游戏中所需使用的AssetBundle、其它配置文件
    /// </summary>
    public static readonly string PATH = Platform.PERSISTENT_DATA_PATH + "/" + ROOT_FOLDER_NAME;

    /// <summary>
    ///   初始路径
    ///   此路径为安装包中的AssetBundlePacker所携带的资源和配置文件路径
    /// </summary>
    public static readonly string INITIAL_PATH = Platform.STREAMING_ASSETS_PATH + "/" + ROOT_FOLDER_NAME;

    /// <summary>
    ///   缓存路径
    ///   此路径主要存放临时文件（下载，拷贝缓存等）
    /// </summary>
    public static readonly string CACHE_PATH = PATH + "/Cache";

    /// <summary>
    ///   DownloadCache文件路径
    /// </summary>
    public static readonly string DOWNLOADCACHE_FILE_PATH = CACHE_PATH + "/DownloadCache.cfg";

    /// <summary>
    ///   主Manifest文件名称（必须存在）
    /// </summary>
    public const string MAIN_MANIFEST_FILE_NAME = "AssetBundles";

    /// <summary>
    ///   获得资源全局路径
    /// </summary>
    public static string GetFileFullName(string file)
    {
        return PATH + "/" + file;
    }

    /// <summary>
    ///   获得资源原始全局路径
    /// </summary>
    public static string GetInitialFileFullName(string file)
    {
        return INITIAL_PATH + "/" + file;
    }

    /// <summary>
    ///   获得缓存路径
    /// </summary>
    public static string GetCacheFileFullName(string file)
    {
        return CACHE_PATH + "/" + file;
    }

    /// <summary>
    ///   拷贝文件
    /// </summary>
    public static IEnumerator StartCopyFile(string str, string dest)
    {
        yield return FileHelper.CopyStreamingAssetsToFile(str, dest);
    }

    /// <summary>
    ///   拷贝文件
    /// </summary>
    public static IEnumerator StartCopyInitialFile(string local_name)
    {
        yield return FileHelper.CopyStreamingAssetsToFile(
                                            GetInitialFileFullName(local_name),
                                            GetFileFullName(local_name));
    }

    /// <summary>
    ///   载入Manifest
    /// </summary>
    public static AssetBundleManifest LoadMainManifest()
    {
        string file = DownLoadCommon.GetFileFullName(DownLoadCommon.MAIN_MANIFEST_FILE_NAME);
        return LoadMainManifestByPath(file);
    }


    /// <summary>
    ///   载入Manifest
    /// </summary>
    public static AssetBundleManifest LoadMainManifestByPath(string full_name)
    {
        if (!System.IO.File.Exists(full_name))
        {
            return null;
        }

        AssetBundleManifest manifest = null;
        UnityEngine.AssetBundle mainfest_bundle = UnityEngine.AssetBundle.LoadFromFile(full_name);
        if (mainfest_bundle != null)
        {
            manifest = (AssetBundleManifest)mainfest_bundle.LoadAsset("AssetBundleManifest");
            mainfest_bundle.Unload(false);
        }

        return manifest;
    }

    /// <summary>
    /// 计算指定URL的下载URL
    /// </summary>
    public static string CalcAssetBundleDownloadURL(string url)
    {
        string new_url = url;
        if (new_url[new_url.Length - 1] != '/')
            new_url = new_url + '/';
        new_url = new_url + ROOT_FOLDER_NAME + "/";

        return new_url;
    }
}