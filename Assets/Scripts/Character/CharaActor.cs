using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class CharaActor : MonoBehaviour {
    //动画数据
    protected CharaActorInfo _charaInfo;
    //动画
    private Animation _anima;

    private Dictionary<string, List<GameObject>> _effectPool;


    public virtual bool SetInfo(CharaActorInfo info){
        if (null != _charaInfo && null != info && _charaInfo.AnimaId == info.AnimaId) return false;
        RemoveEvent();
        this.ClearEffect();
        _anima = this.gameObject.GetComponent<Animation>();
        _charaInfo = info;
        _effectPool = new Dictionary<string, List<GameObject>>();
        InitEvent();
        return true;
    }

    public virtual void InitEvent(){
        if (null == _charaInfo) return;

        _charaInfo.addEventListener(CharaEvents.PLAY, OnPlayHandler);
        _charaInfo.addEventListener(CharaEvents.UPDATE_POS, OnUpdatePos);
        _charaInfo.addEventListener(CharaEvents.CHANGE_ROTATE, OnChangeRotation);
        _charaInfo.addEventListener(CharaEvents.UPDATE_EFFECT, OnUpdateEffect);
        _charaInfo.addEventListener(CharaEvents.CHANGE_ANIM, OnChangeAnim);
    }

    public virtual void RemoveEvent(){
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
            FightEffectLib.GetEffectByName(info.Id, (Object target, string path) =>
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
                    if (!_effectPool.ContainsKey(info.Id))
                    {
                        _effectPool[info.Id] = new List<GameObject>();
                    }
                    _effectPool[info.Id].Add(effect.gameObject);
                }
            });
        }
        else
        {
            if (_effectPool.ContainsKey(info.Id) && _effectPool[info.Id].Count > 0)
            {
                GameObject go = _effectPool[info.Id][0];
                Destroy(go);
                _effectPool[info.Id].RemoveAt(0);
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
        this.SetInfo(null);
        this.ClearEffect();
    }

    private void ClearEffect()
    {
        if (null != _effectPool)
        {
            foreach (List<GameObject> val in _effectPool.Values)
            {
                for (int i = val.Count - 1; i >= 0; i--)
                {
                    Destroy(val[i]);
                }
            }
            _effectPool.Clear();
            _effectPool = null;
        }
       
    }
    #endregion
}
