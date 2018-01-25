using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectInfo
{
    public string Id;
    public Vector3 Offset = Vector3.zero;
    public float Scale = 1.0f;
    public bool IsAdd = true;
    public EffectInfo(string id = "", float scale = 1.0f, bool isAdd = true, Vector3 offset = default(Vector3))
    {
        Id = id;
        Scale = scale;
        Offset = offset;
        IsAdd = isAdd;
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

public class ColliderInfo : ScriptableObject
{
    public int Interval;    //触发间隔
    public int LifeTime;    //存在时间
    public bool IsPenetrate;    //穿透障碍
    public int ColliderMax; //最大碰撞个数

    public string EffectId;     //特效

    public List<int> SelfActions = new List<int>();
    public List<int> TargetActions = new List<int>();


    // 碰撞框 参数
    public SkillDefine.ColliderTarget ColliderTarget = SkillDefine.ColliderTarget.SELF;     //碰撞对象

    public CollBase.ColType ColliderType = CollBase.ColType.CIRCLE; //碰撞类型
    public CollBase.PosType CollPosType = CollBase.PosType.SKILL;   //挂扣形式
    public float StartX = 0;
    public float StartZ = 0;
    public float StartAngle = 0;
    //矩形
    public float Width = 0;
    public float Height = 0;
    //扇形
    public float InCircle = 0;
    public float OutCircle = 0;
    public float Angle = 1;
    //圆半径
    public float Radius = 0;
}

//public class ColliderData
//{
//    public CollBase Collider;
//    public int Interval;    //触发间隔
//    public int LifeTime;    //存在时间
//    public bool IsPenetrate;    //穿透障碍
//    public int ColliderMax; //最大碰撞个数
//    public string ColliderActions; //碰撞执行
//    public string EffectId;     //特效

//    public List<int> SelfActoins;
//    public List<int> TargetActions;
//    public ColliderData(CollBase collBase, int interval, int lifeTime, int colliderMax, bool isPenetrate, string effectId, string colliderActions)
//    {
//        Collider = collBase;
//        Interval = interval;
//        LifeTime = lifeTime;
//        ColliderMax = colliderMax;
//        IsPenetrate = isPenetrate;
//        ColliderActions = colliderActions;
//        EffectId = effectId;

//        if (colliderActions == "")
//            return;

//        string[] split = colliderActions.Split(',');
//        for (int i = 0; i < split.Length; i++)
//        {
//            int actionId = int.Parse(split[i]);
//            if (actionId > 0)
//            {
//                if (null == TargetActions)
//                {
//                    TargetActions = new List<int>();
//                }
//                TargetActions.Add(actionId);
//            }
//            else
//            {
//                if (null == SelfActoins)
//                {
//                    SelfActoins = new List<int>();
//                }
//                SelfActoins.Add(actionId);
//            }
//        }
//    }

//}


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
    }

    public enum MoveType
    {
        PLAYER,     //跟随玩家
        ROTATE,     //旋转
        LINE,       //线性
        LINE_TARGET,//线性指定点 (玩家操作提供)
    }
}
