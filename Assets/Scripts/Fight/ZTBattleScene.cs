using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[CSharpCallLua]
public class ZTBattleScene : MonoBehaviour {
    private bool IsInit;
    private Action _update;
    public void SetUpdate(Action update)
    {
        _update = update;
    }

    void Awake()
    {
        IsInit = false;
    }
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        if (null != _update)
        {
            _update();
        }
	}

    void OnDestroy()
    {
        if(IsInit)
        {
            //地图移除
            MapManager.GetInstance().Destroy();
        }
        _update = null;
    }

    //启动管理器
    public void ManagerInit(Vector3 centerPos)
    {
        //地图初始化
        MapManager.GetInstance().InitMap(centerPos);

        IsInit = true;
    }

}
