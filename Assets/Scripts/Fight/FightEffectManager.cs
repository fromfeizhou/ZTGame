using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//特效计数器
public class FightEffectCounter
{
    private List<int> _effectAsset;

    public FightEffectCounter()
    {
        _effectAsset = new List<int>();
    }

    public void AddEffect(EffectInfo info, Transform layer = null)
    {
        if (null != _effectAsset)
        {
            FightEffectManager.GetInstance().AddEffectByInfo(info, layer);
            if (info.AssetKey > 0)
            {
                _effectAsset.Add(info.AssetKey);
            };
        }
    }

    public void RemoveEffectByKey(int key)
    {
        FightEffectManager.GetInstance().RemoveEffectInfo(key);
    }

    public void ClearEffect()
    {
        if (null != _effectAsset)
        {
            FightEffectManager.GetInstance().RemoveEffectInfo(_effectAsset);
            _effectAsset.Clear();
        }
    }

    public void Destroy()
    {
        ClearEffect();
        _effectAsset = null;
    }
}

public class FightEffectManager : Singleton<FightEffectManager>
{
    private Dictionary<int, GameObject> _effectDic;
    private int _effectKey;

    public override void Init()
    {
        _effectDic = new Dictionary<int, GameObject>();
        _effectKey = 0;
        base.Init();
    }

    public override void Destroy()
    {
        if (null != _effectDic)
        {
            foreach (int key in _effectDic.Keys)
            {
                GameObject.Destroy(_effectDic[key]);
            }
            _effectDic.Clear();
            _effectDic = null;
        }
        base.Destroy();
    }

    //获取特效预设
    public void AddEffectByInfo(EffectInfo info, Transform layer = null)
    {
        if (info.Id == "") return;

        if (null == layer)
        {
            layer = GameObject.Find("PlayerLayer").transform;
        }
        string effectPath = GetEffectName(info.Id);
        int effectKey = -1;
        //存在声明周期的特效
        if (info.LifeTime <= 0)
        {
            effectKey = GetEffectDicKey();
            info.AssetKey = effectKey;
            //数据层启动 占位 资源加载完毕 覆盖
            _effectDic[effectKey] = null;
        }
        AssetManager.LoadAsset(effectPath, (Object target, string path) =>
        {
            GameObject go = target as GameObject;
            //管理器清除 或者占位符 已经移除 特效已经不存在 不需要创建
            if (null != go && null != layer && null != _effectDic && _effectDic.ContainsKey(effectKey))
            {
                Transform effect = GameObject.Instantiate(go).transform;
                effect.transform.localPosition = new Vector3(info.Offset.x, 0.1f, info.Offset.y);
                effect.transform.localRotation = Quaternion.Euler(Vector3.up * info.Rotate);
                effect.transform.SetParent(layer, false);
                if (info.LifeTime > 0)
                {
                    ParticleDestroy pds = go.GetComponent<ParticleDestroy>();
                    if (null == pds)
                    {
                        pds = go.AddComponent<ParticleDestroy>();
                    }
                    pds.lifetime = info.LifeTime;
                }
                else
                {
                    _effectDic[effectKey] = effect.gameObject;
                }
               
            }
        });
    }

    /// <summary>
    /// 清理管理器的特效
    /// </summary>
    /// <param name="list"></param>
    public void RemoveEffectInfo(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            RemoveEffectInfo(list[i]);
        }
    }

    /// <summary>
    /// 清理管理器的特效
    /// </summary>
    /// <param name="key"></param>
    public void RemoveEffectInfo(int key)
    {
        if (null != _effectDic && _effectDic.ContainsKey(key))
        {
            //排除占位符号
            if (null != _effectDic[key])
            {
                GameObject.Destroy(_effectDic[key]);
            }
            _effectDic.Remove(key);
        }
    }

    private int GetEffectDicKey()
    {
        while (_effectDic.ContainsKey(_effectKey))
        {
            _effectKey++;
            if (_effectKey > FightDefine.MaxFrame) _effectKey = 0;
        }
        return _effectKey;
    }

    //获取特效预设名字
    private string GetEffectName(string name)
    {
        string path = PathManager.GetResPathByName("EffectPrefab", name + ".prefab");
        return path;
    }


}
