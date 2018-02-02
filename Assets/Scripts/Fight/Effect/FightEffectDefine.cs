using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//战斗效果（技能 buff 通用）
public enum FIGHT_EF_TPYE
{
    NONE = 0,
    HURT,       //伤害
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

//buff修改类型(按id 、类型 添加移除)
// buff效果配置 FIGHT_EF_TPYE,buff处理方式，buff参数(efType,atId,id)(efType,atType,type)
public enum BUFF_ALTER_TYPE
{
    ID = 0,      //id
    TYPE,        //类型
}



public class FightEffectDefine
{
    public static void DoEffect(ICharaBattle battleInfo, FightEffect effect)
    {
        if (null == battleInfo)
            return;

        switch (effect.FightEffectType)
        {
            case FIGHT_EF_TPYE.NONE:
                break;
            case FIGHT_EF_TPYE.HURT:
                CalculateHurt(battleInfo, effect);
                break;
            case FIGHT_EF_TPYE.ADD_BUFF:
                AddBuff(battleInfo, effect);
                break;
            case FIGHT_EF_TPYE.RE_BUFF:
                RemoveBuff(battleInfo, effect);
                break;
        }
    }

    public static void CalculateHurt(ICharaBattle battleInfo, FightEffect effect)
    {
        ICharaFight target = battleInfo as ICharaFight;
        ICharaFight user = ZTSceneManager.GetInstance().GetCharaById(effect.UserId) as ICharaFight;
        if (null == target || null == user) return;
        HurtInfo hurtInfo = new HurtInfo();
        hurtInfo.Type = HURT_TYPE.NORMAL;
        hurtInfo.Value = user.Attack;
        battleInfo.AddHurt(hurtInfo);
    }

    public static void AddBuff(ICharaBattle battleInfo, FightEffect effect)
    {
        if (effect.UserId <= 0) return;
        BUFF_ALTER_TYPE type = (BUFF_ALTER_TYPE)effect.Params[0];
        battleInfo.AddBuff(new BuffData(effect.Params[1],ZTSceneManager.GetInstance().SceneFrame,effect.UserId));
    }

     public static void RemoveBuff(ICharaBattle battleInfo, FightEffect effect)
    {
        BUFF_ALTER_TYPE type = (BUFF_ALTER_TYPE)effect.Params[0];
        switch (type)
        {
            case BUFF_ALTER_TYPE.ID:
                battleInfo.RemoveBuff(effect.Params[1]);
                break;
            case BUFF_ALTER_TYPE.TYPE:
                battleInfo.RemoveBuffByType(effect.Params[1]);
                break;
        }
    }

}
