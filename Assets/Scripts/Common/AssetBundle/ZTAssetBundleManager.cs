using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ZTAssetBundleManager : MonoSingleton<AssetBundleManager>
{

    /// <summary>
    ///   常驻的AssetBundle
    /// </summary>
    private Dictionary<string, ZTAssetBundle> _assetbundlePermanent;

    /// <summary>
    ///   缓存的AssetBundle
    /// </summary>
    private Dictionary<string, ZTAssetBundle> _assetbundleCache;

    /// <summary>
    /// 临时的AssetBundle
    /// </summary>
    private Dictionary<string, ZTAssetBundle> _assetbundleTemporary;


    public bool Relaunch()
    {
      
        ShutDown();
        Launch();
        return true;
    }

    /// <summary>
    ///   判断一个AssetBundle是否存在文件中
    /// </summary>
    public bool IsExist(string assetbundlename)
    {
        if (string.IsNullOrEmpty(assetbundlename))
            return false;

        return File.Exists(DownLoadCommon.GetFileFullName(assetbundlename));
    }
   
    /// <summary>
    ///   释放常驻的AssetBundle
    /// </summary>
    public void UnloadAllAssetBundle()
    {
        UnloadAssetBundleByDic(this._assetbundlePermanent);
        UnloadAssetBundleByDic(this._assetbundleCache);
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
    ///   释放缓存的AssetBundle
    /// </summary>
    public void UnloadAssetBundle(string assetbundlename)
    {
        ZTAssetBundle ab = null;
        if (this._assetbundlePermanent.TryGetValue(assetbundlename, out ab))
            this._assetbundlePermanent.Remove(assetbundlename);
        else if (this._assetbundleCache.TryGetValue(assetbundlename, out ab))
            this._assetbundleCache.Remove(assetbundlename);
        else if (this._assetbundleTemporary.TryGetValue(assetbundlename, out ab))
            this._assetbundleTemporary.Remove(assetbundlename);
        ab.Destroy();
    }

    /// <summary>
    ///   启动(仅内部启用)
    /// </summary>
    void Launch()
    {
        if (this._assetbundlePermanent == null)
            this._assetbundlePermanent = new Dictionary<string, ZTAssetBundle>();
        if (this._assetbundleCache == null)
            this._assetbundleCache = new Dictionary<string, ZTAssetBundle>();
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




}
