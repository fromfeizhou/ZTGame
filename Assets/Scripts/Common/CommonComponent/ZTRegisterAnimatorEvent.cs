using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[XLua.LuaCallCSharp]
public class ZTRegisterAnimatorEvent : MonoBehaviour
{
    public Dictionary<string, System.Action> _callback = new Dictionary<string, System.Action>();


    public void Register(System.Action _callback, string _animName, float _percentage)
    {
        string tempKey = _animName + _percentage;
        this._callback[tempKey] = _callback;
        Animator anim = GetComponent<Animator>();
        var clips = anim.runtimeAnimatorController.animationClips;
        foreach (var item in clips)
        {
            if (item.name.Equals(_animName))
            {
                var events = item.events;
                string functionName = "OnComplete";
                bool find = false;
                foreach (var e in events)
                {
                    if (e.functionName.Equals(functionName) && e.time == item.length * _percentage)
                    {
                        find = true;
                        break;
                    }

                }
                if (!find)
                {
                    AnimationEvent ae = new AnimationEvent();
                    ae.functionName = "OnComplete";
                    ae.messageOptions = SendMessageOptions.RequireReceiver;
                    ae.time = item.length * _percentage;
                    ae.stringParameter = tempKey;
                    item.AddEvent(ae);
                }
            }
        }
    }

    void OnComplete(string tempKey)
    {
        if (_callback != null)
        {
            this._callback[tempKey].Invoke();
        }
    }
}