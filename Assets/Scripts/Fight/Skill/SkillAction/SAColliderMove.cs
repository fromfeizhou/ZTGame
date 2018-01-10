using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 移动碰撞
/// </summary>
public class SAColliderMove : SACollider
{
    private MoveInfo _moveInfo;
    private int _moveCount;
    public SAColliderMove(MoveInfo moveInfo, SkillDefine.ColliderTarget colliderTarget, ColliderData collider, SkillActionParser actionParser, int actFrame)
        : base(colliderTarget, collider, actionParser,actFrame)
    {
        _moveInfo = moveInfo;
        _moveCount = 0;
    }

    //刷新对象
    public override void UpdateActoin(int curFrame = 0)
    {
        base.UpdateActoin(curFrame);
        
        if (_moveCount > _moveInfo.FrameCount)
        {
            return;
        }
        int dtFrame = _dtFrame;
        _moveCount += _dtFrame;
        if (_moveCount > _moveInfo.FrameCount)
        {
            //减去溢出部分 
            dtFrame -= _moveCount - _moveInfo.FrameCount;
        }

        if (dtFrame > 0)
        {
            bool moveDone = SkillMethod.MoveAction(_collider.Collider, _moveInfo, _actionParser.Operate, dtFrame);
            if (moveDone)
            {
                Complete();
            }
        }
    }
}
