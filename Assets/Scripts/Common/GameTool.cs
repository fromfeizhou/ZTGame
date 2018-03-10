using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class GameTool
{
    public static void Log(params object[] args)
    {
        string str = "";

        for (int i = 0; i < args.Length; i++)
        {
            if (i == 0)
            {
                if (null != args[i])
                    str = args[i].ToString();
                else
                    str = "null";
            }
            else
            {
                if (null != args[i])
                    str = str + "  " + args[i].ToString();
                else
                    str = str + "  " + "null";
            }
        }
        Debug.Log(str);
    }
}
