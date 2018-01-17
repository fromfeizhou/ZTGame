/***************************************************************
 * Copyright 2016 By Zhang Minglin
 * Author: Zhang Minglin
 * Create: 2016/01/18
 * Note  : AssetBundle资源管理
 *         负责游戏中的AssetBundle资源加载
***************************************************************/
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
///   资源管理器
/// </summary>
public class AssetBundleManager : MonoSingleton<AssetBundleManager>
{
    /// <summary>
    ///   最新的资源版本
    /// </summary>
    public uint Version;

    /// <summary>
    ///   是否准备完成
    /// </summary>
    public bool IsReady { get; private set; }

    /// <summary>
    ///   是否出错
    /// </summary>
    public bool IsFailed
    {
        get { return ErrorCode != EmErrorCode.None; }
    }

    /// <summary>
    /// 
    /// </summary>
    public EmErrorCode ErrorCode { get; private set; }

    /// <summary>
    ///   主AssetBundleMainfest
    /// </summary>
    public AssetBundleManifest MainManifest { get; private set; }

    /// <summary>
    ///   常驻的AssetBundle
    /// </summary>
    private Dictionary<string, AssetBundle> _assetbundle_permanent;

    /// <summary>
    ///   缓存的AssetBundle
    /// </summary>
    private Dictionary<string, AssetBundle> _assetbundle_cache;

    /// <summary>
    /// 临时的AssetBundle
    /// </summary>
    private Dictionary<string, AssetBundle> _assetbundle_temporary;

    protected AssetBundleManager()
    { }

    /// <summary>
    /// 重启
    /// </summary>
    public bool Relaunch()
    {
        //必须处于启动状态或者异常才可以重启
        if (!(IsReady || IsFailed))
            return false;

        ShutDown();
        Launch();

        return true;
    }

    /// <summary>
    /// 等待启动完毕，启动完毕返回True,
    /// </summary>
    public bool WaitForLaunch()
    {
        if (IsReady || IsFailed)
            return true;

        return false;
    }

    /// <summary>
    ///   判断一个AssetBundle是否存在缓存
    /// </summary>
    public bool IsExist(string assetbundlename)
    {
        if (!IsReady)
            return false;

        if (string.IsNullOrEmpty(assetbundlename))
            return false;

        return File.Exists(DownLoadCommon.GetFileFullName(assetbundlename));
    }



    /// <summary>
    ///   获得AssetBundle中的所有资源
    /// </summary>
    public string[] FindAllAssetNames(string assetbundlename)
    {
        AssetBundle bundle = LoadAssetBundle(assetbundlename);
        if (bundle != null)
            return bundle.GetAllAssetNames();
        return null;
    }

    /// <summary>
    ///   释放常驻的AssetBundle
    /// </summary>
    public void UnloadAllAssetBundle(bool unload_all_loaded_objects)
    {
        UnloadAssetBundleCache(unload_all_loaded_objects);
        UnloadAssetBundlePermanent(unload_all_loaded_objects);
    }

    /// <summary>
    ///   释放缓存的AssetBundle
    /// </summary>
    public void UnloadAssetBundleCache(bool unload_all_loaded_objects)
    {
        var itr = _assetbundle_cache.Values.GetEnumerator();
        while (itr.MoveNext())
        {
            itr.Current.Unload(unload_all_loaded_objects);
        }
        itr.Dispose();
        _assetbundle_cache.Clear();
    }

    /// <summary>
    ///   释放缓存的AssetBundle
    /// </summary>
    public void UnloadAssetBundle(string assetbundlename, bool unload_all_loaded_objects)
    {
        AssetBundle ab = null;
        if (_assetbundle_permanent.TryGetValue(assetbundlename, out ab))
            _assetbundle_permanent.Remove(assetbundlename);
        else if (_assetbundle_cache.TryGetValue(assetbundlename, out ab))
            _assetbundle_cache.Remove(assetbundlename);
        else if (_assetbundle_temporary.TryGetValue(assetbundlename, out ab))
            _assetbundle_temporary.Remove(assetbundlename);

        ab.Unload(unload_all_loaded_objects);
    }

    /// <summary>
    ///   加载有依赖的AssetBundle
    /// </summary>
    public AssetBundle LoadAssetBundleAndDependencies(string assetbundlename)
    {
        if (assetbundlename == null)
            return null;
        if (MainManifest == null)
            return null;

        string[] deps = MainManifest.GetAllDependencies(assetbundlename);
        for (int index = 0; index < deps.Length; index++)
        {
            //加载所有的依赖AssetBundle
            if (LoadAssetBundle(deps[index]) == null)
            {
                Debug.LogWarning(assetbundlename + "'s Dependencie AssetBundle can't find. Name is " + deps[index] + "!");
                return null;
            }

        }

        return LoadAssetBundle(assetbundlename);
    }

