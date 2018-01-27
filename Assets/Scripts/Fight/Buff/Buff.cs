using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{

    private BuffInfo _buffInfo;
    private Transform _layer;
    private int _effectKey;
    public Buff(Transform layer = null)
    {
        _layer = layer;
    }

    public void SetInfo(BuffInfo buffInfo)
    {
        _buffInfo = buffInfo;
        UpdateBuffEffect();
    }

    public void UpdateBuffEffect()
    {
        if (null == _layer || null == _buffInfo.EffectInfo || _buffInfo.EffectInfo.Id == "")
            return;
        _effectKey = FightEffectManager.GetInstance().AddEffectByInfo(_buffInfo.EffectInfo,_layer);
    }

    public void Destroy()
    {
        if (_effectKey > 0)
        {
            FightEffectManager.GetInstance().RemoveEffectInfo(_effectKey);
            _effectKey = 0;
        }
    }
}
