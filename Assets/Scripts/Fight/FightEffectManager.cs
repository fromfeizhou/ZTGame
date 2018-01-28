using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (info.LifeTime > 0)
        {
            effectKey = GetEffectDicKey();
            info.AssetKey = effectKey;
        }
        AssetManager.LoadAsset(effectPath, (Object target, string path) =>
        {
            GameObject go = target as GameObject;

            if (null != go && null != layer)
            {
                Transform effect = GameObject.Instantiate(go).transform;
                effect.transform.localPosition = new Vector3(info.Offset.x, 0.1f, info.Offset.y);
                effect.transform.localRotation = Quaternion.Euler(Vector3.up * info.Rotate);
                effect.transform.SetParent(layer, false);
                if (info.AssetKey > 0)
                {
                    ParticleDestroy pds = go.GetComponent<ParticleDestroy>();
                    if (null == pds)
                    {
                        pds = go.AddComponent<ParticleDestroy>();
                    }
                    pds.lifetime = info.LifeTime;
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
            GameObject.Destroy(_effectDic[key]);
            _effectDic.Remove(key);
        }
    }

    private int GetEffectDicKey()
    {
        _effectKey++;
        if (_effectKey > FightDefine.MaxFrame) _effectKey = 0;
        return _effectKey;
    }

    //获取特效预设名字
    private string GetEffectName(string name)
    {
        string path = PathManager.GetResPathByName("EffectPrefab", name + ".prefab");
        return path;
    }


}
