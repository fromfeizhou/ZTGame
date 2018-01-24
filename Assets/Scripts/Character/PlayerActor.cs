using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 形象（装备界面形象）
/// </summary>
public class PlayerActor : CharaActor {
    protected CharaPlayerInfo _playerInfo;
    public PlayerActor()
    {
    }

    public override bool SetInfo(CharaActorInfo info)
    {
        if (base.SetInfo(info))
        {
            _playerInfo = info as CharaPlayerInfo;
            return true;
        }
        return false;
    }
    
}
