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
        : base(collider, collidInfo, actionParser, actFrame)
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
        Vector3 targetPos = _actionParser.Command.TargetPos;
        if (_actionParser.Command.TargetId > 0)
        {
            ICharaBattle battleInfo = ZTSceneManager.GetInstance().GetCharaById(_actionParser.Command.TargetId) as ICharaBattle;
            if (null != battleInfo)
            {
                targetPos = battleInfo.MovePos;
            }
        }
        bool moveDone = SkillMethod.MoveAction(_collider, _moveInfo, _actionParser.Command.SkillDir, targetPos);
        if (moveDone)
        {
            DoneEndMoveAction();
            Complete();
        }
    }

    public void DoneEndMoveAction()
    {
        //指定目标
        if (_colliderInfo.ColliderType == CollBase.ColType.TARGET)
        {
            ICharaBattle battleInfo = ZTSceneManager.GetInstance().GetCharaById(_actionParser.Command.TargetId) as ICharaBattle;
            if (null != battleInfo)
            {
                DoColliderAction(battleInfo);
            }
        }
    }
}
