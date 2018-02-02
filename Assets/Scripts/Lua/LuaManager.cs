using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuaManager : Singleton<LuaManager>
{
    private XLua.LuaEnv _luaenv;
    public XLua.LuaEnv Luaenv
    {
        get { return _luaenv; }
    }

    public override void Init()
    {
        _luaenv = new XLua.LuaEnv();
    }

    public override void Destroy()
    {
        _luaenv.Dispose();
    }

}
