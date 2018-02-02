using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaFightInfo : CharaActorInfo,ICharaFight {
    //**===================接口实现=======================**//
    //生命值
    public int Hp { get; set; }
    //攻击力
    public int Attack { get; set; }

    public void SetFightInfo(int hp = 100,int attack = 10)
    {
        Hp = hp;
        Attack = attack;
    }


    //**===================接口实现=======================**//

    public CharaFightInfo(int animaId, CHARA_TYPE type)
        : base(animaId,type)
    {
        SetFightInfo();
    }

    public override void Destroy()
    {
        base.Destroy();
    }
}
