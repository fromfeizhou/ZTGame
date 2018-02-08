using com.game.client.network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BP_BATTLE_TYPE{
    ENTER = 0,
    MOVE,
}

[System.Serializable]
public class BPBattle
{
    public BP_BATTLE_TYPE Type;
    public uint BattleId;
    public Vector3 Pos;
    public uint Frame;

    public BPBattle(uint battleId, Vector3 pos, uint frame)
    {
        BattleId = battleId;
        Pos = pos;
        Frame = frame;
    }
}

public class BPEnter:BPBattle
{
    public BPEnter(uint battleId, Vector3 pos, uint frame)
        : base(battleId,pos,frame)
    {
        Type = BP_BATTLE_TYPE.ENTER;
    }
}

public class BPMove:BPBattle{
    public MOVE_DIR Dir;
    public BPMove(uint battleId, Vector3 pos, uint frame, MOVE_DIR dir)
        : base(battleId,pos,frame)
    {
        Dir = dir;
        Type = BP_BATTLE_TYPE.MOVE;
    }
}

public class BP_BATTLE_EVENT
{
    public  const string COMMAND = "BP_BATTLE_EVENT_COMMAND";
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

    //进入场景
    public void SendEnterBattle(uint battleId, Vector3 pos, uint frame)
    {
        BPBattle bp = new BPBattle(battleId, pos,frame);
        JsonUtility.ToJson(bp);
        string bpOut = JsonUtility.ToJson(bp);

        gprotocol.role_bc_info_c2s vo = new gprotocol.role_bc_info_c2s()
        {
            data = bpOut,
        };
        NetWorkManager.Instace.SendNetMsg(Module.role, Command.role_bc_info, vo);
    }

    //推帧
    public void SendFrameCommand(int battleId, uint frame)
    {
    }

    //移动
    public void SendMoveComand(int battleId, uint frame, Vector3 pos, MOVE_DIR dir)
    {
        
    }

    //技能使用
    public void SendSkillCommand(int battleId, uint frame, int actionId, Vector3 dir, Vector3 targetPos)
    {
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
        }
      
    }

    //收到玩家进入场景
    public void ParseEnterBattle(BPEnter bp)
    {
        SceneEvent.GetInstance().dispatchEvent(SCENE_EVENT.ADD_PLAYER, new Notification(bp.BattleId));
    }

    //收到推帧命令
    public void ParseFrameCommand(int battleId, int frame)
    {
    }

    //移动
    public void ParseMoveComand(BPMove bp)
    {
    }

    //技能使用
    public void ParseSkillCommand(int battleId, int frame, int actionId, Vector3 dir, Vector3 targetPos)
    {
    }


    public override void Destroy()
    {
    }
}
