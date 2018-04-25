using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class ZTBaseScene : MonoBehaviour
{
    private Action<float> _update;
    private Action _destroy;
    public void SetUpdate(Action<float> update)
    {
        _update = update;
    }
    public void SetDestroy(Action destroy)
    {
        _destroy = destroy;
    }


    // Update is called once per frame
    void Update()
    {
        if (null != _update)
        {
            _update(Time.deltaTime);
        }
    }


    void OnDestroy()
    {
        if (null != _destroy)
        {
            _destroy();
        }
    }
}
