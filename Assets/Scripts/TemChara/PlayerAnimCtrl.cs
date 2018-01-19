using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class PlayerAnimCtrl : MonoBehaviour
{
    private PlayerBase _playerBase;
    private Animation _playerAnim;

    private GameObject _colliderView;

    private Dictionary<string, List<GameObject>> _effectPool;
    // Use this for initialization
    void Start()
    {
        _playerAnim = this.gameObject.GetComponent<Animation>();
        _effectPool = new Dictionary<string, List<GameObject>>();
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
    void Update()
    {

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
        _playerBase.addEventListener(PlayerAnimEvents.ADD_EFFECT, OnAddEffect);
        _playerBase.addEventListener(PlayerAnimEvents.REMOVE_EFFECT, OnRemoveEffect);
    }
    //移除控制事件
    private void RemoveControlEvent()
    {
        _playerBase.removeEventListener(PlayerAnimEvents.PLAY, OnPlayHandler);
        _playerBase.removeEventListener(PlayerAnimEvents.UPDATE_POS, OnUpdatePos);
        _playerBase.removeEventListener(PlayerAnimEvents.ADD_EFFECT, OnAddEffect);
        _playerBase.removeEventListener(PlayerAnimEvents.REMOVE_EFFECT, OnRemoveEffect);
    }

    //操作回调
    private void OnPlayHandler(Notification data)
    {
        string actoinName = (string)data.param;
        _playerAnim.Play(actoinName);
    }

    //刷新位置
    private void OnUpdatePos(Notification data)
    {
        UpdatePlayerRotation();
        this.gameObject.transform.localPosition = _playerBase.PlayerPos;
    }

    //添加特效
    private void OnAddEffect(Notification data)
    {
        string name = (string)data.param;
        FightEffectLib.GetEffectByName(name, (Object target, string path) =>
        {
            GameObject go = target as GameObject;
            if (null != go)
            {
                Transform effect = Instantiate(go).transform;
                effect.SetParent(this.gameObject.transform);
                effect.transform.localPosition = Vector3.zero;

                ParticleDestroy pds = go.GetComponent<ParticleDestroy>();
                if (null != pds)
                {
                    return;
                }
                //加入特效队列(非自动消亡)
                if (!_effectPool.ContainsKey(name))
                {
                    _effectPool[name] = new List<GameObject>();
                }
                _effectPool[name].Add(effect.gameObject);
            }
        });
    }

    //移除特效
    private void OnRemoveEffect(Notification data)
    {
        string name = (string)data.param;
        if (_effectPool.ContainsKey(name) && _effectPool[name].Count > 0)
        {
            GameObject go = _effectPool[name][0];
            Destroy(go);
            _effectPool[name].RemoveAt(0);
        }
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
