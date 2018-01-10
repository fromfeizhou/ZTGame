/***************************************************************
 * Copyright 2016 By Zhang Minglin
 * Author: Zhang Minglin
 * Create: 2016/01/18 16:21:22
 * Note  : AssetBundle下载器
***************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
///   AssetBundle下载器
/// </summary>
public class AssetBundleDownloader
{
    /// <summary>
    ///   并发下载最大数量
    ///   如果需要>2，则需修改System.Net.ServicePointManager.DefaultConnectionLimit
    /// </summary>
    public const int CONCURRENCE_DOWNLOAD_NUMBER = 2;

    /// <summary>
    ///   下载根路径
    /// </summary>
    public string Root;

    /// <summary>
    ///   URL
    /// </summary>
    public string URL;

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
    ///   下载的大小
    /// </summary>
    public long CompletedSize { get; private set; }

    /// <summary>
    ///   总大小
    /// </summary>
    public long TotalSize { get; private set; }

    /// <summary>
    ///   需要下载的资源
    /// </summary>
    public List<string> ImcompleteDownloads { get; private set; }

    /// <summary>
    ///   已下载的资源
    /// </summary>
    public List<string> CompleteDownloads { get; private set; }

    /// <summary>
    ///   下载失败的资源
    /// </summary>
    public List<string> FailedDownloads { get; private set; }

    /// <summary>
    ///   http下载
    /// </summary>
    private List<HttpAsyDownload> downloads_ = new List<HttpAsyDownload>();


    /// <summary>
    ///   锁对象，用于保证多线程下载安全
    /// </summary>
    object lock_obj_ = new object();

    /// <summary>
    ///   下载资源
    /// </summary>
    public AssetBundleDownloader(string url
        , int concurrence_download_number = CONCURRENCE_DOWNLOAD_NUMBER)
    {
        URL = url;
        IsDone = false;
        ErrorCode = EmErrorCode.None;
        CompletedSize = 0;
        TotalSize = 0;
        ImcompleteDownloads = new List<string>();
        CompleteDownloads = new List<string>();
        FailedDownloads = new List<string>();

        System.Net.ServicePointManager.DefaultConnectionLimit = concurrence_download_number;
    }

    /// <summary>
    /// 开始下载
    /// </summary>
    public bool Start(string root, string assetbundlename)
    {
        List<string> list = new List<string>();
        list.Add(assetbundlename);
        return Start(root,list);
    }

    /// <summary>
    ///   开始下载
    /// </summary>
    public bool Start(string root,List<string> assetbundles)
    {
        Abort();

        InitializeDownload(root,assetbundles);
        UpdateState();
        DownloadAll();

        return true;
    }

    /// <summary>
    ///   取消下载
    /// </summary>
    public void Cancel()
    {
        for (int i = 0; i < downloads_.Count; ++i)
        {
            downloads_[i].Cancel();
        }
    }

    /// <summary>
    ///   终止下载
    /// </summary>
    public void Abort()
    {
        for (int i = 0; i < downloads_.Count; ++i)
        {
            downloads_[i].Abort();
        }
    }

    /// <summary>
    /// 初始化下载信息
    /// </summary>
    void InitializeDownload(string root,List<string> assetbundles)
    {
        Root = root;
        ImcompleteDownloads = assetbundles;

        IsDone = false;
        ErrorCode = EmErrorCode.None;
        CompleteDownloads.Clear();
        FailedDownloads.Clear();

        if (ImcompleteDownloads == null) ImcompleteDownloads = new List<string>();

        //统计数据
        TotalSize = 0;
        CompletedSize = 1000;
        //for (int i = 0; i < ImcompleteDownloads.Count; ++i)
        //{
        //    var ab = resources_manifest_.Find(ImcompleteDownloads[i]);
        //    if (ab != null)
        //    {
        //        TotalSize += ab.Size;
        //    }
        //}
    }


    /// <summary>
    ///   是否正在下载
    /// </summary>
    public bool IsDownLoading(string file_name)
    {
        HttpAsyDownload ad = downloads_.Find(delegate(HttpAsyDownload d)
        {
            return d.LocalName == file_name;
        });

        return ad != null;
    }

    /// <summary>
    /// 获得或创建一个闲置的下载
    /// </summary>
    HttpAsyDownload GetIdleDownload(bool is_create)
    {
        lock (lock_obj_)
        {
            for (int i = 0; i < downloads_.Count; ++i)
            {
                //还在下载队列列的 httpload（状态虽然可用 但逻辑还没处理完）
                if (downloads_[i].IsDone && !ImcompleteDownloads.Contains(downloads_[i].LocalName))
                    return downloads_[i];
            }

            if (is_create)
            {
                if (downloads_.Count < System.Net.ServicePointManager.DefaultConnectionLimit)
                {
                    HttpAsyDownload d = new HttpAsyDownload(URL);
                    downloads_.Add(d);
                    return d;
                }
            }
            return null;
        }
    }

    /// <summary>
    ///   下载所有资源
    /// </summary>
    void DownloadAll()
    {
        lock (lock_obj_)
        {
            //下载
            for (int i = 0; i < ImcompleteDownloads.Count; ++i)
            {
                if (!Download(ImcompleteDownloads[i]))
                    break;
            }
        }
    }

    /// <summary>
    ///   更新
    /// </summary>
    void UpdateState()
    {
        IsDone = ImcompleteDownloads.Count == 0;
        ErrorCode = FailedDownloads.Count > 0 ? EmErrorCode.DownloadFailed : ErrorCode;
    }

    /// <summary>
    ///   下载
    /// </summary>
    bool Download(string assetbundlename)
    {
        lock (lock_obj_)
        {
            //var ab = resources_manifest_.Find(assetbundlename);
            //if (ab == null)
            //{
            //    Debug.LogWarning("AssetBundleDownloader.Download - AssetBundleName is invalid.");
            //    return true;
            //}

            //string file_name = ab.IsCompress ?
            //    Compress.GetCompressFileName(assetbundlename) : assetbundlename;
            if (!IsDownLoading(assetbundlename))
            {
                HttpAsyDownload d = GetIdleDownload(true);
                if (d == null)
                    return false;
                Debug.Log("AssetBundleDownloader: " + assetbundlename);
                d.Start(Root, assetbundlename, _DownloadNotify, _DownloadError);
            }
            return true;
        }
    }

    /// <summary>
    /// 下载完成
    /// </summary>
    void DownloadSucceed(string file_name)
    {
        lock (lock_obj_)
        {
            //bool is_compress = Compress.IsCompressFile(file_name);
            //string assetbundle = is_compress ?
            //    Compress.GetDefaultFileName(file_name) : file_name;

            if (ImcompleteDownloads.Contains(file_name))
                ImcompleteDownloads.Remove(file_name);
            CompleteDownloads.Add(file_name);

            ////判断是否需要解压文件
            //if (is_compress)
            //{
            //    // 解压文件
            //    string in_file = Root + "/" + file_name;
            //    string out_file = Root + "/" + assetbundle;
            //    Compress.DecompressFile(in_file, out_file);
            //    // 删除压缩包
            //    System.IO.File.Delete(in_file);
            //}
        }
    }

    /// <summary>
    ///   
    /// </summary>
    void _DownloadNotify(HttpAsyDownload d, long size)
    {
        lock (lock_obj_)
        {
            if (d.IsDone)
            {
                DownloadSucceed(d.LocalName);
                DownloadAll();
            }
            CompletedSize += size;
            UpdateState();
        }
    }

    /// <summary>
    ///   
    /// </summary>
    void _DownloadError(HttpAsyDownload d)
    {
        lock (lock_obj_)
        {
            Debug.Log(d.LocalName);
            //从未下载列表中移除
            if (ImcompleteDownloads.Contains(d.LocalName))
                ImcompleteDownloads.Remove(d.LocalName);
            //加入失败列表
            FailedDownloads.Add(d.LocalName);
            DownloadAll();
            UpdateState();
        }
    }
}