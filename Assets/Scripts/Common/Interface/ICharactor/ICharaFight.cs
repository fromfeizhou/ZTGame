using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//战斗相关属性
public interface ICharaFight{
  
    //生命值
    int Hp { get; set; }
    int MaxHp { get; set; }
    int Attack { get; set; }

    //添加伤害
    void AddHurt(HurtInfo info);
    void SetFightInfo(int hp = 100, int maxHp = 100,int attack = 10);

}
