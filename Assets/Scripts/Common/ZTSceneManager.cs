using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XLua;

[LuaCallCSharp]
public class ZTSceneManager : MonoSingleton<ZTSceneManager> {
    private  string _sceneName;
    private  Action _callBack;

    public  void ReplaceScene(string sceneName,Action callBack)
    {
        _sceneName = sceneName;
        _callBack = callBack;
        StartCoroutine(onLoadGameScene());
    }

    private  IEnumerator onLoadGameScene()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(_sceneName);
        yield return new WaitUntil(() => op.isDone);
        if (null != _callBack)
        {
            _callBack();
            _callBack = null;
        }
    }
}
