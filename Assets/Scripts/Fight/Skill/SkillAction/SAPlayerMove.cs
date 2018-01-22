using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 人物移动
/// </summary>
public class SAPlayerMove : SkillActionBase
{
    private MoveInfo _moveInfo;
    public override int ActFrame
    {
        get { return _actFrame; }
        set
        {
            _curFrame = value;
            _actFrame = value;
            _frameMax = _actFrame + _moveInfo.FrameCount;
        }
    }
  
    public SAPlayerMove(MoveInfo moveInfo, SkillActionParser actionParser, int actFrame)
        : base(actionParser, actFrame)
    {
        _moveInfo = moveInfo;
        ActionType = SkillDefine.SkillActionType.PLAY_ANIM;
        _frameMax = _actFrame + _moveInfo.FrameCount;
    }

    //移动
    public override void UpdateActoin(int curFrame = 0)
    {
        base.UpdateActoin(curFrame);
        if (_dtFrame > 0)
        {
            bool moveDone = SkillMethod.MoveAction(_skillPlayer, _moveInfo, _actionParser.Operate, _dtFrame);
            if (moveDone)
            {
                Complete();
            }
        }
    }


}