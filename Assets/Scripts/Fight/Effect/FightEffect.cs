using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightEffect{
    public FightEffectInfo Info;        //参数
    public int UserId;              //施法者 外部传递
    public object TakeParam;        //外部传递参数

    public void FightEffect(FightEffectInfo info,int userId = -1,object takeParam = null)
    {
        Info = info;
        UserId = userId;
        TakeParam = takeParam;
    }
}
