using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaFightInfo : CharaActorInfo, ICharaFight
{
    //**===================接口实现=======================**//
    //生命值
    private int _hp;
    public int Hp
    {
        get { return _hp; }
        set
        {
            if (_hp == value) return;
            _hp = value;

            if (_hp <= 0)
            {
                _hp = 0;
                ICharaBattle info = this as ICharaBattle;
                if (null != info)
                {
                    info.SetDead(true);
                }
            }
            else if (_hp > MaxHp)
            {
                _hp = MaxHp;
            }

            this.dispatchEvent(CHARA_EVENT.UPDATE_HP);

        }
    } 

    public int MaxHp { get; set; }
    //攻击力
    public int Attack { get; set; }

    public void SetFightInfo(int hp = 100, int maxHp = 100, int attack = 10)
    {
        Hp = hp;
        MaxHp = maxHp;
        Attack = attack;
    }


    //添加伤害 显示
    public void AddHurt(HurtInfo info)
    {
        Hp += info.Value;
        SceneEvent.GetInstance().dispatchEvent(SCENE_EVENT.ADD_UI_HURT_VALUE, new Notification(info));
    }


    //**===================接口实现=======================**//

    public CharaFightInfo(int animaId, CHARA_TYPE type)
        : base(animaId, type)
    {
        SetFightInfo();
    }

    public override void Destroy()
    {
        base.Destroy();
    }
}
