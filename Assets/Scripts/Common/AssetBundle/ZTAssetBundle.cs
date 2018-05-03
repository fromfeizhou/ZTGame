using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ab包加载 读取
/// 注意：单一ab包内 所有资源名字不能相同，否则取出资源不准确（本ab管理 不提供传递类型 加载资源接口）
/// </summary>
public class ZTAssetBundle
{

    public string AbPath;       //ab包名
    public bool IsLoadAbDone;   //加载结束
    private AssetBundle _assetBundle;   //ab包
    private Dictionary<string, Object> _assetDic;   //已经读取资源

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="abName"></param>
    public ZTAssetBundle(string abName)
    {
        this.AbPath = DownLoadCommon.GetFileFullName(abName); ;
        this._assetDic = new Dictionary<string, Object>();
         this.Load();
    }

    /// <summary>
    ///  删除
    /// </summary>
    public void Destroy()
    {
        this._assetDic = null;
        if (null != this._assetBundle)
        {
            this._assetBundle.Unload(true);
        }
        this._assetBundle = null;

    }

    /// <summary>
    /// 同步加载资源
    /// </summary>
    public Object GetAsset(string assetName, System.Type type = null)
    {
        if (this._assetDic.ContainsKey(assetName))
        {
            return this._assetDic[assetName];
        }

        if (null != this._assetBundle)
        {
            if(null != type)
            {
                this._assetDic.Add(assetName, this._assetBundle.LoadAsset(assetName,type));
            }
            else
            {
                this._assetDic.Add(assetName, this._assetBundle.LoadAsset(assetName));
            }
            return this._assetDic[assetName];
        }
        return null;
    }

    /// <summary>
    /// 同步加载ab包 
    /// </summary>
    private void Load()
    {
        if (System.IO.File.Exists(this.AbPath))
        {
            this._assetBundle = AssetBundle.LoadFromFile(this.AbPath);
        }
    }

    ///// <summary>
    ///// 异步加载ab包
    ///// </summary>
    //IEnumerator LoadSync()
    //{
    //    AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(this.AbPath);
    //    while (!abcr.isDone)
    //    {
    //        yield return null;
    //    }

    //    this._assetBundle = abcr.assetBundle;
    //    this.IsLoadAbDone = true;
    //}

    /// <summary>
    /// 异步加载资源
    /// </summary>
    public IEnumerator GetAssetSync(string assetName, System.Action<Object> callback, System.Type type = null)
    {
        if (this._assetBundle == null)
        {
            callback(null);
            yield break;
        }
        if (this._assetDic.ContainsKey(assetName))
        {
            Object go = this._assetDic[assetName];
            callback(go);
            yield break;
        }

        AssetBundleRequest request;
        if (null != type)
        {
            request = this._assetBundle.LoadAssetAsync(assetName, type);
        }
        else
        {
            request = this._assetBundle.LoadAssetAsync(assetName);
        }

        while (!request.isDone)
        {
            yield return null;
        }
        this._assetDic.Add(assetName, request.asset);
        callback(request.asset);
    }



}