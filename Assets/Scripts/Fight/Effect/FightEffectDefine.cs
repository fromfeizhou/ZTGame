using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//战斗效果（技能 buff 通用）
public enum FIGHT_EF_TPYE
{
    HURT = 0,       //伤害
    ATTRIBUTE,      //属性
    ADD_BUFF,       //添加buff
    RE_BUFF,        //移除buff
}

public enum ATTRIBUTE
{
    HP,     //血量
    ATTACK,     //攻击
}

//属性改变类型
public enum ATT_ALTER_TYPE
{
    VALUE = 0,      //绝对值
    PRECENT,        //基础值百分比
    ATTACK_PREC,    //攻击力百分比
    HURT_PREC,           //伤害百分比
}

//buff修改类型
public enum BUFF_ALTER_TYPE
{
    ID = 0,      //id
    TYPE,        //类型
}




public class FightEffectDefine
{
}
