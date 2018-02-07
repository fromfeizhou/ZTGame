using com.game.client.network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleProtocol : Singleton<BattleProtocol> {

    public override void Init()
    {
        base.Init();
    }

    //进入场景
    public void SendEnterBattle(int battleId, Vector3 pos)
    {
        gprotocol.role_bc_info_c2s vo = new gprotocol.role_bc_info_c2s()
        {
           data = "123124214",
        };
        NetWorkManager.Instace.SendNetMsg(Module.role, Command.role_bc_info, vo);
    }

    //推帧
    public void SendFrameCommand(int battleId, int frame)
    {
    }

    //移动
    public void SendMoveComand(int battleId, int frame,Vector3 pos, MOVE_DIR dir)
    {
        
    }

    //技能使用
    public void SendSkillCommand(int battleId, int frame, int actionId, Vector3 dir, Vector3 targetPos)
    {
    }

    //收到玩家进入场景
    public void ParseEnterBattle(int battleId, int frame)
    {
    }

    //收到推帧命令
    public void ParseFrameCommand(int battleId, int frame)
    {
    }

    //移动
    public void ParseMoveComand(int battleId, int frame, Vector3 pos, MOVE_DIR dir)
    {
    }

    //技能使用
    public void ParseSkillCommand(int battleId, int frame, int actionId, Vector3 dir, Vector3 targetPos)
    {
    }


    public override void Destroy()
    {
        base.Destroy();
    }
}
