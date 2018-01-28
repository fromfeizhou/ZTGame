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
        this.dispatchEvent(CHARA_EVENT.PLAY, new Notification(actionName));
    }
    //更新角度
    public void ChangeRotate(Vector3 dir)
    {
        this.dispatchEvent(CHARA_EVENT.CHANGE_ROTATE, new Notification(dir));
    }
    //更新位置
    public void UpdatePos(Vector3 pos)
    {
        this.dispatchEvent(CHARA_EVENT.UPDATE_POS, new Notification(pos));
    }
    //更新特效
    public void AddEffect(EffectInfo info)
    {
        this.dispatchEvent(CHARA_EVENT.ADD_EFFECT, new Notification(info));
    }
    public void RemoveEffect(int assetId)
    {
        this.dispatchEvent(CHARA_EVENT.REMOVE_EFFECT, new Notification(assetId));
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
        this.dispatchEvent(CHARA_EVENT.DESTROY);
        Debug.Log("CharaActorInfo Destroy");
    }


}
