using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using XLua;

[LuaCallCSharp]
public class Widget
{
    private GameObject _panel;
    public static Widget Create(GameObject panel)
    {
        Debug.Log("CreateWidget:" + panel.name);
        Widget widget = new Widget();
        widget._panel = panel;
        return widget;
    }

    private Widget()
    {

    }

    public void BandingToggle_OnValueChange(UnityAction<bool> degAction, string path = "")
    {
        GetComponent<Toggle>(path).onValueChanged.AddListener(degAction);
    }
    public void BandingBtn_OnClick(UnityAction degAction, string path = "")
    {
        GetComponent<Button>(path).onClick.AddListener(degAction);
    }
    public void BandingInputField_OnEndEdit(UnityAction<string> degAction, string path = "")
    {
        GetComponent<InputField>(path).onEndEdit.AddListener(degAction);
    }
    public void BandingInputField_OnValueChange(UnityAction<string> degAction, string path = "")
    {
        GetComponent<InputField>(path).onValueChanged.AddListener(degAction);
    }

    public T GetComponent<T>(string path = "") where T : Component
    {
        if (_panel == null)
        {
            Debug.LogError("[Widget] Panel is Null. (no init?))");
            return default(T);
        }
        Transform childObj = _panel.transform;
        if (!string.IsNullOrEmpty(path))
        {
            childObj = _panel.transform.Find(path);
            if (childObj == null)
            {
                Debug.LogErrorFormat("[Widget] found PanelChild. Panel:{0}, Path:{1}", _panel, path);
                return default(T);
            }
        }

        T component = childObj.GetComponent<T>();
        if (component == null)
        {
            Debug.LogErrorFormat("[Widget] found Component. Panel:{0}, Path:{1}, Component:{2}", _panel,path,typeof(T));
            return default(T);
        }

        return component;
    }
}
