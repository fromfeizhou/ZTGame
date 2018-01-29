using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//带形象的buff
public class Buff :BuffBase
{
    private Transform _layer;
    public EffectInfo BuffEffectInfo;   //特效
    private FightEffectCounter _effectCounter;

    public Buff(int buffId,int frame,int userId = -1,Transform layer = null):base(buffId,frame,userId)
    {
        _layer = layer;

        BuffEffectInfo = new EffectInfo("27_RFX_Magic_FlameSwirl1");
    }

    public override void Start()
    {
        base.Start();
        if (null == _layer || null == BuffEffectInfo || BuffEffectInfo.Id == "")
            return;
        _effectCounter = new FightEffectCounter();
        _effectCounter.AddEffect(BuffEffectInfo, _layer);
    }

    public override void Update()
    {
        base.Update();
    }


    public override void Destroy()
    {
        if (null != _effectCounter)
        {
            _effectCounter.Destroy();
            _effectCounter = null;
        }
        base.Destroy();
    }
  
}
