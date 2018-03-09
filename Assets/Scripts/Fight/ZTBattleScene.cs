using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[CSharpCallLua]
public class ZTBattleScene : MonoBehaviour {
    private bool IsInit;
    void Awake()
    {
        IsInit = false;
    }
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDestroy()
    {
        if(IsInit)
        {
            //地图移除
            MapManager.GetInstance().Destroy();
        }
    }

    //启动管理器
    public void ManagerInit(Vector3 centerPos)
    {
        //地图初始化
        MapManager.GetInstance().InitMap(centerPos);

        IsInit = true;
    }

}
