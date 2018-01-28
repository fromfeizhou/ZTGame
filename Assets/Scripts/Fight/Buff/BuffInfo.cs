using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffInfo:NotificationDelegate{

    public int BuffId;      //buffId
    public BUFF_TYPE BuffType;    //buff类型
    public EffectInfo EffectInfo;   //特效
    
    public int StartTime;          //创建时间
    public int IntervalTime;        //触发间隔
    public int MaxTime;             //总时间
    public int LifeTime;            //存在时间

    public BuffInfo(int buffId,int frame)
    {
        BuffId = buffId;
        BuffType = BUFF_TYPE.NORMAL;
        
        StartTime = frame;
        LifeTime = frame;
        IntervalTime = 30;
        MaxTime = frame + 300;
    }

    //创建
    public virtual void Start()
    {

    }

    //过程
    public virtual void Update()
    {
        if (MaxTime == -1) return;
        LifeTime++;
        if (LifeTime > MaxTime)
        {
            return;
        }

    }

    //处理buff 事件
    public virtual void DoActoin()
    {
    }

    public virtual void Destroy()
    {
    }
}
