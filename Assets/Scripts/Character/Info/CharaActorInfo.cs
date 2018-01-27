using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaActorInfo : NotificationDelegate, ICharaActor
{
    //**===================接口实现=======================**//
    public int AnimaId { get; set; }
    public CHARA_TYPE CharaType { get; set; }
    
    public virtual void PlayAction(string actionName)
    {
        this.dispatchEvent(CharaEvents.PLAY, new Notification(actionName));
    }
    //更新角度
    public void ChangeRotate(Vector3 dir)
    {
        this.dispatchEvent(CharaEvents.CHANGE_ROTATE, new Notification(dir));
    }
    //更新位置
    public void UpdatePos(Vector3 pos)
    {
        this.dispatchEvent(CharaEvents.UPDATE_POS, new Notification(pos));
    }
    //更新特效
    public void UpdateEffect(EffectInfo info)
    {
        this.dispatchEvent(CharaEvents.UPDATE_EFFECT, new Notification(info));
    }

    //每帧刷新
    public virtual void UpdateFrame()
    {

    }
    //**===================接口实现=======================**//

    public CharaActorInfo(int animaId, CHARA_TYPE type)
    {
        AnimaId = animaId;
        CharaType = type;
    }

    public virtual void Destroy()
    {
        this.dispatchEvent(CharaEvents.DESTROY);
        Debug.Log("CharaActorInfo Destroy");
    }


}
