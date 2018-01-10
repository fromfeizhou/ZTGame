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
    public string animName = "";
    
    //移动 参数
    public SkillDefine.MoveType moveType = SkillDefine.MoveType.LINE;
    public int frameCount = 0;
    public float speedX = 1.0f;
    public float speedZ = 1.0f;
    public float angle = 0;
    public float rotate = 0;    //旋转角度

    //碰撞 参数
    public int interval = 0;    //碰撞间隙
    public int colliderMax = 1;    //碰撞最大次数
    public int colliderTime = 1;    //碰撞存在时间
    public bool isPenetrate = false;    //碰撞消失

    public string colliderActions = "";

    public SkillDefine.ColliderTarget colliderTarget = SkillDefine.ColliderTarget.SELF;     //碰撞对象

    public CollBase.ColType colliderType = CollBase.ColType.CIRCLE;
    public CollBase.PosType collPosType = CollBase.PosType.SKILL;
    public float csX = 0;
    public float csZ = 0;
    public float csA = 0;
    public float width = 0;
    public float height = 0;

    public float inCircle = 0;
    public float outCircle = 0;
    public float cAngle = 1;

    public float radius = 0;    //圆半径
    public List<EffectInfo> collMoveEffList = new List<EffectInfo>();


    //特效
    public EffectInfo effectInfo = new EffectInfo();

}

[System.Serializable]
public class SkillAssetInforGroup
{
    public int FrameTime = 0;
    public List<SkillAssetInfo> ListSkillInfo;
}