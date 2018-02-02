using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 战斗场景形象
/// </summary>
public class PlayerBattleActor : PlayerActor {
    protected PlayerBattleInfo _battleInfo;

    private GameObject _colliderView;

    public override bool SetInfo(CharaActorInfo info)
    {
        if (base.SetInfo(info))
        {
            _battleInfo = info as PlayerBattleInfo;
#if UNITY_EDITOR
            UpdateColliderView();
#endif
            return true;
        }
        return false;
    }

    public override void InitEvent()
    {
        base.InitEvent();
        if (null == _charaInfo) return;

        _charaInfo.addEventListener(CHARA_EVENT.ADD_BUFF, OnAddBuffHandler);
        _charaInfo.addEventListener(CHARA_EVENT.REMOVE_BUFF, OnRemoveBuffHandler);
       
    }

    public override void RemoveEvent()
    {
        base.RemoveEvent();
        if (null == _charaInfo) return;

        _charaInfo.removeEventListener(CHARA_EVENT.ADD_BUFF, OnAddBuffHandler);
        _charaInfo.removeEventListener(CHARA_EVENT.REMOVE_BUFF, OnRemoveBuffHandler);
    }

    //添加buff
    public void OnAddBuffHandler(Notification data)
    {
        _battleInfo.AddBuff(data.param as BuffData);
    }

    public void OnRemoveBuffHandler(Notification data)
    {
        int buffId = (int)data.param;
        _battleInfo.RemoveBuff(buffId);
    }

    private void UpdateColliderView()
    {
        if (null != _battleInfo)
        {
            ClearColliderView();
            _colliderView = ZColliderView.CreateColliderView(_battleInfo.Collider);
            _colliderView.transform.localPosition = new Vector3(_battleInfo.Collider.x, 0.1f, _battleInfo.Collider.y);
            _colliderView.transform.SetParent(this.gameObject.transform, false);
            _colliderView.transform.localRotation = Quaternion.Euler(Vector3.up * _battleInfo.Collider.rotate);
        }
        else
        {
            ClearColliderView();
        }
       
    }

    private void ClearColliderView()
    {
        if (null != _colliderView)
        {
            GameObject.Destroy(_colliderView);
            _colliderView = null;
        }
    }
}
