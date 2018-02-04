using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class FightEffectInfo
{
    public FIGHT_EF_TPYE EffectType;
    public int Param1;  //表演id; buff id/类型; 改变属性参数;
    public int Param2;
    public int Param3;
    public int Param4;
}

[System.Serializable]
public class EffectInfo
{
    public int AssetKey = -1;//资源管理id 用于自动清理
    public string Id;
    public Vector3 Offset = Vector3.zero;
    public float Scale = 1.0f;
    public float Rotate = 0;
    public int LifeTime = -1;
    public EffectInfo(string id = "", float scale = 1.0f, int lifeTime = -1, Vector3 offset = default(Vector3))
    {
        Id = id;
        Scale = scale;
        Offset = offset;
        LifeTime = lifeTime;
    }

}
[System.Serializable]
public class MoveInfo
{
    public int FrameCount;
    public float SpeedX;     //速度speed
    public float SpeedZ;
    public float Angle;     //起始角度
    public float Rotate;    //旋转角度
    public SkillDefine.MoveType MoveType;

    public MoveInfo(SkillDefine.MoveType type = SkillDefine.MoveType.LINE, float angle = 0, int count = 0, float speedX = 0, float speedZ = 0, float rotate = 0)
    {
        MoveType = type;
        FrameCount = count;
        SpeedX = speedX;
        SpeedZ = speedZ;
        Angle = angle;
        Rotate = rotate;
    }
}
[System.Serializable]
public class ColliderInfo
{
    public int Interval;    //触发间隔
    public int LifeTime;    //存在时间
    public bool IsPenetrate;    //穿透障碍
    public int ColliderMax; //最大碰撞个数

    public List<int> SelfActions;
    public List<int> TargetActions;
    public List<EffectInfo> EffectInfos;


    // 碰撞框 参数
    public SkillDefine.ColliderTarget ColliderTarget;     //碰撞对象

    public CollBase.ColType ColliderType; //碰撞类型
    public CollBase.PosType CollPosType;   //挂扣形式
    public float StartX;
    public float StartZ;
    public float StartAngle;
    //矩形
    public float Width;
    public float Height;
    //扇形
    public float InCircle;
    public float OutCircle;
    public float Angle;
    //圆半径
    public float Radius;

    public ColliderInfo()
    {
        Interval = 0;
        LifeTime = 10;
        IsPenetrate = false;
        ColliderMax = -1;

        SelfActions = new List<int>();
        TargetActions = new List<int>();
        EffectInfos = new List<EffectInfo>();

        ColliderTarget = SkillDefine.ColliderTarget.SELF;

        ColliderType = CollBase.ColType.CIRCLE;
        CollPosType = CollBase.PosType.SKILL;

        StartX = 0;
        StartZ = 0;
        StartAngle = 0;
        //矩形
        Width = 0;
        Height = 0;
        //扇形
        InCircle = 0;
        OutCircle = 0;
        Angle = 1;
        //圆半径
        Radius = 0;
    }
}


public class SkillDefine
{

    public enum SkillPointType
    {
        NONE,   //无指向
        POINT,  //指定点
        DIRECTION,  //方向性
    }

    public enum ColliderTarget
    {
        SELF,       //自己
        TEAM,       //队友
        ENEMY,      //敌人
        ALL,        //所有对象
    }


    public enum SkillActionType
    {
        NONE,           //释放控制权(控制玩家状态)
        PLAY_CONTROL,   //设置技能控制权(控制玩家状态)
        PLAY_ANIM,     //动画播放
        PLAY_MOVE,      //玩家移动
        COLLIDER,      //碰撞
        COLLIDER_MOVE,  //碰撞移动
        ADD_EFFECT,     //添加特效
        FIGHT_EFFECT,   //战斗效果
    }

    public enum MoveType
    {
        PLAYER,     //跟随玩家
        ROTATE,     //旋转
        LINE,       //线性
        LINE_TARGET,//线性指定点 (玩家操作提供)
    }
}
