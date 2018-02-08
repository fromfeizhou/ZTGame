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
    //特效资源记录(一般为 技能特殊添加特效)
    private EffectCounter _effectCounter;


    public virtual bool SetInfo(CharaActorInfo info)
    {
        if (null != _charaInfo && null != info && _charaInfo.AnimaId == info.AnimaId) return false;
        RemoveEvent();
        this.ClearEffect();
        _charaInfo = info;

        if (null == _charaInfo) return true;

        _effectCounter = new EffectCounter();
        InitEvent();
        return true;
    }

    public virtual void InitEvent()
    {
        if (null == _charaInfo) return;

        _charaInfo.addEventListener(CHARA_EVENT.PLAY, OnPlayHandler);
        _charaInfo.addEventListener(CHARA_EVENT.UPDATE_POS, OnUpdatePos);
        _charaInfo.addEventListener(CHARA_EVENT.CHANGE_ROTATE, OnChangeRotation);
        _charaInfo.addEventListener(CHARA_EVENT.ADD_EFFECT, OnAddEffect);
        _charaInfo.addEventListener(CHARA_EVENT.REMOVE_EFFECT, OnRemoveEffect);
        _charaInfo.addEventListener(CHARA_EVENT.CHANGE_ANIM, OnChangeAnim);
        _charaInfo.addEventListener(CHARA_EVENT.CHANGE_OPACITY, OnChangeOpacity);
    }

    public virtual void RemoveEvent()
    {
        if (null == _charaInfo) return;

        _charaInfo.removeEventListener(CHARA_EVENT.PLAY, OnPlayHandler);
        _charaInfo.removeEventListener(CHARA_EVENT.UPDATE_POS, OnUpdatePos);
        _charaInfo.removeEventListener(CHARA_EVENT.CHANGE_ROTATE, OnChangeRotation);
        _charaInfo.removeEventListener(CHARA_EVENT.ADD_EFFECT, OnAddEffect);
        _charaInfo.removeEventListener(CHARA_EVENT.REMOVE_EFFECT, OnRemoveEffect);
        _charaInfo.removeEventListener(CHARA_EVENT.CHANGE_ANIM, OnChangeAnim);
        _charaInfo.removeEventListener(CHARA_EVENT.CHANGE_OPACITY, OnChangeOpacity);
    }

    //改变透明度
    public void OnChangeOpacity(Notification data)
    {
        float opcacity = (float)data.param;
        Debug.Log(opcacity);
        if (opcacity > 0)
        {
            _anima.gameObject.SetActive(true);
           SkinnedMeshRenderer render = _anima.gameObject.transform.Find("equitPos").GetComponent<SkinnedMeshRenderer>();
           Color color = render.material.color;
           Debug.Log(color.r);
           Debug.Log(color.g);
           Debug.Log(color.b);
           render.material.color = new Color(color.r, color.g, color.b, opcacity);
        }
        else
        {
            _anima.gameObject.SetActive(false);
        }
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
    private void OnAddEffect(Notification data)
    {
        EffectInfo info = (EffectInfo)data.param;
        _effectCounter.AddEffect(info, this.gameObject.transform);
    }

    private void OnRemoveEffect(Notification data)
    {
        int assetId = (int)data.param;
        _effectCounter.RemoveEffectByKey(assetId);
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
        if (null != _effectCounter)
        {
            _effectCounter.Destroy();
            _effectCounter = null;
        }

    }
    #endregion
}
