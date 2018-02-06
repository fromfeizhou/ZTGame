using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAsset : ScriptableObject
{
    public List<SkillAssetInforGroup> ListSkillGroup;
}

[System.Serializable]
public class SkillAssetInfo
{
    public SkillDefine.SkillActionType actionType = SkillDefine.SkillActionType.NONE;
    //动作播放
    public string animName;
    //控制器
    public bool isCtrl;

    //移动参数
    public MoveInfo moveInfo;
    //特效
    public EffectInfo effectInfo;

    //碰撞 参数
    public ColliderInfo colliderInfo;

    //public List<FightEffectInfo> fightEffects;

    public SkillAssetInfo()
    {
        isCtrl = false;
        animName = "";
        moveInfo = new MoveInfo();
        effectInfo = new EffectInfo();
        colliderInfo = new ColliderInfo() ;
        //fightEffects = new List<FightEffectInfo>();
    }
}

[System.Serializable]
public class SkillAssetInforGroup
{
    public int FrameTime = 0;
    public List<SkillAssetInfo> ListSkillInfo;
}