using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTool
{
    public static void Log(params object[] args)
    {
        string str = "";

        for (int i = 0; i < args.Length; i++)
        {
            if (i == 0)
            {
                str = args[i].ToString();
            }
            else
            {
                str = str + "  " + args[i].ToString();
            }
        }
        Debug.Log(str);
    }
}
