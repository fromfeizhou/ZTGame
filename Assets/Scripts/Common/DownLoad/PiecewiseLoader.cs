using System;
using System.Collections.Generic;
using UnityEngine;

public class PiecewiseLoader
{
    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

    private float intervalTime;
    private int waitTime;
    private List<Func<bool>> callbackList=new List<Func<bool>>();

    /// <summary>
    /// 单位 秒 s
    /// </summary>
    /// <param name="intervalTime"></param>
    /// <param name="loadTime"></param>
    public PiecewiseLoader(float intervalTime)
    {
        this.intervalTime = 0.028f;//intervalTime;45帧
        this.waitTime = 0;
        PiecewiseLoaderMgr.GetInstance().AddLoaders(this);
    }

    public void PushCallBack(Func<bool> callback)
    {
        callbackList.Add(callback);
    }

    ~PiecewiseLoader()
    {
        PiecewiseLoaderMgr.GetInstance().RemoveLoader(this);
    }

    public void UpdateLoader(float time)
    {
        if (waitTime > 0)
        {
            waitTime--;
            return;
        }
        waitTime = Mathf.RoundToInt(time / intervalTime);
        while ( callbackList.Count > 0)
        {
            bool isEnd = callbackList[0]();
            callbackList.RemoveAt(0);
            if (isEnd)
            {
                break;
            }
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
