﻿using UnityEngine;
using XLua;

public class LuaMainScene : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        LuaEnv luaenv = new LuaEnv();
        luaenv.DoString("CS.UnityEngine.Debug.Log('hello world')");
        luaenv.Dispose();
    }

    // Update is called once per frame
    void Update()
    {

    }
}