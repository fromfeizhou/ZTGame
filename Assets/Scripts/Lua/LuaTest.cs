using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuaTest : MonoBehaviour
{
    private LuaManager _luaMgr;
    
	// Use this for initialization
	void Start ()
	{
	    _luaMgr = LuaManager.GetInstance();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartUpLua();
        }

        if (panelBase != null)
            panelBase.Update();
    }

    public void StartUpLua()
    {
        if (_luaMgr.Luaenv != null)
        {
            _luaMgr.Destroy();
        }
        _luaMgr.Init();
        LoadLuaCenter();
    }

    private PanelBase panelBase;
    private void LoadLuaCenter()
    {
        string luaPBPath = "Assets/ResourcesLib/LuaScripts/claine_pb.lua";
        string luaPBScript = System.IO.File.ReadAllText(luaPBPath);
        _luaMgr.Luaenv.DoString(luaPBScript);

        string luaScriptPath = "Assets/ResourcesLib/LuaScripts/Panel_Login.lua";
        string luaScript = System.IO.File.ReadAllText(luaScriptPath);

        panelBase = new PanelBase();
        panelBase.Init(luaScript,gameObject);
    }


}
