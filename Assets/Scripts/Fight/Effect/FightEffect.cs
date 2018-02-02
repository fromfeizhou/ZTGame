using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightEffect{
    public FIGHT_EF_TPYE FightEffectType = FIGHT_EF_TPYE.NONE;  //效果类型
    public List<int> Params;        //参数
    public int UserId;              //施法者

    public void Parse(string effectStr)
    {
        string[] result = effectStr.Split(',');
        if (result.Length > 0)
        {
            FightEffectType = (FIGHT_EF_TPYE)int.Parse(result[0]);
        }
        Params = new List<int>();
        for (int i = 1; i < result.Length; i++)
        {
            Params.Add(int.Parse(result[i]));
        }
    }
}
