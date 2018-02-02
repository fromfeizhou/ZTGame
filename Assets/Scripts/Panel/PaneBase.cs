using System;
using UnityEngine;
using XLua;

public class PanelBase
{
    private GameObject _panel;
    public GameObject gameObject
    {
        get { return _panel; }
    }
    public void Init(string luaScript, GameObject panelGo)
    {
        _panel = panelGo;

        LuaTable scriptEnv = LuaManager.GetInstance().Luaenv.NewTable();

        LuaTable meta = LuaManager.GetInstance().Luaenv.NewTable();
        meta.Set("__index", LuaManager.GetInstance().Luaenv.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        LuaManager.GetInstance().Luaenv.DoString(luaScript, "PanelBase", scriptEnv);

        scriptEnv.Set("self", this);
        scriptEnv.Set("widget", Widget.Create(panelGo));
        scriptEnv.Get("update", out luaUpdate);

        Action luaInitPanel = scriptEnv.Get<Action>("init_panel");
        if (luaInitPanel != null)
            luaInitPanel();
    }

    private Action luaUpdate;

    public void Update()
    {
        if (luaUpdate != null)
        {
            luaUpdate();
        }
    }

    private void OnClick()
    {
        Debug.Log("OnClick");
    }
}
