using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigHoter{
    private class cHotCfg
    {
        public eCfgName cfgName;
        public int row;
        public int col;
        public string value;

        public static cHotCfg Parse(string str)
        {
            cHotCfg hotCfg = new cHotCfg();
            string[] param = str.Trim().Split('_');
            hotCfg.cfgName = (eCfgName)System.Enum.Parse(typeof(eCfgName), param[0]);
            hotCfg.row = int.Parse(param[1]);
            hotCfg.col = int.Parse(param[2]);
            hotCfg.value = param[3];
            return hotCfg;
        }
    }
    private List<cHotCfg> hotCfgList = new List<cHotCfg>();
    public void ReLoadHotCfg()
    {
        if (hotCfgList.Count > 0)
            hotCfgList.Clear();

        string[] hotCfg = System.IO.File.ReadAllLines("HotCfg.txt");
        for (int i = 0; i < hotCfg.Length; i++)
            hotCfgList.Add(cHotCfg.Parse(hotCfg[i]));
    }

    public bool tryGetHotCfgValue(eCfgName cfgName, int row, int col,out string value)
    {
        value = string.Empty;
        int index = hotCfgList.FindIndex(a => a.cfgName == cfgName && a.row == row && a.col == col);
        if (index >= 0)
        {
            value = hotCfgList[index].value;
            return true;
        }
        return false;
    }
}
