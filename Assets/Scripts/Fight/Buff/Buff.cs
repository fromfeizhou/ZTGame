using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff{

    public int BuffId;      //buffId
    public EffectInfo BuffEffectInfo;   //特效

    public int BuffType;    //buff类型
    
    public int StartTime;          //创建时间
    public int IntervalTime;        //触发间隔
    public int MaxTime;             //总时间 -1 无上限
    public int LifeTime;            //存在时间

    public int UserId;
    public int MixMax;          //叠加上限 -1:无上限，1一次

    public bool IsStart;        //buff启动
    public bool IsExit;         //是否结束效果

    public Buff(BuffData buffData)
    {
        BuffId = buffData.BuffId;

        BuffEffectInfo = new EffectInfo("27_RFX_Magic_FlameSwirl1");
        
        BuffType = (int)BUFF_TYPE.NORMAL;

        StartTime = buffData.Frame;
        LifeTime = buffData.Frame;
        IntervalTime = 30;
        MaxTime = buffData.Frame + 300;

        UserId = buffData.UserId;
        MixMax = 1;


        IsExit = false;
    }

    //创建
    public virtual void Start()
    {
        IsStart = true;
    }

    //过程
    public virtual void Update()
    {
        if (!IsStart || IsExit) return;
        LifeTime++;
        //持续时间 计时判断
        if (MaxTime != -1 && LifeTime > MaxTime)
        {
            IsExit = true;
        }
        if (!IsExit && LifeTime > 0 && LifeTime % IntervalTime == 0)
        {
            DoActoin();
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
