using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaFightInfo : CharaActorInfo,ICharaFight {
    //**===================接口实现=======================**//
    //生命值
    public int Hp { get; set; }

    public void SetFightInfo(int hp = 100)
    {
        Hp = hp;
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
