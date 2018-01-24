using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 战斗场景形象
/// </summary>
public class PlayerBattleActor : PlayerActor {
    protected PlayerBattleInfo _battleInfo;
    public override bool SetInfo(CharaActorInfo info)
    {
        if (base.SetInfo(info))
        {
            _battleInfo = info as PlayerBattleInfo;
            return true;
        }
        return false;
    }

    public override void InitEvent()
    {
        base.InitEvent();
    }

    public override void RemoveEvent()
    {
        base.RemoveEvent();
    }
}
