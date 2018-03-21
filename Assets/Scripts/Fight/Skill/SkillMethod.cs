using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMethod
{
    //===================================目标选择=================================================
    public static List<ICharaBattle> GetTargetList(ICharaBattle battleInfo, SkillDefine.ColliderTarget targetType)
    {
        List<ICharaBattle> targetList = new List<ICharaBattle>();
        if (targetType == SkillDefine.ColliderTarget.SELF)
        {
            targetList.Add(battleInfo as ICharaBattle);
            return targetList;
        }

        List<CharaActorInfo> list = ZTBattleSceneManager.GetInstance().GetCharaList();
        for (int i = 0; i < list.Count; i++)
        {
            ICharaBattle info = list[i] as ICharaBattle;
            //过滤死亡玩家
            if (null != info && !info.IsDead())
            {
                switch (targetType)
                {
                    case SkillDefine.ColliderTarget.TEAM:
                        if (battleInfo.Camp == info.Camp)
                        {
                            targetList.Add(info);
                        }
                        break;
                    case SkillDefine.ColliderTarget.ENEMY:
                        if (battleInfo.Camp != info.Camp)
                        {
                            targetList.Add(info);
                        }
                        break;
                    case SkillDefine.ColliderTarget.ALL:
                        targetList.Add(info);
                        break;
                }
            }
        }
        return targetList;
    }

    public static ICharaBattle GetNearestTarget(ICharaBattle battleInfo, SkillDefine.ColliderTarget targetType)
    {
        List<ICharaBattle> targetList = GetTargetList(battleInfo, targetType);
        ICharaBattle target = null;
        float dis = -1;
        for (int i = 0; i < targetList.Count; i++)
        {
            float tmpDis = Vector3.Distance(battleInfo.MovePos, targetList[i].MovePos);
            if (dis == -1)
            {
                dis = tmpDis;
                target = targetList[i];
            }
            else
            {
                if (tmpDis < dis)
                {
                    dis = tmpDis;
                    target = targetList[i];
                }
            }
        }

        return target;
    }

    //===================================目标选择=================================================

    //===================================移动=================================================
    public static bool MoveAction(IMove target, MoveInfo moveInfo, Vector3 dir, Vector3 targetPos, Vector3 playerPos)
    {
        switch (moveInfo.MoveType)
        {
            case SkillDefine.MoveType.LINE:
                return UpdateLine(target, moveInfo, dir);
            case SkillDefine.MoveType.LINE_TARGET:
                return UpdateLineTarget(target, moveInfo, dir, targetPos);
            case SkillDefine.MoveType.PLAYER:
                target.MovePos = playerPos;
                return false;
            case SkillDefine.MoveType.ROTATE:
                break;
        }
        return true;
    }
    //帧间隙直线位移
    protected static bool UpdateLine(IMove target, MoveInfo moveInfo, Vector3 dir)
    {
        float speed = moveInfo.SpeedX;
        target.MovePos += speed * dir;
        return false;
    }

    //移动到目标点
    protected static bool UpdateLineTarget(IMove target, MoveInfo moveInfo, Vector3 dir, Vector3 targetPos)
    {
        //速度最大
        if (moveInfo.SpeedX == -1)
        {
            target.MovePos = targetPos;
            return true;
        }

        target.MovePos += moveInfo.SpeedX * dir;
        if (Vector3.Distance(target.MovePos, targetPos) <= 2 * moveInfo.SpeedX)
        {
            target.MovePos = targetPos;
            return true;
        }
        return false;
    }
    //===================================移动=================================================
}