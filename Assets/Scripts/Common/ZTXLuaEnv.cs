using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[System.Serializable]
public class Injection
{
    public string name;
    public GameObject value;
}


public class ZTXLuaEnv : MonoSingleton<ZTXLuaEnv>
{
    internal static LuaEnv LuaEnv = new LuaEnv(); //all lua behaviour shared one luaenv only!
    internal static float LastGCTime = 0;
    internal const float GCInterval = 10;//1 second 

    public Dictionary<int, ItfSkillTab> CfgTabSkill;

    public override void Init()
    {
        base.Init();
        LuaEnv = new XLua.LuaEnv();
        this.LoadLuaConfigFile();

    }

    public void Update()
    {
        if (Time.time - ZTXLuaEnv.LastGCTime > GCInterval)
        {
            LuaEnv.Tick();
            ZTXLuaEnv.LastGCTime = Time.time;
        }
    }

    [CSharpCallLua]
    public interface ItfSkillTab
    {
        int id { get; set; }
        string name { get; set; }
        int acttorId { get; set; }
        Dictionary<int, LuaTable> test { get; set; }
    }
    //加载lua
    private void LoadLuaConfigFile()
    {
        AssetManager.LoadAsset(PathManager.GetResPathByName("LuaConfig", "tab_skill.lua.txt"), (UnityEngine.Object target, string path) =>
        {
            TextAsset file = target as TextAsset;
            LuaEnv.DoString(file.text);
            Dictionary<string, LuaTable> tab_skill = LuaEnv.Global.Get<Dictionary<string, LuaTable>>("tab_skill");

            CfgTabSkill = new Dictionary<int, ItfSkillTab>();

            foreach (string key in tab_skill.Keys)
            {
                ItfSkillTab tab = tab_skill[key].Cast<ItfSkillTab>();
                CfgTabSkill[tab.id] = tab;
            }
        });
    }

    public override void Destroy()
    {
        base.Destroy();
        if (null != LuaEnv)
        {
            Debug.Log("LuaEnv Destroy");
            LuaEnv.Dispose();
            LuaEnv = null;
        }
    }
}
