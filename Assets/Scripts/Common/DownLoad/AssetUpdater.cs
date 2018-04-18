/***************************************************************
 * Copyright 2016 By Zhang Minglin
 * Author: Zhang Minglin
 * Create: 2016/03/11
 * Note  : AssetBundle资源更新器, 用于游戏启动时自动更新游戏资源
***************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XLua;

[LuaCallCSharp]
public class AssetUpdater : MonoBehaviour
{
    /// <summary>
    ///   状态
    /// </summary>
    public enum EmState
    {
        None,               // 无
        Initialize,         // 初始化更新器
        VerifyURL,          // 验证有效的URL
        DownLuaZip,         // 下载lua文件
        UnZipLua,           //解压lua包
        DownloadMainConfig, // 下载主要的配置文件
        UpdateAssetBundle,  // 更新AssetBundle
        CopyCacheFile,      // 复制缓存下的文件
        Dispose,            // 后备工作
        Completed,          // 完成
        Failed,             // 失败
        Cancel,             // 取消
        Abort,              // 中断

        Max
    }

    public delegate void UpdateDelegate(string name, float current, float max);
    private UpdateDelegate _onUpdate;
    public delegate void DoneDelegate();
    private DoneDelegate _onDone;

    public void SetAssetLuaFunc(UpdateDelegate update, DoneDelegate done)
    {
        _onUpdate = update;
        _onDone = done;
    }


    /// <summary>
    ///   是否结束
    /// </summary>
    public bool IsDone { get; private set; }

    /// <summary>
    ///   是否出错
    /// </summary>
    public bool IsFailed
    {
        get { return ErrorCode != EmErrorCode.None; }
    }

    /// <summary>
    ///   错误代码
    /// </summary>
    public EmErrorCode ErrorCode { get; private set; }

    /// <summary>
    ///   当前状态
    /// </summary>
    public EmState CurrentState { get; private set; }

    /// <summary>
    ///   当前状态的完成度
    /// </summary>
    public float CurrentStateCompleteValue { get; private set; }

    /// <summary>
    ///   当前状态的总需完成度
    /// </summary>
    public float CurrentStateTotalValue { get; private set; }

    /// <summary>
    /// 下载地址列表
    /// </summary>
    private List<string> _url_group;

    /// <summary>
    ///   当前可用的下载地址
    /// </summary>
    private string _current_url;

    /// <summary>
    ///   
    /// </summary>
    private URLVerifier _verifier;

    /// <summary>
    ///   文件下载器
    /// </summary>
    private FileDownload _file_download;

    /// <summary>
    ///   资源下载器
    /// </summary>
    private AssetBundleDownloader _ab_download;

    /// <summary>
    /// 
    /// </summary>
    protected AssetUpdater()
    { }

    /// <summary>
    ///   开始更新
    /// </summary>
    public bool StartUpdate(List<string> url_group)
    {
        //if (!AssetBundleManager.Instance.IsReady)
        //    return false;
        if (!IsDone && CurrentState != EmState.None)
            return false;

        _url_group = url_group;
        _current_url = null;
        StartCoroutine(Updating());

        return true;
    }

    /// <summary>
    ///   取消更新
    /// </summary>
    public void CancelUpdate()
    {
        StopAllCoroutines();

        if (_verifier != null)
        {
            _verifier.Abort();
            _verifier = null;
        }
        if (_file_download != null)
        {
            _file_download.Cancel();
            _file_download = null;
        }
        if (_ab_download != null)
        {
            _ab_download.Cancel();
            _ab_download = null;
        }
        SaveDownloadCacheData();
        UpdateState(EmState.Cancel);
        Done();
    }

    /// <summary>
    ///   
    /// </summary>
    public void AbortUpdate()
    {
        StopAllCoroutines();

        if (_verifier != null)
        {
            _verifier.Abort();
            _verifier = null;
        }
        if (_file_download != null)
        {
            _file_download.Abort();
            _file_download = null;
        }
        if (_ab_download != null)
        {
            _ab_download.Abort();
            _ab_download = null;

        }
        SaveDownloadCacheData();
        UpdateState(EmState.Abort);
        Done();
    }

    /// <summary>
    ///   更新
    /// </summary>
    IEnumerator Updating()
    {
        UpdateState(EmState.Initialize);
        yield return StartInitialize();
        UpdateState(EmState.VerifyURL);
        yield return StartVerifyURL();
        UpdateState(EmState.DownLuaZip);
        yield return StartDownLuaZip();
        UpdateState(EmState.UnZipLua);
        yield return StartUnZipLua();
        UpdateState(EmState.DownloadMainConfig);
        yield return StartDownloadMainConfig();
        UpdateState(EmState.UpdateAssetBundle);
        yield return StartUpdateAssetBundle();
        UpdateState(EmState.CopyCacheFile);
        yield return StartCopyCacheFile();
        UpdateState(EmState.Dispose);
        yield return StartDispose();
        UpdateState(ErrorCode == EmErrorCode.None ? EmState.Completed : EmState.Failed);

        Done();
    }

    #region Initialize
    /// <summary>
    ///   初始化更新器
    /// </summary>
    IEnumerator StartInitialize()
    {
        Debug.Log("AssetUpdater:StartInitialize");
        if (ErrorCode != EmErrorCode.None)
            yield break;

        UpdateCompleteValue(0f, 1f);

        //创建缓存目录
        if (!Directory.Exists(DownLoadCommon.CACHE_PATH))
            Directory.CreateDirectory(DownLoadCommon.CACHE_PATH);
        AssetBundleManager.GetInstance().Relaunch();

        //ab包启动
        while (!AssetBundleManager.GetInstance().WaitForLaunch())
        {
            yield return null;
        }
        UpdateCompleteValue(1f, 1f);
        yield return null;
    }
    #endregion

    #region VerifyURL
    /// <summary>
    ///   开始进行资源URL检测
    /// </summary>
    IEnumerator StartVerifyURL()
    {
        Debug.Log("AssetUpdater:StartVerifyURL");
        if (ErrorCode != EmErrorCode.None)
            yield break;

        UpdateCompleteValue(0f, 1f);

        //下载地址重定向为根文件夹
        for (int i = 0; i < _url_group.Count; ++i)
        {
            //#if UNITY_EDITOR
            //            _url_group[i] = _url_group[i] + "/win/AssetBundle/";
            //#elif UNITY_ANDROID
            //        _url_group[i] = _url_group[i] + "/android/AssetBundle/";
            //#endif
        }
        //找到合适的资源服务器
        _verifier = new URLVerifier(_url_group);
        _verifier.Start();
        while (!_verifier.IsDone)
        {
            yield return null;
        }
        _current_url = _verifier.URL;
        if (string.IsNullOrEmpty(_current_url))
        {
            Debug.LogWarning("Can't find valid Resources URL");
            Error(EmErrorCode.InvalidURL);
        }
        _verifier.Abort();
        _verifier = null;
        UpdateCompleteValue(1f, 1f);
        yield return null;
    }
    #endregion

    #region DownLuaZip
    IEnumerator StartDownLuaZip()
    {
        Debug.Log("AssetUpdater:StartDownLuaZip");
        if (ErrorCode != EmErrorCode.None)
            yield break;

        //下载主配置文件
        _file_download = new FileDownload(_current_url
                                , DownLoadCommon.CACHE_PATH
                                , "/LuaScript.zip");
        _file_download.Start();
        while (!_file_download.IsDone)
        {
            UpdateCompleteValue(_file_download.CompletedSize, _file_download.TotalSize);
            yield return null;
        }
        if (_file_download.IsFailed)
        {
            Error(EmErrorCode.DownloadFailed
                , " download luazip failed!");
            yield break;
        }
        _file_download = null;
        UpdateCompleteValue(1f, 1f);
    }
    #endregion

    #region StartUnZipLua
    IEnumerator StartUnZipLua()
    {
        Debug.Log("AssetUpdater:StartUnZipLua");
        if (ErrorCode != EmErrorCode.None)
            yield break;
      
        string path = DownLoadCommon.GetCacheFileFullName("LuaScript.zip");
        string outPath = DownLoadCommon.HOT_LUA_PATH;

        CompressHelper.CompressTask task = CompressHelper.UnCompressAsync(path, outPath);
        while (!task.IsDone)
        {
            Debug.Log(task.Progress);
            UpdateCompleteValue(Mathf.Floor(task.Progress * 100), 100f);
            yield return null;
        }
        File.Delete(path);
      
        UpdateCompleteValue(1f, 1f);
    }
    #endregion

    #region DownloadMainFile
    /// <summary>
    ///   开始进行主要文件下载,下载至缓存目录
    /// </summary>
    IEnumerator StartDownloadMainConfig()
    {
        Debug.Log("AssetUpdater:StartDownloadMainConfig");
        if (ErrorCode != EmErrorCode.None)
            yield break;

        //下载主配置文件
        _file_download = new FileDownload(_current_url
                                , DownLoadCommon.CACHE_PATH
                                , DownLoadCommon.MAIN_MANIFEST_FILE_NAME);
        _file_download.Start();
        while (!_file_download.IsDone)
        {
            yield return null;
        }
        if (_file_download.IsFailed)
        {
            Error(EmErrorCode.DownloadMainConfigFileFailed
                , DownLoadCommon.MAIN_MANIFEST_FILE_NAME + " download failed!");
            yield break;
        }
        _file_download = null;
        UpdateCompleteValue(1f, 1f);

        yield return null;
    }
    #endregion

    #region UpdateAssetBundle
    /// <summary>
    ///   更新AssetBundle
    /// </summary>
    IEnumerator StartUpdateAssetBundle()
    {
        Debug.Log("AssetUpdater:StartUpdateAssetBundle");
        if (ErrorCode != EmErrorCode.None)
            yield break;

        UpdateCompleteValue(0f, 0f);

        ////载入MainManifest
        AssetBundleManifest manifest = AssetBundleManager.GetInstance().MainManifest;
        //载入新的ResourcesManifest
        string file = DownLoadCommon.GetCacheFileFullName(DownLoadCommon.MAIN_MANIFEST_FILE_NAME);
        AssetBundleManifest new_manifest = DownLoadCommon.LoadMainManifestByPath(file);
        if (new_manifest == null)
        {
            Error(EmErrorCode.LoadNewMainManifestFailed
                , "Can't find new version MainManifest!");
            yield break;
        }

        ////获取需下载的资源列表与删除的资源的列表
        List<string> download_files = new List<string>();
        List<string> delete_files = new List<string>();
        CompareAssetBundleDifference(ref download_files, ref delete_files
                                    , manifest, new_manifest);

        //删除已废弃的文件
        if (delete_files.Count > 0)
        {
            for (int i = 0; i < delete_files.Count; ++i)
            {
                string full_name = DownLoadCommon.GetFileFullName(delete_files[i]);
                if (File.Exists(full_name))
                {
                    File.Delete(full_name);
                    yield return 0;
                }
            }
        }

        //更新所有需下载的资源
        _ab_download = new AssetBundleDownloader(_current_url);
        _ab_download.Start(DownLoadCommon.PATH, download_files);
        while (!_ab_download.IsDone)
        {
            UpdateCompleteValue(_ab_download.CompletedSize, _ab_download.TotalSize);
            yield return 0;
        }
        if (_ab_download.IsFailed)
        {
            Error(EmErrorCode.DownloadAssetBundleFailed);
            yield break;
        }


    }

    //<summary>
    //  比较AssetBundle差异，获得下载列表与删除列表
    //</summary>
    static void CompareAssetBundleDifference(ref List<string> download_files
                                            , ref List<string> delete_files
                                            , AssetBundleManifest old_manifest
                                            , AssetBundleManifest new_manifest
                                           )
    {
        if (download_files != null)
            download_files.Clear();
        if (delete_files != null)
            delete_files.Clear();

        if (old_manifest == null)
            return;
        if (new_manifest == null)
            return;

        //采用位标记的方式判断资源
        //位标记： 0： 存在旧资源中 1： 存在新资源中 2：本地资源标记
        int old_version_bit = 0x1;                      // 存在旧资源中
        int new_version_bit = 0x2;                      // 存在新资源中

        Dictionary<string, int> temp_dic = new Dictionary<string, int>();
        //标记旧资源
        string[] all_assetbundle = old_manifest.GetAllAssetBundles();
        for (int i = 0; i < all_assetbundle.Length; ++i)
        {
            string name = all_assetbundle[i];
            _SetDictionaryBit(ref temp_dic, name, old_version_bit);
        }
        //标记新资源
        string[] new_all_assetbundle = new_manifest.GetAllAssetBundles();
        for (int i = 0; i < new_all_assetbundle.Length; ++i)
        {
            string name = new_all_assetbundle[i];
            _SetDictionaryBit(ref temp_dic, name, new_version_bit);
        }


        //获得对应需操作的文件名， 优先级： both > add > delete
        //both: 第0位与第1位都被标记的
        //delete: 仅第0位被标记的
        //add: 第2位未标记，且第3位被标记的
        int both_bit = old_version_bit | new_version_bit;        // 二个版本资源都存在
        List<string> add_files = new List<string>();
        List<string> both_files = new List<string>();
        var itr = temp_dic.GetEnumerator();
        while (itr.MoveNext())
        {
            string name = itr.Current.Key;
            int mask = itr.Current.Value;
            if ((mask & new_version_bit) == new_version_bit
                && (mask & old_version_bit) == 0)
                add_files.Add(name);
            else if ((mask & both_bit) == both_bit)
                both_files.Add(name);
            else if ((mask & old_version_bit) == old_version_bit)
                delete_files.Add(name);
        }
        itr.Dispose();

        //载入下载缓存数据
        DownloadCache download_cache = new DownloadCache();
        download_cache.Load(DownLoadCommon.DOWNLOADCACHE_FILE_PATH);
        if (!download_cache.HasData())
            download_cache = null;

        //记录需下载的文件
        {
            //加入新增的文件
            download_files.AddRange(add_files);
            //比较所有同时存在的文件，判断哪些需要更新
            for (int i = 0; i < both_files.Count; ++i)
            {
                string name = both_files[i];
                string full_name = DownLoadCommon.GetFileFullName(name);
                if (File.Exists(full_name))
                {
                    //判断哈希值是否相等
                    string old_hash = old_manifest.GetAssetBundleHash(name).ToString();
                    string new_hash = new_manifest.GetAssetBundleHash(name).ToString();
                    if (old_hash.CompareTo(new_hash) == 0)
                        continue;
                    download_files.Add(name);
                }
                else
                {
                    download_files.Add(name);
                }
            }

            //过滤缓存中已下载的文件
            if (download_cache != null)
            {
                var cache_itr = download_cache.Data.AssetBundles.GetEnumerator();
                while (cache_itr.MoveNext())
                {
                    DownloadCacheData.AssetBundle elem = cache_itr.Current.Value;
                    string name = elem.AssetBundleName;
                    string full_name = DownLoadCommon.GetFileFullName(name);
                    if (File.Exists(full_name))
                    {
                        string cache_hash = elem.Hash;
                        string new_hash = new_manifest.GetAssetBundleHash(name).ToString();
                        if (!string.IsNullOrEmpty(cache_hash)
                                && cache_hash.CompareTo(new_hash) == 0)
                            download_files.Remove(name);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    static void _SetDictionaryBit(ref Dictionary<string, int> dic, string name, int bit)
    {
        if (!dic.ContainsKey(name))
        {
            dic.Add(name, bit);
        }
        else
        {
            dic[name] |= bit;
        }
    }
    #endregion

    #region CopyCacheFile
    /// <summary>
    ///   拷贝文件并覆盖旧数据文件
    /// </summary>
    IEnumerator StartCopyCacheFile()
    {
        Debug.Log("AssetUpdater:StartCopyCacheFile");
        if (ErrorCode != EmErrorCode.None)
            yield break;

        string str = DownLoadCommon.GetCacheFileFullName(DownLoadCommon.MAIN_MANIFEST_FILE_NAME);
        string dest = DownLoadCommon.GetFileFullName(DownLoadCommon.MAIN_MANIFEST_FILE_NAME);
        UpdateCompleteValue(1, 1);
        yield return DownLoadCommon.StartCopyFile(str, dest);
    }
    #endregion

    #region Dispose
    /// <summary>
    ///   清理
    /// </summary>
    IEnumerator StartDispose()
    {
        UpdateCompleteValue(0f, 1f);

        if (ErrorCode != EmErrorCode.None)
        {
            //缓存已下载内容,便于下次继续下载
            SaveDownloadCacheData();
        }
        else
        {
            //删除缓存目录
            if (Directory.Exists(DownLoadCommon.CACHE_PATH))
                Directory.Delete(DownLoadCommon.CACHE_PATH, true);

            //重启AssetBundleManager
            AssetBundleManager.GetInstance().Relaunch();
        }

        //ab包重新启动
        while (!AssetBundleManager.GetInstance().WaitForLaunch())
        {
            yield return null;
        }

        UpdateCompleteValue(1f, 1f);
        yield return 0;
    }
    #endregion

    #region Other
    /// <summary>
    /// 
    /// </summary>
    void Reset()
    {
        IsDone = false;
        ErrorCode = EmErrorCode.None;
        CurrentState = EmState.None;
        CurrentStateCompleteValue = 0f;
        CurrentStateTotalValue = 0f;
        _current_url = "";
    }

    /// <summary>
    ///   结束
    /// </summary>
    void Done()
    {
        Debug.Log("AssetUpdater:Done");
        IsDone = true;
        OnDoneEvent();
        //UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    /// <summary>
    ///   设置状态
    /// </summary>
    void UpdateState(EmState state)
    {
        CurrentState = state;
        OnUpdateEvent();
    }

    /// <summary>
    ///   更新完成度
    /// </summary>
    void UpdateCompleteValue(float current)
    {
        UpdateCompleteValue(current, CurrentStateTotalValue);
    }
    /// <summary>
    ///   更新完成度
    /// </summary>
    void UpdateCompleteValue(float current, float total)
    {
        CurrentStateCompleteValue = current;
        CurrentStateTotalValue = total;
        //Debug.Log("LoadPrecent: " + current / total);
        OnUpdateEvent();
    }

    /// <summary>
    ///   更新
    /// </summary>
    void OnUpdateEvent()
    {
        if (_onUpdate != null)
            _onUpdate(CurrentState.ToString(), CurrentStateCompleteValue, CurrentStateTotalValue);
    }

    /// <summary>
    ///   结束事件
    /// </summary>
    void OnDoneEvent()
    {
        if (_onDone != null)
            _onDone();
    }

    /// <summary>
    ///   错误
    /// </summary>
    void Error(EmErrorCode ec, string message = null)
    {
        ErrorCode = ec;

        string ms = string.IsNullOrEmpty(message) ?
            ErrorCode.ToString() : ErrorCode.ToString() + " - " + message;
        Debug.LogError(ms);
    }

    /// <summary>
    ///   写入下载缓存信息，用于断点续传
    /// </summary>
    void SaveDownloadCacheData()
    {
        if (CurrentState < EmState.UpdateAssetBundle)
            return;

        if (!Directory.Exists(DownLoadCommon.CACHE_PATH))
            return;

        //载入新的Manifest
        string new_manifest_name = DownLoadCommon.GetCacheFileFullName(DownLoadCommon.MAIN_MANIFEST_FILE_NAME);
        AssetBundleManifest new_manifest = DownLoadCommon.LoadMainManifestByPath(new_manifest_name);
        if (new_manifest == null)
            return;

        //先尝试读取旧的缓存信息，再保存现在已经下载的数据
        //PS:由于只有版本完整更新完才会移动Cache目录，且玩家可能多次尝试下载更新，所以必须保留旧的缓存信息
        DownloadCache cache = new DownloadCache();
        cache.Load(DownLoadCommon.DOWNLOADCACHE_FILE_PATH);
        if (_ab_download != null
            && _ab_download.CompleteDownloads != null
            && _ab_download.CompleteDownloads.Count > 0)
        {
            for (int i = 0; i < _ab_download.CompleteDownloads.Count; ++i)
            {
                string assetbundle_name = _ab_download.CompleteDownloads[i];
                Hash128 hash_code = new_manifest.GetAssetBundleHash(assetbundle_name);
                if (hash_code.isValid && !cache.Data.AssetBundles.ContainsKey(assetbundle_name))
                {
                    DownloadCacheData.AssetBundle elem = new DownloadCacheData.AssetBundle()
                    {
                        AssetBundleName = assetbundle_name,
                        Hash = hash_code.ToString(),
                    };
                    Debug.Log(cache.Data.AssetBundles.Count + " - Cache Add:" + assetbundle_name);
                    cache.Data.AssetBundles.Add(assetbundle_name, elem);
                }

            }
        }
        if (cache.HasData())
            cache.Save(DownLoadCommon.DOWNLOADCACHE_FILE_PATH);
    }
    #endregion

    #region MonoBehaviour
    /// <summary>
    ///   
    /// </summary>

    public void StartAssetUpdater(string url)
    {
        Reset();
        List<string> url_group = new List<string>();
        url_group.Add(url);
        this.StartUpdate(url_group);
    }
    #endregion
}