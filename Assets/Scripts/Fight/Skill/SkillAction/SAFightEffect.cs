using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAFightEffect : SkillActionBase
{
	public List<FightEffectInfo> EffectList;

    public SAFightEffect(List<FightEffectInfo> list, SkillActionParser actionParser, int actFrame)
        : base(actionParser, actFrame)
    {
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
            FightEffectDefine.ParseEffect(_skillPlayer, new FightEffect(EffectList[i], _skillPlayer.BattleId));
        }
        Complete();
    }
}
