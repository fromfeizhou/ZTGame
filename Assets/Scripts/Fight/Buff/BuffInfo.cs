using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffInfo{

    public int BuffId;      //buffId
    public BUFF_TYPE BuffType;    //buff类型
    public EffectInfo EffectInfo;   //特效
    
    public int StartTime;          //创建时间
    public int IntervalTime;        //触发间隔
    public int LifeTime;            //总时间
    public int CountTime;           //存在时间

    public BuffInfo(int buffId,int frame)
    {
        BuffId = buffId;
        BuffType = BUFF_TYPE.NORMAL;
        
        StartTime = frame;
        IntervalTime = 30;
        LifeTime = frame + 300;
        CountTime = 0;
    }

    //创建
    public virtual void Start()
    {
    }

    //过程
    public virtual void Process()
    {
    }

    //处理buff 事件
    public virtual void DoActoin()
    {
    }
}
