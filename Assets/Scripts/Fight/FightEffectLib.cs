using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightEffectLib {

    //获取预设
    public static void GetEffectByName(string name, UnityEngine.Events.UnityAction<Object, string> callback = null)
    {
        string path = GetEffectName(name);
        AssetManager.LoadAsset(path,callback);
    }

    //获取特效预设名字
    private static string GetEffectName(string name)
    {
        string path = PathManager.GetResPathByName("EffectPrefab", name + ".prefab");
        return path;
    }

   
}
