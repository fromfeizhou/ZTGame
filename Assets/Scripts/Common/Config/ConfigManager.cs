using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCfgName
{
    test = 0,
    count,
}

public class ConfigManager {
    private static ConfigManager _instance;
    public static ConfigManager Instance {
        get {
            if (_instance == null)
                _instance = new ConfigManager();
            return _instance;
        }
    }
    public delegate void dCfgForeach(int row, int col, string value);

    private Dictionary<eCfgName, ConfigAsset> _cfgDic = new Dictionary<eCfgName, ConfigAsset>();
    private ConfigHoter cfgHoter = new ConfigHoter();

    public void Init()
    {
        int cfgCnt = (int)eCfgName.count;
        for (int i = 0; i < cfgCnt; i++)
        {
            eCfgName tmpCfgName = (eCfgName)i;
            string configPath = ConfigConst.ConfigResPath + tmpCfgName + ".asset";
            AssetManager.LoadAsset(configPath, (obj, str) => _cfgDic[tmpCfgName] = obj as ConfigAsset);
        }
#if UNITY_EDITOR
        ReLoadHotCfg();
#endif

    }

    private string GetValue(eCfgName cfgName, int row, int col)
    {
        string value = string.Empty;
#if UNITY_EDITOR
        if (cfgHoter.tryGetHotCfgValue(cfgName, row, col, out value))
            return value;
#endif
        value = _cfgDic[cfgName][row, col];
        return value;
    }

    public void CfgForeach(eCfgName cfgName, dCfgForeach cfgForeach)
    {
        for (int row = 0; row < _cfgDic[cfgName].MaxRow; row++)
        {
            for (int col = 0; col < _cfgDic[cfgName].MaxCol; col++)
            {
                cfgForeach(row,col, GetValue(cfgName, row, col));
            }
        }
    }

    public void ReLoadHotCfg()
    {
        cfgHoter.ReLoadHotCfg();
    }
}

