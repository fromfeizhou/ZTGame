using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMethod {

    public static bool MoveAction(IMove target,MoveInfo moveInfo,SkillCommand command,int dtFrame = 0)
    {
        switch (moveInfo.MoveType)
        {
            case SkillDefine.MoveType.LINE:
                return UpdateLine(target, moveInfo, command, dtFrame);
            case SkillDefine.MoveType.LINE_TARGET:
                return UpdateLineTarget(target, moveInfo, command, dtFrame);
            case SkillDefine.MoveType.PLAYER:
                break;
            case SkillDefine.MoveType.ROTATE:
                break;
        }
        return true;
    }
    //帧间隙直线位移
    protected static bool UpdateLine(IMove target, MoveInfo moveInfo, SkillCommand command, int dtFrame = 0)
    {
        float speed = dtFrame * moveInfo.SpeedX;
        target.MovePos += speed * command.SkillDir;
        return false;
    }

    //移动到目标点
    protected static bool UpdateLineTarget(IMove target, MoveInfo moveInfo, SkillCommand command, int dtFrame = 0)
    {
        //速度最大
        if (moveInfo.SpeedX == -1)
        {
            target.MovePos = command.TargetPos;
            return true;
        }
        //每帧移动一段 防止一次移动多帧 超出范围
        for (int i = 0; i < dtFrame; i++)
        {
            target.MovePos += moveInfo.SpeedX * command.SkillDir;
            if (Vector3.Distance(target.MovePos, command.TargetPos) <= 2 * moveInfo.SpeedX)
            {
                target.MovePos = command.TargetPos;
                return true;
            }
        }
        return false;
    }

}
