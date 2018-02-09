using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 人物移动
/// </summary>
public class SAPlayerMove : SkillActionBase
{
    private MoveInfo _moveInfo;
    private int _moveCount;

    public override uint ActFrame
    {
        get { return _actFrame; }
        set
        {
            _curFrame = value;
            _actFrame = value;
            _frameMax = _actFrame + _moveInfo.FrameCount;
        }
    }

    public SAPlayerMove(MoveInfo moveInfo, SkillActionParser actionParser, uint actFrame)
        : base(actionParser, actFrame)
    {
        _moveInfo = moveInfo;
        ActionType = SkillDefine.SkillActionType.PLAY_ANIM;
        _frameMax = _actFrame + _moveInfo.FrameCount;
        _moveCount = 0;
    }

    //移动
    public override void UpdateActoin(uint curFrame = 0)
    {
        base.UpdateActoin(curFrame);
        if (_moveCount > _moveInfo.FrameCount)
        {
            return;
        }
        _moveCount++;
        bool moveDone = SkillMethod.MoveAction(_skillPlayer, _moveInfo, _actionParser.Command.SkillDir, _actionParser.Command.TargetPos,_skillPlayer.MovePos);
        if (moveDone)
        {
            Complete();
        }
    }


}