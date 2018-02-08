using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 播放动作
/// </summary>
public class SAEffect : SkillActionBase
{
    public EffectInfo EffectData;

    public SAEffect(EffectInfo info, SkillActionParser actionParser, uint actFrame)
        : base(actionParser, actFrame)
    {
        EffectData = info;
    }

    //播放动作
    public override void UpdateActoin(uint curFrame = 0)
    {
        base.UpdateActoin(curFrame);
        DoAction();
    }

    protected override void DoAction()
    {
        if (EffectData.Id != "" && null != _skillPlayer)
        {
            ICharaActor chara = _skillPlayer as ICharaActor;
            if (null != chara)
            {
                chara.AddEffect(EffectData);
            }
        }
        Complete();
    }
}
