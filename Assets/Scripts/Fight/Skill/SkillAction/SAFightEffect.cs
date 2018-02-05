using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//自己对自己使用效果(废弃不用 通过碰撞来触发)
public class SAFightEffect : SkillActionBase
{
	public List<FightEffectInfo> EffectList;

    public SAFightEffect(List<FightEffectInfo> list, SkillActionParser actionParser, int actFrame)
        : base(actionParser, actFrame)
    {
        Debug.LogError("自己对自己使用效果(废弃不用 通过碰撞来触发)");
        EffectList = list;
    }

    //播放动作
    public override void UpdateActoin(int curFrame = 0)
    {
        base.UpdateActoin(curFrame);
        DoAction();
    }

    protected override void DoAction()
    {
        for (int i = 0; i < EffectList.Count; i++)
        {
            FightEffectDefine.ParseEffect(_skillPlayer, EffectList[i], _skillPlayer.BattleId);
        }
        Complete();
    }
}
