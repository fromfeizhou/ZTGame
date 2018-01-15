using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConfigDemo : MonoBehaviour {
    void Start()
    {
        ConfigManager.Instance.Init();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ConfigManager.Instance.CfgForeach(eCfgName.test, (row, col, value) =>
            {
                Debug.Log(string.Format("row:{0},col:{1},value:{2}.", row, col, value));
            });
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ConfigManager.Instance.ReLoadHotCfg();
        }
    }
}
