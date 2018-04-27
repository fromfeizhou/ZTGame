using System;
using System.Collections.Generic;
using UnityEngine;

public class PiecewiseLoader
{
    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

    private float intervalTime;
    private float loadTime;
    private List<Action> callbackList=new List<Action>();

    private float lastIntervalTime = 0f;  

    public PiecewiseLoader(float intervalTime, float loadTime)
    {
        this.intervalTime = intervalTime;
        this.loadTime = loadTime;
        lastIntervalTime = Time.realtimeSinceStartup;
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

    private float tempCurTime;
    private double tempLoadTime;
    public void UpdateLoader()
    {
        tempCurTime = Time.realtimeSinceStartup;
        if (tempCurTime - lastIntervalTime >= intervalTime)
        {
            lastIntervalTime = tempCurTime;
            tempLoadTime = 0;
            
        }

        if (tempLoadTime <= loadTime&&callbackList.Count>0)
        {

            Action callback = callbackList[0];
            stopwatch.Reset();
            stopwatch.Start();
            callback();
            stopwatch.Stop();
            System.TimeSpan timespan = stopwatch.Elapsed;
            double milliseconds = timespan.TotalMilliseconds;  //  总毫秒数
          
            //Debug.LogError("UseTime++++++++++++++++++" + milliseconds);
            tempLoadTime +=  milliseconds;
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
