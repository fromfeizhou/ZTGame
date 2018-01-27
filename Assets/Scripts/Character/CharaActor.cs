using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class CharaActor : MonoBehaviour
{
    //动画数据
    protected CharaActorInfo _charaInfo;
    //动画
    private Animation _anima;

    private Dictionary<string, List<int>> _effectKeys;


    public virtual bool SetInfo(CharaActorInfo info)
    {
        if (null != _charaInfo && null != info && _charaInfo.AnimaId == info.AnimaId) return false;
        RemoveEvent();
        this.ClearEffect();
        _charaInfo = info;

        if (null == _charaInfo) return true;

        _effectKeys = new Dictionary<string, List<int>>();
        InitEvent();
        return true;
    }

    public virtual void InitEvent()
    {
        if (null == _charaInfo) return;

        _charaInfo.addEventListener(CharaEvents.PLAY, OnPlayHandler);
        _charaInfo.addEventListener(CharaEvents.UPDATE_POS, OnUpdatePos);
        _charaInfo.addEventListener(CharaEvents.CHANGE_ROTATE, OnChangeRotation);
        _charaInfo.addEventListener(CharaEvents.UPDATE_EFFECT, OnUpdateEffect);
        _charaInfo.addEventListener(CharaEvents.CHANGE_ANIM, OnChangeAnim);
    }

    public virtual void RemoveEvent()
    {
        if (null == _charaInfo) return;

        _charaInfo.removeEventListener(CharaEvents.PLAY, OnPlayHandler);
        _charaInfo.removeEventListener(CharaEvents.UPDATE_POS, OnUpdatePos);
        _charaInfo.removeEventListener(CharaEvents.CHANGE_ROTATE, OnChangeRotation);
        _charaInfo.removeEventListener(CharaEvents.UPDATE_EFFECT, OnUpdateEffect);
        _charaInfo.removeEventListener(CharaEvents.CHANGE_ANIM, OnChangeAnim);
    }
    //操作回调
    private void OnPlayHandler(Notification data)
    {
        string actoinName = (string)data.param;
        _anima.Play(actoinName);
    }

    //刷新位置
    private void OnUpdatePos(Notification data)
    {
        Vector3 pos = (Vector3)data.param;
        this.gameObject.transform.localPosition = pos;
    }

    //添加移除特效
    private void OnUpdateEffect(Notification data)
    {
        EffectInfo info = (EffectInfo)data.param;

        if (info.IsAdd)
        {
            //特效已经存在(过滤)
            if (_effectKeys.ContainsKey(info.Id))
            {
                _effectKeys[info.Id].Add(_effectKeys[info.Id][0]);
                return;
            }

            int effectKey = FightEffectManager.GetInstance().AddEffectByInfo(info, this.gameObject.transform);
            if (effectKey > 0)
            {
                if (!_effectKeys.ContainsKey(info.Id))
                {
                    _effectKeys.Add(info.Id, new List<int>());
                }

                _effectKeys[info.Id].Add(effectKey);
            }
        }
        else
        {
            if (_effectKeys.ContainsKey(info.Id))
            {
                //计数-1
                if (_effectKeys[info.Id].Count > 1)
                {
                    _effectKeys[info.Id].RemoveAt(0);
                }
                else
                {
                    //清理特效
                    int effectKey = _effectKeys[info.Id][0];
                    FightEffectManager.GetInstance().RemoveEffectInfo(effectKey);
                    _effectKeys[info.Id].Clear();
                    _effectKeys.Remove(info.Id);
                }
            }
        }

    }

    //更新角度
    private void OnChangeRotation(Notification data)
    {
        Vector3 dir = (Vector3)data.param;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        this.gameObject.transform.localRotation = Quaternion.Euler(Vector3.up * angle);
    }

    //动画改变（变身 换装等）
    private void OnChangeAnim(Notification data)
    {
    }

    #region MonoBehaviour
    public virtual void Awake()
    {
        _anima = this.gameObject.GetComponent<Animation>();

    }

    public virtual void OnDestroy()
    {
        Debug.Log("CharaActor OnDestroy");
        this.SetInfo(null);
        this.ClearEffect();
    }

    private void ClearEffect()
    {
        if (null != _effectKeys)
        {
            foreach (List<int> val in _effectKeys.Values)
            {
                if (val.Count > 0)
                {
                    int effectKey = val[0];
                    FightEffectManager.GetInstance().RemoveEffectInfo(effectKey);
                }
                val.Clear();
            }
            _effectKeys.Clear();
            _effectKeys = null;
        }

    }
    #endregion
}
