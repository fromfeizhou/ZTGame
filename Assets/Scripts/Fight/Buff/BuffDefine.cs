using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//buff计数器
public class BuffCounter
{
    private Dictionary<int,List<Buff>> _buffListDic;
    private ICharaActor _chara;
    private Dictionary<int,int> _removeKeys;

    public BuffCounter(ICharaActor chara)
    {
        _buffListDic = new Dictionary<int, List<Buff>>();
        _chara = chara;
        _removeKeys = new Dictionary<int, int>();
    }

    public void Update()
    {
        foreach (int key in _buffListDic.Keys)
        {

            List<Buff> list = _buffListDic[key];
            int assetId = -1;
            for (int i = list.Count - 1; i >=0; i--)
            {
                Buff tmp = list[i];
                tmp.Update();
                if (tmp.IsExit)
                {
                    assetId = tmp.BuffEffectInfo.AssetKey;
                    tmp.Destroy();
                    list.RemoveAt(i);
                }
            }
            if (list.Count == 0)
            {
                _removeKeys.Add(key, assetId);
            }
        }
        if (_removeKeys.Count > 0)
        {
            foreach (int reKey in _removeKeys.Keys)
            {
                _buffListDic.Remove(reKey);
                RemoveBuffEffect(_removeKeys[reKey]);
            }
            _removeKeys.Clear();
        }
     
    }

    public void AddBuff(Buff buffInfo)
    {
        int buffId = buffInfo.BuffId;
        int assetId = -1;

        if(!_buffListDic.ContainsKey(buffId)){
            _buffListDic.Add(buffId, new List<Buff>());
            if (null != buffInfo.BuffEffectInfo && null != _chara)
            {
                _chara.AddEffect(buffInfo.BuffEffectInfo);
                assetId = buffInfo.BuffEffectInfo.AssetKey;
            }
        }
        //叠加buff
        if ( _buffListDic[buffId].Count > 0 )
        {
            assetId = _buffListDic[buffId][0].BuffEffectInfo.AssetKey;

            while(_buffListDic[buffId].Count >= buffInfo.MixMax){
                Buff tmp = _buffListDic[buffId][0];
                tmp.Destroy();
                _buffListDic[buffId].RemoveAt(0);
            }
        }
        buffInfo.BuffEffectInfo.AssetKey = assetId;
        _buffListDic[buffId].Add(buffInfo);
        
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
        int assetId = -1;
        foreach (Buff buff in list)
        {
            if (buff.BuffEffectInfo.AssetKey >= 0)
            {
                assetId = buff.BuffEffectInfo.AssetKey;
            }
            buff.Destroy();
        }
        RemoveBuffEffect(assetId);
        list.Clear();
        list = null;
    }

    private void RemoveBuffEffect(int assetId = -1)
    {
        if (assetId >= 0 && null != _chara)
        {
            _chara.RemoveEffect(assetId);
        }
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
