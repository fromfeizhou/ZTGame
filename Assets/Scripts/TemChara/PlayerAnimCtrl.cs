using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class PlayerAnimCtrl : MonoBehaviour
{
    private PlayerBase _playerBase;
    private Animation _playerAnim;

    private GameObject _colliderView;
	// Use this for initialization
	void Start () {
        _playerAnim = this.gameObject.GetComponent<Animation>();
	}

    void OnDestroy()
    {
        RemoveControlEvent();
#if UNITY_EDITOR
        if (null != _colliderView)
        {
            GameObject.Destroy(_colliderView);
            _colliderView = null;
        }
#endif
    }

	// Update is called once per frame
	void Update () {
		
	}
    //设置对象
    public void SetPlayerData(PlayerBase playerBase)
    {
        _playerBase = playerBase;
        InitControlEvent();
#if UNITY_EDITOR
        CreateColliderView();
#endif
    }

    //初始化控制事件
    private void InitControlEvent()
    {
        _playerBase.addEventListener(PlayerAnimEvents.PLAY, OnPlayHandler);
        _playerBase.addEventListener(PlayerAnimEvents.UPDATE_POS, OnUpdatePos);
    }
    //移除控制事件
    private void RemoveControlEvent()
    {
        _playerBase.removeEventListener(PlayerAnimEvents.PLAY, OnPlayHandler);
        _playerBase.removeEventListener(PlayerAnimEvents.UPDATE_POS, OnUpdatePos);
    }

    private void OnPlayHandler(Notification data)
    {
        string actoinName = (string)data.param;
        _playerAnim.Play(actoinName);
    }

    private void OnUpdatePos(Notification data)
    {
        UpdatePlayerRotation();
        this.gameObject.transform.localPosition = _playerBase.PlayerPos;
    }

    private void UpdatePlayerRotation()
    {
        this.gameObject.transform.localRotation = FightDefine.GetDirEuler(_playerBase.MoveDir);
    }

    private void CreateColliderView()
    {
        _colliderView = ZColliderView.CreateColliderView(_playerBase.Collider);
        _colliderView.transform.localPosition = new Vector3(_playerBase.Collider.x, 0.1f, _playerBase.Collider.y);
        _colliderView.transform.parent = this.gameObject.transform;
        _colliderView.transform.localRotation = Quaternion.Euler(Vector3.up * _playerBase.Collider.rotate);
    }

}
