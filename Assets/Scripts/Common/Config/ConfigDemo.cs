using UnityEngine;

public class ConfigDemo : MonoBehaviour
{
    private ConfigManager configMgr;

    void Start()
    {
        configMgr = ConfigManager.GetInstance();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            configMgr.CfgForeach(eCfgName.test, (row, col, value) =>
            {
                Debug.Log(string.Format("row:{0},col:{1},value:{2}.", row, col, value));
            });
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            configMgr.ReLoadHotCfg();
        }
    }
}
