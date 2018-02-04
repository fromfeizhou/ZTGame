using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//角色类型
public enum CHARA_TYPE
{
    PLAYER = 0,     //玩家
    PET,       //宠物
    MOUNT,   //坐骑
    MONSTER,   //怪物
}

// 角色状态
public enum BATTLE_STATE
{
    NONE = 0,   //正常
    MOVE,   //移动
    SKILL,  //技能中
    DIE     //死亡
}
// 移动方向
public enum MOVE_DIR
{
    NONE = 0,   //待机
    UP = 1,
    UP_LEFT = 2,
    LEFT = 3,
    DOWN_LEFT = 4,
    DOWN = 5,
    DOWN_RIGHT = 6,
    RIGHT = 7,
    UP_RIGHT = 8
}

public enum ATTRIBUTE
{
    HP = 0,     //血量
    ATTACK,     //攻击
}


//属性改变类型
public enum ATT_ALTER_TYPE
{
    VALUE = 0,      //绝对值
    PRECENT,        //基础值百分比
}


//角色事件
public struct CHARA_EVENT
{
    public static readonly string DESTROY = "CHARA_EVENT_DESTROY";
    public static readonly string PLAY = "CHARA_EVENTPLAY";
    public static readonly string CHANGE_ANIM = "CHARA_EVENT_CHANGE_ANIM";
    public static readonly string UPDATE_POS = "CHARA_EVENT_UPDATE_POS";
    public static readonly string CHANGE_ROTATE = "CHARA_EVENT_CHANGE_ROTATE";
    public static readonly string ADD_EFFECT = "CHARA_EVENT_ADD_EFFECT";
    public static readonly string REMOVE_EFFECT = "CHARA_EVENT_REMOVE_EFFECT";
    public static readonly string ADD_BUFF = "CHARA_EVENTADD_BUFF";
    public static readonly string REMOVE_BUFF = "CHARA_EVENT_REMOVE_BUFF";
    public static readonly string ADD_HURT = "CHARA_EVENT_ADD_HURT";
}

public struct PLAYER_AC_NAME
{
    public static string IDLE = "idle";             //待机
    public static string IDLE_1 = "idle_1";         //随机待机
    public static string RUN = "run";               //跑
    public static string ATTACK_1 = "attack_1";     //攻击1
    public static string ATTACK_2 = "attack_2";     //攻击2
    public static string ATTACK_3 = "attack_3";     //攻击3
    public static string HIT = "hit";               //受击
    public static string DIE = "die";               //死亡
    public static string SKILL_1 = "skill_1";       //技能1
    public static string SKILL_2 = "skill_2";       //技能2
    public static string SKILL_3 = "skill_3";       //技能3
    public static string STUN = "stun";             //晕眩
    public static string DODGE = "dodge";           //闪躲
    public static string REPEL = "repel";           //击退
    public static string WIN = "win";               //胜利
}

public class CharaDefine
{
    public static float PLAYER_SPEED = 0.05f;      //移动速度
    public static float PLAYER_RADIUS = 1f;       //碰撞半径

    public static Vector3 VecUpLeft = new Vector3(-1, 0, 1);
    public static Vector3 VecUpRight = new Vector3(1, 0, 1);
    public static Vector3 VecDownLeft = new Vector3(-1, 0, -1);
    public static Vector3 VecDownRight = new Vector3(1, 0, -1);
    //获取方向 单位向量
    public static Vector3 GetDirVec(MOVE_DIR dir)
    {
        if (MOVE_DIR.NONE == 0)
        {
        }
        switch (dir)
        {
            case MOVE_DIR.UP:
                return Vector3.forward;
            case MOVE_DIR.DOWN:
                return Vector3.back;
            case MOVE_DIR.LEFT:
                return Vector3.left;
            case MOVE_DIR.RIGHT:
                return Vector3.right;
            case MOVE_DIR.UP_LEFT:
                return VecUpLeft;
            case MOVE_DIR.UP_RIGHT:
                return VecUpRight;
            case MOVE_DIR.DOWN_LEFT:
                return VecDownLeft;
            case MOVE_DIR.DOWN_RIGHT:
                return VecDownRight;
        }
        return Vector3.zero;
    }

    public static List<Vector3> GetDirMoveVecs(MOVE_DIR dir)
    {
        List<Vector3> list = new List<Vector3>();
        switch (dir)
        {
            case MOVE_DIR.UP:
                list.Add(Vector3.forward);
                list.Add(Vector3.right);
                break;
            case MOVE_DIR.DOWN:
                list.Add(Vector3.back);
                list.Add(Vector3.left);
                break;
            case MOVE_DIR.LEFT:
                list.Add(Vector3.left);
                list.Add(Vector3.forward);
                break;
            case MOVE_DIR.RIGHT:
                list.Add(Vector3.right);
                list.Add(Vector3.back);
                break;
            case MOVE_DIR.UP_LEFT:
                list.Add(VecUpLeft);
                list.Add(Vector3.forward);
                list.Add(Vector3.left);
                break;
            case MOVE_DIR.UP_RIGHT:
                list.Add(VecUpRight);
                list.Add(Vector3.forward);
                list.Add(Vector3.right);
                break;
            case MOVE_DIR.DOWN_LEFT:
                list.Add(VecDownLeft);
                list.Add(Vector3.back);
                list.Add(Vector3.left);
                break;
            case MOVE_DIR.DOWN_RIGHT:
                list.Add(VecDownRight);
                list.Add(Vector3.back);
                list.Add(Vector3.right);
                break;
        }
        return list;
    }
}

public enum HURT_TYPE
{
    NORMAL = 0, //普通
    CRIT,       //暴击
    PARRY,      //格挡
}
//伤害结构
public struct HurtInfo
{
    public HURT_TYPE Type;
    public float Value;
}