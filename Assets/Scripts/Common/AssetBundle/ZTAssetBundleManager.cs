using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ZTAssetBundleManager : MonoSingleton<AssetBundleManager>
{
    /// <summary>
    ///   主AssetBundleMainfest
    /// </summary>
    public AssetBundleManifest MainManifest { get; private set; }

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

    public EmErrorCode ErrorCode { get; private set; }

    void Error(EmErrorCode ec, string message = null)
    {
        ErrorCode = ec;

        StringBuilder sb = new StringBuilder("[AssetBundleError] - ");
        sb.Append(ErrorCode.ToString());
        if (!string.IsNullOrEmpty(message)) { sb.Append("\n"); sb.Append(message); }
        Debug.LogWarning(sb.ToString());
    }

    /// <summary>
    ///   常驻的AssetBundle
    /// </summary>
    private Dictionary<string, ZTAssetBundle> _assetbundlePermanent;

    /// <summary>
    /// 临时的AssetBundle
    /// </summary>
    private Dictionary<string, ZTAssetBundle> _assetbundleTemporary;


    public void Relaunch()
    {
      
        ShutDown();
        Launch();
    }

    /// <summary>
    ///   启动(仅内部启用)
    /// </summary>
    void Launch()
    {
        if (this._assetbundlePermanent == null)
            this._assetbundlePermanent = new Dictionary<string, ZTAssetBundle>();
       
        if (this._assetbundleTemporary == null)
            _assetbundleTemporary = new Dictionary<string, ZTAssetBundle>();

    }

    /// <summary>
    /// 关闭
    /// </summary>
    void ShutDown()
    {
        UnloadAllAssetBundle();
    }

    /// <summary>
    ///   释放常驻的AssetBundle
    /// </summary>
    public void UnloadAllAssetBundle()
    {
        UnloadAssetBundleByDic(this._assetbundlePermanent);
        UnloadAssetBundleByDic(this._assetbundleTemporary);
    }

    /// <summary>
    /// 释放AssetBundle
    /// </summary>
    void UnloadAssetBundleByDic(Dictionary<string, ZTAssetBundle> assetbundleDic)
    {
        var itr = assetbundleDic.Values.GetEnumerator();
        while (itr.MoveNext())
        {
            itr.Current.Destroy();
        }
        itr.Dispose();
        assetbundleDic.Clear();
    }

    /// <summary>
    ///   释放指定的AssetBundle
    /// </summary>
    public void UnloadAssetBundle(string assetbundlename)
    {
        ZTAssetBundle ab = null;
        if (this._assetbundlePermanent.TryGetValue(assetbundlename, out ab))
            this._assetbundlePermanent.Remove(assetbundlename);
        else if (this._assetbundleTemporary.TryGetValue(assetbundlename, out ab))
            this._assetbundleTemporary.Remove(assetbundlename);
        ab.Destroy();
    }

    /// <summary>
    /// 获取 缓存中的 ab包
    /// </summary>
    ZTAssetBundle LoadInCahce(string assetbundlename)
    {
        ZTAssetBundle ab = null;
        if (this._assetbundlePermanent.ContainsKey(assetbundlename))
        {
            ab = this._assetbundlePermanent[assetbundlename];
        }
        else if (this._assetbundleTemporary.ContainsKey(assetbundlename))
        {
            ab = this._assetbundleTemporary[assetbundlename];
        }
        return ab;
    }
    /// <summary>
    /// 根据预定义 存储ab包
    /// </summary>
    void SaveInCahce(string assetbundlename, ZTAssetBundle ab)
    {
         this._assetbundleTemporary.Add(assetbundlename, ab);
    }
    /// <summary>
    /// 同步加载资源
    /// </summary>
    public void LoadAssetInBundle(string assetbundlename, string assetname, System.Action<Object> callback, System.Type type = null)
    {
        if (assetbundlename == null)
            callback(null);
        if (MainManifest == null)
            callback(null);
        ZTAssetBundle ab = LoadAssetBundleAndDependencies(assetbundlename);
        callback(ab.GetAsset(assetname,type));
    }

    /// <summary>
    ///   加载有依赖的AssetBundle
    /// </summary>
    public ZTAssetBundle LoadAssetBundleAndDependencies(string assetbundlename)
    {
        if (assetbundlename == null)
            return null;
        if (MainManifest == null)
            return null;

        string[] deps = MainManifest.GetAllDependencies(assetbundlename);
        for (int index = 0; index < deps.Length; index++)
        {
            //加载所有的依赖AssetBundle
            if (LoadZTAssetBundle(deps[index]) == null)
            {
                Debug.LogWarning(assetbundlename + "'s Dependencie AssetBundle can't find. Name is " + deps[index] + "!");
                return null;
            }

        }

        return LoadZTAssetBundle(assetbundlename);
    }
    /// <summary>
    ///   加载AssetBundle
    /// </summary>
    ZTAssetBundle LoadZTAssetBundle(string assetbundlename)
    {
        if (assetbundlename == null || MainManifest == null)
            return null;

        ZTAssetBundle ab = LoadInCahce(assetbundlename);
        if (null != ab)
            return ab;
     
        ab = new ZTAssetBundle(assetbundlename);
        SaveInCahce(assetbundlename, ab);

        return ab;
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    public void LoadAssetInBundleSync(string assetbundlename, string assetname, System.Action<Object> callback, System.Type type = null)
    {
        if (assetbundlename == null)
            callback(null);
        if (MainManifest == null)
            callback(null);
        ZTAssetBundle ab = LoadAssetBundleAndDependencies(assetbundlename);
        StartCoroutine(ab.GetAssetSync(assetname,callback,type));
    }




    /// <summary>
    ///   初始化(检测资源完整性)
    /// </summary>
    IEnumerator CheckInitAsset()
    {
        //创建资源根目录
        if (!Directory.Exists(DownLoadCommon.PATH))
            Directory.CreateDirectory(DownLoadCommon.PATH);

        //判断主资源文件 和 初始化结束资源 是否存在，不存在则拷贝备份资源至资源根目录
        string resources_manifest_file = DownLoadCommon.GetFileFullName(DownLoadCommon.MAIN_MANIFEST_FILE_NAME);
        string resources_end_File = DownLoadCommon.GetFileFullName(DownLoadCommon.END_RESOUCES_FILE_NAME);
        if (!File.Exists(resources_manifest_file) || !File.Exists(resources_end_File))
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
        this.MainManifest = DownLoadCommon.LoadMainManifest();

        if (this.MainManifest == null)
        {
            Error(EmErrorCode.LoadMainManifestFailed
                , "Can't load MainManifest file!");
            return false;
        }

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

        string mainifestFullName = DownLoadCommon.GetFileFullName(DownLoadCommon.MAIN_MANIFEST_FILE_NAME);
        AssetBundleManifest initial = DownLoadCommon.LoadMainManifestByPath(mainifestFullName);
        if (initial == null)
            yield break;
        //拷贝AssetBundle文件
        string[] all_assetbundle = initial.GetAllAssetBundles();
        for (int i = 0; i < all_assetbundle.Length; ++i)
        {
            string name = all_assetbundle[i];
            yield return DownLoadCommon.StartCopyInitialFile(name);
        }

        //拷贝结束资源 保证安装包拷贝结束
        yield return DownLoadCommon.StartCopyInitialFile(DownLoadCommon.END_RESOUCES_FILE_NAME);
    }

}
