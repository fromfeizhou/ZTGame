using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 播放动作
/// </summary>
public class SAPlayAnim : SkillActionBase
{
    public string AnimName = "";

    public SAPlayAnim(string animName, SkillActionParser actionParser, int actFrame)
        : base(actionParser,actFrame)
    {
        AnimName = animName;
        ActionType = SkillDefine.SkillActionType.PLAY_ANIM;
    }

     //播放动作
    public override void UpdateActoin(int curFrame = 0)
    {
        base.UpdateActoin(curFrame);
        DoAction();
    }

    protected override void DoAction()
    {
        if (AnimName != "" && null != _skillPlayer)
        {
            _skillPlayer.PlayAction(AnimName);
        }
        Complete();
    }
}
