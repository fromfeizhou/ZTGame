using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillOpera
{
    public int ActionId = -1;
    public int StartFrame = 0;
    public Vector3 SkillDir = Vector3.zero;     //技能方向
    public Vector3 TargetPos = Vector3.zero;    //目标坐标(相对player)

    public SkillOpera(int actionId,int startFrame = 1, Vector3 skillDir = default(Vector3), Vector3 targetPos = default(Vector3))
    {
        ActionId = actionId;
        StartFrame = startFrame;
        SkillDir = skillDir;
        TargetPos = targetPos;
    }
}

public class MoveInfo
{
    public int FrameCount;
    public float SpeedX;     //速度speed
    public float SpeedZ;   
    public float Angle;     //起始角度
    public float Rotate;    //旋转角度
    public SkillDefine.MoveType MoveType;

    public MoveInfo(SkillDefine.MoveType type,float angle = 0.1f, int count = 0, float speedX = 0.1f, float speedZ = 0.1f, float rotate = 0.1f)
    {
        MoveType = type;
        FrameCount = count;
        SpeedX = speedX;
        SpeedZ = speedZ;
        Angle = angle;
        Rotate = rotate;
    }
}

public class ColliderData
{
    public CollBase Collider;
    public int Interval;    //触发间隔
    public int LifeTime;    //存在时间
    public bool IsPenetrate;    //穿透障碍
    public int ColliderMax; //最大碰撞个数
    public string ColliderActions; //碰撞执行
    public string EffectId;     //特效

    public List<int> SelfActoins;
    public List<int> TargetActions;
    public ColliderData(CollBase collBase, int interval, int lifeTime, int colliderMax, bool isPenetrate, string effectId, string colliderActions)
    {
        Collider = collBase;
        Interval = interval;
        LifeTime = lifeTime;
        ColliderMax = colliderMax;
        IsPenetrate = isPenetrate;
        ColliderActions = colliderActions;
        EffectId = effectId;

        if (colliderActions == "")
            return;

        string[] split = colliderActions.Split(',');
        for (int i = 0; i < split.Length; i++)
        {
            int actionId = int.Parse(split[i]);
            if (actionId > 0)
            {
                if (null == TargetActions)
                {
                    TargetActions = new List<int>();
                }
                TargetActions.Add(actionId);
            }
            else
            {
                if (null == SelfActoins)
                {
                    SelfActoins = new List<int>();
                }
                SelfActoins.Add(actionId);
            }
        }
    }

}
[System.Serializable]
public class EffectInfo
{
    public string Id;
    public Vector3 Offset = Vector3.zero;
    public float Scale = 1.0f;
    public EffectInfo(string id = "", float scale = 1.0f, Vector3 offset = default(Vector3))
    {
        Id = id;
        Scale = scale;
        Offset = offset;
    }

}

public class SkillDefine  {

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
        NONE,
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
