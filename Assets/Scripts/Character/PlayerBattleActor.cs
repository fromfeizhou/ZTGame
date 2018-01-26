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
    }

    public override void RemoveEvent()
    {
        base.RemoveEvent();
    }

    private void UpdateColliderView()
    {
        if (null != _battleInfo)
        {
            ClearColliderView();
            _colliderView = ZColliderView.CreateColliderView(_battleInfo.Collider);
            _colliderView.transform.localPosition = new Vector3(_battleInfo.Collider.x, 0.1f, _battleInfo.Collider.y);
            //_colliderView.transform.parent = this.gameObject.transform;
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
