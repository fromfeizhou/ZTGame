using com.game.client.network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BP_BATTLE_TYPE
{
    ENTER = 0,
    MOVE,
    SKILL,
}

[System.Serializable]
public class BPBattle
{
    public BP_BATTLE_TYPE Type;
    public uint BattleId;
    public Vector3 Pos;
    public uint Frame;

    public BPBattle(uint battleId)
    {
        BattleId = battleId;
        Frame = ZTSceneManager.GetInstance().SceneFrame;

        Pos = new Vector3(400, 0, 400);
        ICharaBattle info = ZTSceneManager.GetInstance().GetCharaById(battleId) as ICharaBattle;
        if (null != info)
        {
            Pos = info.MovePos;
        }
    }
}

public class BPEnter : BPBattle
{
    public uint CareerType;
    public BPEnter(uint battleId, uint career)
        : base(battleId)
    {
        Type = BP_BATTLE_TYPE.ENTER;
        CareerType = career;
    }
}

public class BPMove : BPBattle
{
    public MOVE_DIR Dir;
    public BPMove(uint battleId,  MOVE_DIR dir)
        : base(battleId)
    {
        Dir = dir;
        Type = BP_BATTLE_TYPE.MOVE;
    }
}

public class BPSkill : BPBattle
{
    public int SkillId;
    public Vector3 SkillDir;
    public Vector3 TargetPos;
    public uint TargetId;

    public BPSkill(uint battleId, int skillId, Vector3 dir, Vector3 targetPos, uint targetId)
        : base(battleId)
    {
        Type = BP_BATTLE_TYPE.SKILL;
        SkillId = skillId;
        SkillDir = dir;
        TargetPos = targetPos;
        TargetId = targetId;
    }
}

public class BP_BATTLE_EVENT
{
    public const string COMMAND = "BP_BATTLE_EVENT_COMMAND";
}
public class BPBattleEvent : Singleton<NotificationDelegate>
{
}

public class BattleProtocol : Singleton<BattleProtocol>
{

    public override void Init()
    {
        InitEvent();
    }

    public void SendMsg(object bp)
    {
        JsonUtility.ToJson(bp);
        string bpOut = JsonUtility.ToJson(bp);
        gprotocol.role_bc_info_c2s vo = new gprotocol.role_bc_info_c2s()
        {
            data = bpOut,
        };
        NetWorkManager.Instace.SendNetMsg(Module.role, Command.role_bc_info, vo);
    }

    //进入场景
    public void SendEnterBattle(uint battleId, uint careerType)
    {
        BPEnter bp = new BPEnter(battleId, careerType);
        SendMsg(bp);
    }

    //推帧
    public void SendFrameCommand(uint battleId, uint frame)
    {

    }

    //移动
    public void SendMoveComand(uint battleId,  MOVE_DIR dir)
    {
        Debug.Log("SendMoveComand");
        BPMove bp = new BPMove(battleId, dir);
        SendMsg(bp);
    }

    //技能使用
    public void SendSkillCommand(uint battleId, int actionId, Vector3 dir, Vector3 targetPos,uint targetId)
    {
        BPSkill bp = new BPSkill(battleId,actionId,dir,targetPos,targetId);
        SendMsg(bp);
    }

    public void InitEvent()
    {
        BPBattleEvent.GetInstance().addEventListener(BP_BATTLE_EVENT.COMMAND, OnBPBattleEvent);
    }

    public void RemoveEvent()
    {
        BPBattleEvent.GetInstance().removeEventListener(BP_BATTLE_EVENT.COMMAND, OnBPBattleEvent);
    }

    public void OnBPBattleEvent(Notification data)
    {
        string bpOut = (string)data.param;
        BPBattle bp = JsonUtility.FromJson<BPBattle>(bpOut);
       
        switch (bp.Type)
        {
            case BP_BATTLE_TYPE.ENTER:
                ParseEnterBattle(JsonUtility.FromJson<BPEnter>(bpOut));
                break;
            case BP_BATTLE_TYPE.MOVE:
                ParseMoveComand(JsonUtility.FromJson<BPMove>(bpOut));
                break;
            case BP_BATTLE_TYPE.SKILL:
                ICharaBattle info = ZTSceneManager.GetInstance().GetCharaById(bp.BattleId) as ICharaBattle;
                if (null != info)
                {
                    info.MovePos = bp.Pos;
                }
                ParseSkillCommand(JsonUtility.FromJson<BPSkill>(bpOut));
                break;
        }

    }

    //收到玩家进入场景
    public void ParseEnterBattle(BPEnter bp)
    {
        SceneEvent.GetInstance().dispatchEvent(SCENE_EVENT.ADD_PLAYER, new Notification(bp));

        //通知其他玩家自己位置
        if (bp.BattleId != PlayerModule.GetInstance().RoleID)
        {
            BattleProtocol.GetInstance().SendEnterBattle(PlayerModule.GetInstance().RoleID,PlayerModule.GetInstance().RoleJob);
        }
    }

    //收到推帧命令
    public void ParseFrameCommand(BPBattle bp)
    {
        
    }

    //移动
    public void ParseMoveComand(BPMove bp)
    {
        Debug.Log("ParseMoveComand");
        MoveCommand command = FightDefine.GetMoveCommand(bp.BattleId, bp.Frame,bp.Dir);
        SceneEvent.GetInstance().dispatchEvent(SCENE_EVENT.ADD_COMMAND, new Notification(command));
    }

    //技能使用
    public void ParseSkillCommand(BPSkill bp)
    {
        SkillCommand command = FightDefine.GetSkillCommand(bp.BattleId, bp.Frame, bp.SkillId, bp.SkillDir, bp.TargetPos, bp.TargetId);
        SceneEvent.GetInstance().dispatchEvent(SCENE_EVENT.ADD_COMMAND, new Notification(command));
    }


    public override void Destroy()
    {
    }
}
