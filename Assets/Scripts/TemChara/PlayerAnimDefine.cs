using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public struct PlayerAnimEvents
{
    public static readonly string PLAY = "PlayerAnimEvents_PlayActoin";
    public static readonly string UPDATE_POS = "PlayerAnimEvents_UpdatePos";
}

public class PlayerAnimDefine
{
}
