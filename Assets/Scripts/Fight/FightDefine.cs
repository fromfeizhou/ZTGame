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
    public Vector3 Dir;
    public Vector3 TargetPos;
    public SkillCommand(int battleId, int frame, Vector3 dir, Vector3 targetPos)
        : base(COMMAND_TYPE.SKILL, battleId, frame)
    {
        Dir = dir;
        TargetPos = targetPos;
    }
}


public class FightDefine
{
    public static int MaxFrame = int.MaxValue - 450;

    public static bool CompareFrame(int frame)
    {
        int curFrame = ZTSceneManager.GetInstance().SceneFrame;
        if (curFrame >= frame)
        {
            return true;
        }
        //帧率溢出 补足(10s 的帧数450)
        if (MaxFrame - frame < 450 && curFrame < 450)
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
    public static SkillCommand GetSkillCommand(int battleId,Vector3 dir,Vector3 targetPos)
    {
        int frame = ZTSceneManager.GetInstance().SceneFrame;
        return new SkillCommand(battleId,frame,dir,targetPos);
    }


}
