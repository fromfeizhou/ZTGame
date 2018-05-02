using System;
using System.Collections.Generic;
using UnityEngine;

public class PiecewiseLoader
{
    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

    private float intervalTime;
    private List<Action> callbackList=new List<Action>();

    /// <summary>
    /// 单位 秒 s
    /// </summary>
    /// <param name="intervalTime"></param>
    /// <param name="loadTime"></param>
    public PiecewiseLoader(float intervalTime)
    {
        this.intervalTime = 1 / 45f;//intervalTime;45帧
        PiecewiseLoaderMgr.GetInstance().AddLoaders(this);
    }

    public void PushCallBack(Action callback)
    {
        callbackList.Add(callback);
    }

    ~PiecewiseLoader()
    {
        PiecewiseLoaderMgr.GetInstance().RemoveLoader(this);
    }

    public void UpdateLoader(float time)
    {
        if (time > intervalTime)
            return;
        if ( callbackList.Count > 0)
        {
            Action callback = callbackList[0];
            callback();
            callbackList.RemoveAt(0);
        }
    }
    public void Clear()
    {
        callbackList.Clear();
    }


    public void Release()
    {
        callbackList.Clear();
    }
}
