using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFaceCache
{
    private static Dictionary<int,SpriteAsset> _spAssetDic = null;
    private static int _loadIndex = 0;
    private static List<string> _pathList = null;

    public static void ParseAsset()
    {
        _spAssetDic = new Dictionary<int, SpriteAsset>();
        string path = System.IO.Path.Combine(PathManager.GetResPath("FaceSpAsset"), "emoji.asset");
        string path2 = System.IO.Path.Combine(PathManager.GetResPath("FaceSpAsset"), "emoji_lxh.asset");
        _pathList = new List<string>();
        _pathList.Add(path);
        _pathList.Add(path2);
        _loadIndex = 0;
        LoadAsset();
    }

    private static void LoadAsset()
    {
        string path = _pathList[_loadIndex];
        AssetManager.LoadAsset(path, LoadAssetCom);
    }

    private static void LoadAssetCom(Object target, string path)
    {
        SpriteAsset spAsset = target as SpriteAsset;
        if (null != spAsset)
        {
            _spAssetDic[spAsset.ID] = spAsset;
        }
        _loadIndex++;
        if (_loadIndex >= _pathList.Count)
        {
            GameStartEvent.GetInstance().dispatchEvent(GAME_LOAD_SETP_EVENT.LOAD_FACE_ASSET);
            return;
        }
        LoadAsset();
    }

    public static SpriteAsset GetAsset(int index)
    {
            if (null != _spAssetDic && _spAssetDic.ContainsKey(index))
        {
            return _spAssetDic[index];
        }
        return null;
    }

    public static SpriteInforGroup GetAsset(int index, string name)
    {
        if (null != _spAssetDic &&_spAssetDic.ContainsKey(index))
        {
            foreach (SpriteInforGroup spriteInforGroup in _spAssetDic[index].listSpriteGroup)
            {
                if (spriteInforGroup.tag == name)
                {
                    return spriteInforGroup;
                }
            }
            return _spAssetDic[index].listSpriteGroup[0];
        }
        return null;
    }

    public static void Destory()
    {
        _spAssetDic = null;
    }
}
