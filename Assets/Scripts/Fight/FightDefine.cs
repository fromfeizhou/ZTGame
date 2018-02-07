using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum COMMAND_TYPE
{
    MOVE = 0,
    SKILL,
}

//操作指令
public class FightCommandBase
{
    public COMMAND_TYPE CommandType;
    public int Frame;
    public int BattleId;
    public FightCommandBase(COMMAND_TYPE type, int battleId, int frame)
    {
        CommandType = type;
        BattleId = battleId;
        Frame = frame;
    }
}

public class MoveCommand : FightCommandBase
{
    public MOVE_DIR MoveDir;
    public MoveCommand(int battleId, int frame, MOVE_DIR dir)
        : base(COMMAND_TYPE.MOVE, battleId, frame)
    {
        MoveDir = dir;
    }
}

public class SkillCommand : FightCommandBase
{
    public int SkillId = -1;
    public int StartFrame = 0;
    public Vector3 SkillDir = Vector3.zero;     //技能方向
    public Vector3 TargetPos = Vector3.zero;    //目标坐标(相对player)
    public int TargetId;    //指定目标
    public SkillCommand(int battleId, int frame,int actionId, Vector3 dir, Vector3 targetPos,int targetId)
        : base(COMMAND_TYPE.SKILL, battleId, frame)
    {
        SkillId = actionId;
        StartFrame = frame;
        SkillDir = dir;
        TargetPos = targetPos;
        TargetId = targetId;
    }
}


public class FightDefine
{
    public static int MaxFrame = int.MaxValue; //60帧每秒 理论上可以计数400多天

    public static bool CompareFrame(int frame)
    {
        int curFrame = ZTSceneManager.GetInstance().SceneFrame;
        if (curFrame >= frame)
        {
            return true;
        }
        return false;
    }

    // 获得操作结构
    public static MoveCommand GetMoveCommand(int battleId, MOVE_DIR dir = MOVE_DIR.NONE)
    {
        int frame = ZTSceneManager.GetInstance().SceneFrame;
        return new MoveCommand(battleId,frame, dir);
    }
    // 获得操作结构
    public static SkillCommand GetSkillCommand(int battleId,int actionId,Vector3 dir,Vector3 targetPos,int targetId = -1)
    {
        int frame = ZTSceneManager.GetInstance().SceneFrame;
        return new SkillCommand(battleId,frame,actionId,dir,targetPos,targetId);
    }


}
