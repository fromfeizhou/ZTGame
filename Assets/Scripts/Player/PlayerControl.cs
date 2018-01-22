using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class PlayerControl : MonoBehaviour
{
    public PlayerBase _playerBase = null;
    public Animation _playerAnimation = null;
    void Awake()
    {
        //_playerBase = new PlayerBase();
        //_playerAnimation.GetComponent<PlayerAnimCtrl>().SetPlayerData(_playerBase);
      
    }
    // Use this for initialization

    public void SetPlayerData(PlayerBase playerData)
    {
        _playerBase = playerData;
        _playerBase.Target = gameObject;
        gameObject.GetComponent<PlayerAnimCtrl>().SetPlayerData(_playerBase);
        if (null != _playerBase && _playerBase.Id == 1)
        {
            InitControlEvent();
        }
    }
    void Start()
    {

    }

    void OnDestroy()
    {
        if (null != _playerBase && _playerBase.Id == 1)
        {
            RemoveControlEvent();
            _playerBase = null;
        }
    }

    // Update is called once per frame
    void Update()
    {


    }

    //初始化控制事件
    private void InitControlEvent()
    {
        TouchEvent.GetInstance().addEventListener(GameTouchEvents.JOY_MOVE, OnJoveMoveHandler);
    }

    //移除控制事件
    private void RemoveControlEvent()
    {
        TouchEvent.GetInstance().removeEventListener(GameTouchEvents.JOY_MOVE, OnJoveMoveHandler);
    }

    public eMapBlockType MapBlockType;

    private void OnJoveMoveHandler(Notification data)
    {
        if (null == _playerBase) return;

        FightDefine.PLAYERDIR dir = (FightDefine.PLAYERDIR)data.param;
        _playerBase.StartMove(dir);
        MapBlockType = MapManager.GetInstance().GetFloorColl(_playerBase.PlayerPos);
    }

}
