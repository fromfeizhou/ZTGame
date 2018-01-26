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
    
    public SAColliderMove(MoveInfo moveInfo, CollBase collider, ColliderInfo collidInfo, SkillActionParser actionParser, int actFrame)
        : base(collider, collidInfo,actionParser, actFrame)
    {
        _moveInfo = moveInfo;
        _moveCount = 0;

        ActionType = SkillDefine.SkillActionType.COLLIDER_MOVE;
    }

    //刷新对象
    public override void UpdateActoin(int curFrame = 0)
    {
        base.UpdateActoin(curFrame);
        
        if (_moveCount > _moveInfo.FrameCount)
        {
            return;
        }
        _moveCount++;
      
        bool moveDone = SkillMethod.MoveAction(_collider, _moveInfo, _actionParser.Command);
        if (moveDone)
        {
            Complete();
        }
    }
}
