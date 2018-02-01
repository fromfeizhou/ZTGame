using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    private BuffInfo _buffInfo;
    private Transform _layer;
    private FightEffectCounter _effectCounter;
    /// <summary>
    /// buff 施法者
    /// buffId
    /// buff挂钩位置
    /// </summary>
    /// <param name="battleId"></param>
    /// <param name="buffId"></param>
    /// <param name="layer"></param>
    public Buff(int userId,int targetId,int buffId,int frame,Transform layer = null)
    {
        _layer = layer;
        _buffInfo = new BuffInfo(buffId, frame);
        _effectCounter = new FightEffectCounter();
        UpdateBuffEffect();
    }

    public void Update()
    {
        if (null != _buffInfo)
        {
            _buffInfo.Update();
        }
    }

    private void UpdateBuffEffect()
    {
        if (null == _layer || null == _buffInfo.EffectInfo || _buffInfo.EffectInfo.Id == "")
            return;
        _effectCounter.AddEffect(_buffInfo.EffectInfo, _layer);
    }

    public void Destroy()
    {
        if (null != _effectCounter)
        {
            _effectCounter.Destroy();
            _effectCounter = null;
        }
    }
}
