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


//角色事件
public struct CharaEvents
{
    public static readonly string DESTROY = "CharaEvents_Destroy";
    public static readonly string PLAY = "CharaEvents_PlayActoin";
    public static readonly string CHANGE_ANIM = "CharaEvents_ChangeAnim";
    public static readonly string UPDATE_POS = "CharaEvents_UpdatePos";
    public static readonly string CHANGE_ROTATE = "CharaEvents_ChangeRotate";
    public static readonly string UPDATE_EFFECT = "CharaEvents_UpdateEffect";
}

public struct PlayerActionName
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
    public static float PLAYER_SPEED = 0.1f;      //移动速度
    public static float PLAYER_RADIUS = 1f;       //碰撞半径
    
    //获取方向 单位向量
    public static Vector3 GetDirVec(MOVE_DIR dir)
    {
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
                return new Vector3(-1, 0, 1);
            case MOVE_DIR.UP_RIGHT:
                return new Vector3(1, 0, 1);
            case MOVE_DIR.DOWN_LEFT:
                return new Vector3(-1, 0, -1);
            case MOVE_DIR.DOWN_RIGHT:
                return new Vector3(1, 0, -1);
        }
        return Vector3.zero;
    }
}