    private static List<string> _permanentList = new List<string> { "assets_resourceslib_config" };
    /// <summary>
    ///   加载AssetBundle
    /// </summary>
    AssetBundle LoadAssetBundle(string assetbundlename)
    {
        if (assetbundlename == null)
            return null;
        if (MainManifest == null)
            return null;

        AssetBundle ab = null;
        if (_assetbundle_permanent.ContainsKey(assetbundlename))
        {
            ab = _assetbundle_permanent[assetbundlename];
        }
        else if (_assetbundle_cache.ContainsKey(assetbundlename))
        {
            ab = _assetbundle_cache[assetbundlename];
        }
        else if (_assetbundle_temporary.ContainsKey(assetbundlename))
        {
            ab = _assetbundle_temporary[assetbundlename];
        }
        else
        {
            string assetbundle_path = DownLoadCommon.GetFileFullName(assetbundlename);
            if (System.IO.File.Exists(assetbundle_path))
            {
                ab = AssetBundle.LoadFromFile(assetbundle_path);

                //根据AssetBundleDescribe分别存放AssetBundle
                if (_permanentList.Exists(a => a == assetbundlename))
                    _assetbundle_permanent.Add(assetbundlename, ab);
                else
                    _assetbundle_temporary.Add(assetbundlename, ab);
            }
        }

        return ab;
    }

    /// <summary>
    ///   释放常驻的AssetBundle
    /// </summary>
    void UnloadAssetBundlePermanent(bool unload_all_loaded_objects)
    {
        var itr = _assetbundle_permanent.Values.GetEnumerator();
        while (itr.MoveNext())
        {
            itr.Current.Unload(unload_all_loaded_objects);
        }
        itr.Dispose();
        _assetbundle_permanent.Clear();
    }

    /// <summary>
    /// 释放临时的AssetBundle
    /// </summary>
    void UnloadAssetBundleTemporary(bool unload_all_loaded_objects)
    {
        var itr = _assetbundle_temporary.Values.GetEnumerator();
        while (itr.MoveNext())
        {
            itr.Current.Unload(unload_all_loaded_objects);
        }
        itr.Dispose();
        _assetbundle_temporary.Clear();
    }

    /// <summary>
    /// 保存到缓存中
    /// </summary>
    void SaveAssetBundleToCache()
    {
        var itr = _assetbundle_temporary.GetEnumerator();
        while (itr.MoveNext())
        {
            _assetbundle_cache.Add(itr.Current.Key, itr.Current.Value);
        }
        itr.Dispose();
        _assetbundle_temporary.Clear();
    }

    /// <summary>
    /// 处理临时AssetBundle
    /// </summary>
    void DisposeAssetBundleTemporary(bool unload_assetbundle, bool unload_all_loaded_objects)
    {
        if (unload_assetbundle)
            UnloadAssetBundleTemporary(unload_all_loaded_objects);
        else
            SaveAssetBundleToCache();
    }

    /// <summary>
    /// 
    /// </summary>
    void Error(EmErrorCode ec, string message = null)
    {
        ErrorCode = ec;

        StringBuilder sb = new StringBuilder("[AssetBundleError] - ");
        sb.Append(ErrorCode.ToString());
        if (!string.IsNullOrEmpty(message)) { sb.Append("\n"); sb.Append(message); }
        Debug.LogWarning(sb.ToString());
    }

    /// <summary>
    ///   启动(仅内部启用)
    /// </summary>
    void Launch()
    {
        if (_assetbundle_permanent == null)
            _assetbundle_permanent = new Dictionary<string, AssetBundle>();
        if (_assetbundle_cache == null)
            _assetbundle_cache = new Dictionary<string, AssetBundle>();
        if (_assetbundle_temporary == null)
            _assetbundle_temporary = new Dictionary<string, AssetBundle>();
        IsReady = false;
        ErrorCode = EmErrorCode.None;
        StopAllCoroutines();
        StartCoroutine(Preprocess());
    }

    /// <summary>
    /// 关闭
    /// </summary>
    void ShutDown()
    {
        UnloadAllAssetBundle(true);
    }

