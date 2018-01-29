using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//buff计数器
public class BUFFCOUNTER
{
    private Dictionary<int,List<Buff>> _buffListDic;

    public BUFFCOUNTER()
    {
        _buffListDic = new Dictionary<int, List<Buff>>();
    }

    public void AddBuff(int userId,int targetId,int buffId,int frame,Transform layer = null){
        
    }

    //按id移除buff
    public void RemoveBuffById()
    {
    }

    public void RemoveBuffByType()
    {
    }

    private void RmoveBuffByList(List<Buff>  list)
    {
        foreach (Buff buff in list)
        {
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

//buff类型
public enum BUFF_TYPE
{
    NORMAL,       //常规
}

public class BuffDefine{
}
