using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//动画相关
public interface ICharaActor{
    //形象id
    int AnimaId { get; set; }

    //对象类型 （人物 宠物 坐骑 怪物）
    CHARA_TYPE CharaType { get; set; }
   
    //播放动作
    void PlayAction(string actionName);
    //旋转
    void ChangeRotate(Vector3 dir);
    //更新坐标
    void UpdatePos(Vector3 pos);
    //更新特效
    void UpdateEffect(EffectInfo info);

    void UpdateFrame();

}