    /// <summary>
    ///   初始化
    /// </summary>
    IEnumerator Preprocess()
    {
        //创建资源根目录
        if (!Directory.Exists(DownLoadCommon.PATH))
            Directory.CreateDirectory(DownLoadCommon.PATH);

        //判断主资源文件是否存在，不存在则拷贝备份资源至资源根目录
        bool do_initial_copy = false;
        string resources_manifest_file = DownLoadCommon.GetFileFullName(DownLoadCommon.MAIN_MANIFEST_FILE_NAME);
        if (!File.Exists(resources_manifest_file))
        {
            do_initial_copy = true;
        }
        else
        {
            // 拷贝安装包初始化目录中的ResourcesManifest，并判断是否重新拷贝初始化目录下的所有文件
            //string initial_full_name = DownLoadCommon.GetInitialFileFullName(DownLoadCommon.MAIN_MANIFEST_FILE_NAME);
            //string cache_full_name = DownLoadCommon.GetCacheFileFullName(DownLoadCommon.MAIN_MANIFEST_FILE_NAME);
            //yield return DownLoadCommon.StartCopyFile(initial_full_name, cache_full_name);

            ////判断安装包初始目录是否完整
            //ResourcesManifest initial = DownLoadCommon.LoadResourcesManifestByPath(cache_full_name);
            //if (initial == null)
            //{
            //    Error(EmErrorCode.PreprocessError
            //        , "Initial path don't contains "
            //            + DownLoadCommon.RESOURCES_MANIFEST_FILE_NAME + "!");
            //    yield break;
            //}

            //ResourcesManifest current = DownLoadCommon.LoadResourcesManifestByPath(full_name);
            //if (current == null)
            //    do_initial_copy = true;
            //else if (current.Data.Version < initial.Data.Version)
            //    do_initial_copy = true;

            //删除缓存中的文件
            //if (File.Exists(cache_full_name))
            //    File.Delete(cache_full_name);
        }

        if (do_initial_copy)
        {
            yield return CopyAllInitialFiles();
        }

        PreprocessFinished();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    bool PreprocessFinished()
    {
        MainManifest = DownLoadCommon.LoadMainManifest();
        if (MainManifest == null)
        {
            Error(EmErrorCode.LoadMainManifestFailed
                , "Can't load MainManifest file!");
            return false;
        }


        ////记录当前版本号
        //Version = ResourcesManifest.Data.Version;
        ////标记已准备好
        IsReady = ErrorCode == EmErrorCode.None;

        return true;
    }

    /// <summary>
    ///   拷贝初始目录所有文件
    /// </summary>
    IEnumerator CopyAllInitialFiles()
    {
        //拷贝所有配置文件
        yield return DownLoadCommon.StartCopyInitialFile(DownLoadCommon.MAIN_MANIFEST_FILE_NAME);

		string initial_full_name = DownLoadCommon.GetFileFullName(DownLoadCommon.MAIN_MANIFEST_FILE_NAME);
        AssetBundleManifest initial = DownLoadCommon.LoadMainManifestByPath(initial_full_name);
        //拷贝AssetBundle文件
        string[] all_assetbundle = initial.GetAllAssetBundles();
        for (int i = 0; i < all_assetbundle.Length; ++i)
        {
            string name = all_assetbundle[i];
            yield return DownLoadCommon.StartCopyInitialFile(name);
        }

        //ResourcesManifest resources_manifest = DownLoadCommon.LoadResourcesManifest();
        //if (resources_manifest == null)
        //{
        //    Debug.LogWarning("Can't load ResourcesManifest file!");
        //    yield break;
        //}
        //var itr = resources_manifest.Data.AssetBundles.GetEnumerator();
        //while (itr.MoveNext())
        //{
        //    if (itr.Current.Value.IsNative)
        //    {
        //        string assetbundlename = itr.Current.Value.AssetBundleName;
        //        string dest = DownLoadCommon.GetFileFullName(assetbundlename);

        //        //保证路径存在
        //        string directory = Path.GetDirectoryName(dest);
        //        if (!Directory.Exists(directory))
        //            Directory.CreateDirectory(directory);

        //        //拷贝数据
        //        yield return DownLoadCommon.StartCopyInitialFile(assetbundlename);
        //    }
        //}
        //itr.Dispose();
    }

    IEnumerator _WaitUnloadAssetBundleCacheFor(AsyncOperation ao)
    {
        while (!ao.isDone)
            yield return 1;

        UnloadAssetBundleCache(false);
    }

    #region MonoBahaviour
    /// <summary>
    ///   
    /// </summary>
    void Awake()
    {
        Launch();
    }

    /// <summary>
    ///   
    /// </summary>
    void OnDestroy()
    {
        ShutDown();
    }
    #endregion
}

