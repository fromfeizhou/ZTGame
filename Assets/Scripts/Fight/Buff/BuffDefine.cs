using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//buff计数器
public class BuffCounter
{
    private Dictionary<int,List<Buff>> _buffListDic;
    private ICharaActor _chara;

    public BuffCounter(ICharaActor chara)
    {
        _buffListDic = new Dictionary<int, List<Buff>>();
        _chara = chara;
    }

    public void AddBuff(Buff buffInfo)
    {
        int buffId = buffInfo.BuffId;
        if(!_buffListDic.ContainsKey(buffId)){
            _buffListDic.Add(buffId, new List<Buff>());   
        }
        _buffListDic[buffId].Add(buffInfo);
        if (null != buffInfo.BuffEffectInfo && null != _chara)
        {
            _chara.AddEffect(buffInfo.BuffEffectInfo);
        }
    }

    //按id移除buff
    public void RemoveBuffById(int buffId)
    {
        if (!_buffListDic.ContainsKey(buffId)) return;

       RmoveBuffByList(_buffListDic[buffId]);
        _buffListDic.Remove(buffId);
    }

    //按类型移除
    public void RemoveBuffByType(int type)
    {
        foreach (int key in _buffListDic.Keys)
        {
            List<Buff> list = _buffListDic[key];
            if (list.Count > 0)
            {
                Buff buffInfo = list[0];
                if (buffInfo.BuffType == type)
                {
                    RemoveBuffById(buffInfo.BuffId);
                }
            }
        }
    }

    private void RmoveBuffByList(List<Buff> list)
    {
        foreach (Buff buff in list)
        {
            if (null != buff.BuffEffectInfo && buff.BuffEffectInfo.AssetKey > 0 && null != _chara)
            {
                _chara.RemoveEffect(buff.BuffEffectInfo.AssetKey);
            }
            buff.Destroy();
        }
        list.Clear();
        list = null;
    }

    public void Destroy()
    {
        if (null != _buffListDic)
        {
            foreach (int buffId in _buffListDic.Keys)
            {
                RmoveBuffByList(_buffListDic[buffId]);
            }
            _buffListDic.Clear();
            _buffListDic = null;
        }
    }
}

//buff类型 配置表配置
public enum BUFF_TYPE
{
    NORMAL,       //常规
}

public class BuffData
{
    public int BuffId;
    public int Frame;
    public int UserId;

    public BuffData(int buffId, int frame, int userId = -1)
    {
        BuffId = buffId;
        Frame = frame;
        UserId = userId;
    }
}


public class BuffDefine{

    public static Buff GetBuffInfo(BuffData buffdata)
    {
        ////读取配置表获取类型
        //BUFF_TYPE type = BUFF_TYPE.NORMAL;
        //switch (type)
        //{
        //    case BUFF_TYPE.NORMAL:
        //        return new Buff(buffdata);
        //}
        return new Buff(buffdata);
    }
}
